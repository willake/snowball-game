using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
// Don't edit if you are not sure
// reference: https://manuel-rauber.com/2022/05/23/instantiate-your-own-prefabs-via-gameobject-menu/

#if UNITY_EDITOR
namespace Game.UI
{
    public static class UIComponentMenus
    {
        private const string PrefabManagerPath = "Assets/StaticAssets/UI/UIComponentPrefabManager.asset";

        private static UIComponentPrefabManagerSO LocatePrefabManager() =>
            AssetDatabase.LoadAssetAtPath<UIComponentPrefabManagerSO>(PrefabManagerPath);
        private const int MenuPriority = -50;


        private static void SafeInstantiate(Func<UIComponentPrefabManagerSO, GameObject> itemSelector)
        {
            var prefabManager = LocatePrefabManager();

            if (!prefabManager)
            {
                Debug.LogWarning($"PrefabManager not found at path {PrefabManagerPath}");
                return;
            }

            var item = itemSelector(prefabManager);
            var instance = PrefabUtility.InstantiatePrefab(item, Selection.activeTransform);
            PrefabUtility.UnpackPrefabInstance((GameObject)instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }

        [MenuItem("GameObject/UI/Custom Components/EmptyPanel", priority = MenuPriority)]
        private static void CreateEmptyPanel()
        {
            SafeInstantiate(prefabManager => prefabManager.UIPanels.EmptyPanel);
        }

        [MenuItem("GameObject/UI/Custom Components/EmptyPanel(with background)", priority = MenuPriority)]
        private static void CreateEmptyPanelWithBackground()
        {
            SafeInstantiate(prefabManager => prefabManager.UIPanels.EmptyPanelWithBackground);
        }

        [MenuItem("GameObject/UI/Custom Components/Button", priority = MenuPriority)]
        private static void CreateButton()
        {
            SafeInstantiate(prefabManager => prefabManager.Buttons.Button);
        }

        [MenuItem("GameObject/UI/Custom Components/Text Button", priority = MenuPriority)]
        private static void CreateTextButton()
        {
            SafeInstantiate(prefabManager => prefabManager.Buttons.TextButton);
        }

        [MenuItem("GameObject/UI/Custom Components/Text", priority = MenuPriority)]
        private static void CreateText()
        {
            SafeInstantiate(prefabManager => prefabManager.Texts.Text);
        }
    }
}
#endif