using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using WillakeD.CommonPatterns;
using System.Linq;
using Game.Inputs;

namespace Game.UI
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("references")]
        public Canvas canvas;
        public GameObject inputBlocker;

        private readonly Stack<UIPanel> _panelStack = new Stack<UIPanel>();

        private readonly HashSet<AvailableUI> _openedUIList = new HashSet<AvailableUI>();

        private readonly Dictionary<AvailableUI, UIPanel> _panelPool
            = new Dictionary<AvailableUI, UIPanel>();

        private UIPanel _focusing;
        private WDButton[] _selectableButtons;
        private int _selectedIndex;

        public bool enableKeyboardControl = false;

        private void Start()
        {
            if (enableKeyboardControl)
            {
                UIInputSet inputSet = InputManager.instance.inputSetUI;
                inputSet.ePressUpEvent.AddListener(KeyboardSelectPrev);
                inputSet.ePressDownEvent.AddListener(KeyboardSelectNext);
                inputSet.ePressConfirmEvent.AddListener(ClickSelectedButton);
                inputSet.ePressCancelEvent.AddListener(PerformCancelAction);
            }
        }

        public UIPanel OpenUI(AvailableUI ui)
        {
            if (_openedUIList.Contains(ui))
            {
                Debug.LogError("UI has been opened. There might be wrong implementation. ");
                return null;
            }

            if (_panelPool.TryGetValue(ui, out var panel) == false)
            {
                var prefab = GetUIPrefab(ui);
                if (prefab != null)
                {
                    var go = Instantiate(prefab, canvas.transform);
                    panel = go.GetComponent<UIPanel>();
                    _openedUIList.Add(ui);
                }
            }

            if (panel)
            {
                _panelStack.Push(panel);
                panel.Open();
                panel.transform.SetAsLastSibling();
                SetFocusing(panel);
            }

            return panel;
        }

        public async UniTask<UIPanel> OpenUIAsync(AvailableUI ui)
        {
            if (_openedUIList.Contains(ui))
            {
                Debug.LogError("UI has been opened. There might be wrong implementation. ");
                return null;
            }
            if (_panelPool.TryGetValue(ui, out UIPanel panel) == false)
            {
                GameObject prefab = GetUIPrefab(ui);
                if (prefab != null)
                {
                    GameObject go = Instantiate(prefab, canvas.transform);
                    panel = go.GetComponent<UIPanel>();
                    _openedUIList.Add(ui);
                }
            }

            if (panel)
            {
                _panelStack.Push(panel);
                panel.transform.SetAsLastSibling();
                BlockUIInput();
                await panel.OpenAsync();
                SetFocusing(panel);
            }
            UnblockUIInput();
            return panel;
        }

        private void SetFocusing(UIPanel panel)
        {
            _focusing = panel;

            if (enableKeyboardControl == false) return;

            _selectableButtons = _focusing.GetSelectableButtons();

            if (_selectableButtons.Length == 0)
            {
                _selectableButtons = null;
            }
            else
            {
                DeselectAllButtons();
                _selectedIndex = -1;
                // SelectButton(0);
            }
        }

        public void CloseAllUI()
        {
            while (_panelStack.Count > 0)
            {
                var panel = _panelStack.Pop();
                panel.Close();
                _panelPool[panel.Type] = panel;
            }

            ClearUICache();
        }

        public async UniTask CloseAllUIAsync()
        {
            BlockUIInput();
            while (_panelStack.Count > 0)
            {
                UIPanel panel = _panelStack.Pop();
                await panel.CloseAsync();
                _panelPool[panel.Type] = panel;
            }

            ClearUICache();
            UnblockUIInput();
        }

        public void ClearUICache()
        {
            var keys = _panelPool.Keys;
            var list = keys.ToList();

            foreach (AvailableUI type in list)
            {
                if (_panelPool[type] != null)
                {
                    var panel = _panelPool[type];
                    _panelPool.Remove(type);
                    Destroy(panel.gameObject);
                }
            }
            _openedUIList.Clear();
        }

        private GameObject GetUIPrefab(AvailableUI ui)
        {
            return ui switch
            {
                AvailableUI.MenuPanel => ResourceManager.instance.uiPanelResources.menuPanel,
                AvailableUI.GameHUDPanel => ResourceManager.instance.uiPanelResources.gameHUDPanel,
                AvailableUI.PausePanel => ResourceManager.instance.uiPanelResources.pausePanel,
                AvailableUI.EndGamePanel => ResourceManager.instance.uiPanelResources.endGamePanel,
                AvailableUI.LevelSelectPanel => ResourceManager.instance.uiPanelResources.levelSelectPanel,
                AvailableUI.SettingsPanel => ResourceManager.instance.uiPanelResources.settingsPanel,
                AvailableUI.GameStartPanel => ResourceManager.instance.uiPanelResources.gameStartPanel,
                AvailableUI.TutorialPanel => ResourceManager.instance.uiPanelResources.tutorialPanel,
                AvailableUI.ModalPanel => ResourceManager.instance.uiPanelResources.modalPanel,
                _ => null
            };
        }

        public void Prev()
        {
            var panel = _panelStack.Pop();
            panel.Close();
            _openedUIList.Remove(panel.Type);
            _panelPool[panel.Type] = panel;

            if (_panelStack.Count > 0) SetFocusing(_panelStack.Last());
        }

        public async UniTask PrevAsync()
        {
            BlockUIInput();
            UIPanel panel = _panelStack.Pop();
            await panel.CloseAsync();
            _openedUIList.Remove(panel.Type);
            _panelPool[panel.Type] = panel;

            if (_panelStack.Count > 0) SetFocusing(_panelStack.Last());
            UnblockUIInput();
        }

        private void KeyboardSelectPrev()
        {
            if (_selectableButtons == null) return;
            var index = _selectedIndex - 1;

            if (index < 0)
            {
                index = _selectableButtons.Length - 1;
            }

            if (_selectableButtons[index].IsInteractable == false)
            {
                index = index - 1;
            }

            if (index < 0)
            {
                index = _selectableButtons.Length - 1;
            }

            DeselectButton(_selectedIndex);
            SelectButton(index);
            _selectedIndex = index;
        }

        private void KeyboardSelectNext()
        {
            if (_selectableButtons == null) return;
            var index = _selectedIndex + 1;

            if (index > _selectableButtons.Length - 1)
            {
                index = 0;
            }

            if (_selectableButtons[index].IsInteractable == false)
            {
                index = index + 1;
            }

            if (index > _selectableButtons.Length - 1)
            {
                index = 0;
            }

            DeselectButton(_selectedIndex);
            SelectButton(index);
            _selectedIndex = index;
        }

        private void DeselectButton(int index)
        {
            if (_selectableButtons == null) return;
            WDButton button = _selectableButtons[index];
            button.Deselect();
        }

        private void DeselectAllButtons()
        {
            if (_selectableButtons == null) return;

            foreach (WDButton button in _selectableButtons)
            {
                button.Deselect();
            }
        }

        private void SelectButton(int index)
        {
            if (_selectableButtons == null) return;

            WDButton button = _selectableButtons[index];
            button.Select();
        }

        private void ClickSelectedButton()
        {
            if (_selectableButtons == null) return;
            WDButton button = _selectableButtons[_selectedIndex];
            button.Click();
        }

        private void PerformCancelAction()
        {
            _focusing.PerformCancelAction();
        }

        private void BlockUIInput()
        {
            // InputManager.instance.SetAllowInput(false);
            inputBlocker.SetActive(true);
            inputBlocker.transform.SetAsLastSibling();
        }

        private void UnblockUIInput()
        {
            // InputManager.instance.SetAllowInput(true);
            inputBlocker.SetActive(false);
        }
    }
}