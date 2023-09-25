using System;
using UnityEngine;
using UnityEditor;
// Remove [CreateAssetMenu] when you've created an instance, because you don't need more than one.
[CreateAssetMenu(menuName = "MyGame/PrefabManager")]
public class UIComponentPrefabManagerSO : ScriptableObject
{
#if UNITY_EDITOR
    public GameObject Canvas;
    public UIPanelPrefabs UIPanels;
    public ButtonPrefabs Buttons;
    public TextPrefabs Texts;

    [Serializable]
    public class UIPanelPrefabs
    {
        public GameObject EmptyPanel;
        public GameObject EmptyPanelWithBackground;
    }

    [Serializable]
    public class ButtonPrefabs
    {
        public GameObject Button;
        public GameObject TextButton;
    }

    [Serializable]
    public class TextPrefabs
    {
        public GameObject Text;
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(UIComponentPrefabManagerSO))]
public class UIComponentPrefabManagerSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.HelpBox("If you move this file somewhere else, also change the path in UiLibraryMenus! ", MessageType.Info);
    }
}
#endif