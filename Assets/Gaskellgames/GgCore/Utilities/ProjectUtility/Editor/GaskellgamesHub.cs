#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public class GaskellgamesHub : GGEditorWindow
    {
        #region Variables

        private const string assetPath = "Assets/Gaskellgames/GgCore/Utilities/ProjectUtility/Editor/Icons/";
        private const string bannerPath = assetPath + "InspectorBanner_SettingsHub.png";
        
        public string[] PopUp_ShowAtStartUp = new string[] {"Always", "Never"}; // TBC: "On New Version"
        public int index_ShowAtStartUp;

        public string[] PopUp_PackageBanners = new string[] {"Always", "Never"};
        public int index_PackageBanners;

        public string[] PopUp_TransformExtension = new string[] {"Transform Utilities", "Reset Buttons", "Default Unity"};
        public int index_TransformExtension;
        
        private static readonly int windowWidth = 725;
        private static readonly int windowHeight = 525;
        private static readonly int logoSize = 75;

        // package paths - downloadable
        private const string PlatformController = "Assets/Gaskellgames/Platform Controller/Editor/Icons/Logo_PlatformController.png";
        private const string CameraController = "Assets/Gaskellgames/Camera System/Editor/Icons/Logo_CameraSystem.png";
        private const string CharacterController = "Assets/Gaskellgames/Character Controller/Editor/Icons/Logo_CharacterController.png";
        private const string AudioController = "Assets/Gaskellgames/Audio System/Editor/Icons/Logo_AudioSystem.png";
        private const string SceneController = "Assets/Gaskellgames/Scene Controller/Editor/Icons/Logo_SceneController.png";
        private const string FolderSystem = "Assets/Gaskellgames/Folder System/Editor/Icons/Logo_HierarchyFolderSystem.png";
        private const string SplineSystem = "Assets/Gaskellgames/Spline System/Editor/Icons/Logo_SplineSystem.png";
        
        // package paths - helper
        private const string InputEventSystem = "Assets/Gaskellgames/Input Event System/Editor/Icons/Logo_InputEventSystem.png";
        private const string LogicSystem = "Assets/Gaskellgames/Logic System/Editor/Icons/Logo_LogicSystem.png";
        private const string PoolingSystem = "Assets/Gaskellgames/Pooling System/Editor/Icons/Logo_PoolingSystem.png";
        
        // package paths - powered by
        private const string GgCore = "Assets/Gaskellgames/GgCore/Utilities/ProjectUtility/Editor/Icons/Logo_GgCore.png";
        
        // package icons - helper
        private Texture icon_PlatformController;
        private Texture icon_CameraController;
        private Texture icon_CharacterController;
        private Texture icon_AudioController;
        private Texture icon_SceneController;
        private Texture icon_FolderSystem;
        private Texture icon_SplineSystem;
        
        // package icons - helper
        private Texture icon_InputEventSystem;
        private Texture icon_PoolingSystem;
        private Texture icon_LogicSystem;
        
        // package icons - powered by
        private Texture icon_GgCore;
        
        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Menu Item
        
        [MenuItem(MenuItemUtility.Hub_ToolsMenu_Path, false, MenuItemUtility.Hub_ToolsMenu_Priority)]
        private static void OpenWindow_ToolsMenu()
        {
            OpenWindow_WindowMenu();
        }

        [MenuItem(MenuItemUtility.Hub_WindowMenu_Path, false, MenuItemUtility.Hub_WindowMenu_Priority)]
        public static void OpenWindow_WindowMenu()
        {
            OpenWindow<GaskellgamesHub>("Gaskellgames Hub", windowWidth, windowHeight, true);
        }
        
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Overriding Functions

        protected override void OnInitialise()
        {
            InitialiseSettings();
        }

        protected override void OnFocusChange()
        {
            InitialiseSettings();
        }

        protected override List<ToolbarItem> LeftToolbar()
        {
            string copyright = isWindowWide ? "Copyright \u00a9 2024 Gaskellgames. All rights reserved." : "\u00a9 2024 Gaskellgames.";
            List<ToolbarItem> leftToolbar = new List<ToolbarItem>
            {
                new (null, new GUIContent(copyright)),
            };

            return leftToolbar;
        }

        protected override List<ToolbarItem> RightToolbar()
        {
            string version = isWindowWide ? "Version 1.0.0" : "v1.0.0";
            List<ToolbarItem> leftToolbar = new List<ToolbarItem>
            {
                new (null, new GUIContent(version)),
            };

            return leftToolbar;
        }

        protected override GenericMenu OptionsToolbar()
        {
            GenericMenu toolsMenu = new GenericMenu();
            toolsMenu.AddItem(new GUIContent("Gaskellgames Unity Page"), false, OnSupport_AssetStoreLink);
            toolsMenu.AddItem(new GUIContent("Gaskellgames Discord"), false, OnSupport_DiscordLink);
            toolsMenu.AddItem(new GUIContent("Gaskellgames Website"), false, OnSupport_WebsiteLink);
            return toolsMenu;
        }
        
        protected override void OnPageGUI()
        {
            // draw window content
            EditorWindowUtility.TryDrawBanner(banner, true, true);
            DrawWelcomeMessage();
            EditorExtensions.DrawInspectorLine(InspectorExtensions.backgroundSeperatorColor, 4, 0);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            HandleShowAtStartUp();
            HandleShowPackageBanners();
            HandleTransformExtension();
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Powered By:", EditorStyles.boldLabel, GUILayout.Width(logoSize + 10));
            DrawPackageLogo(icon_GgCore, "Gg Core");
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            EditorExtensions.DrawInspectorLine(InspectorExtensions.backgroundSeperatorColor, 4, 0);
            EditorGUILayout.LabelField("Downloaded Packages:", EditorStyles.boldLabel);
            HandleDownloadedPackages();
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Private Functions
        
        private void InitialiseSettings()
        {
            banner = EditorWindowUtility.LoadInspectorBanner(bannerPath);
            LoadPackageIcons();
            
            switch (EditorWindowUtility.settings.showPackageBanners)
            {
                case true: // always
                    index_PackageBanners = 0;
                    break;
                case false: // never
                    index_PackageBanners = 1;
                    break;
            }
            
            switch (EditorWindowUtility.settings.showHubOnStartup)
            {
                case true: // always
                    index_ShowAtStartUp = 0;
                    break;
                case false: // never
                    index_ShowAtStartUp = 1;
                    break;
            }
        }

        private void LoadPackageIcons()
        {
            icon_PlatformController = (Texture)AssetDatabase.LoadAssetAtPath(PlatformController, typeof(Texture));
            icon_CameraController = (Texture)AssetDatabase.LoadAssetAtPath(CameraController, typeof(Texture));
            icon_CharacterController = (Texture)AssetDatabase.LoadAssetAtPath(CharacterController, typeof(Texture));
            icon_AudioController = (Texture)AssetDatabase.LoadAssetAtPath(AudioController, typeof(Texture));
            icon_SceneController = (Texture)AssetDatabase.LoadAssetAtPath(SceneController, typeof(Texture));
            icon_FolderSystem = (Texture)AssetDatabase.LoadAssetAtPath(FolderSystem, typeof(Texture));
            icon_SplineSystem = (Texture)AssetDatabase.LoadAssetAtPath(SplineSystem, typeof(Texture));
        
            icon_InputEventSystem = (Texture)AssetDatabase.LoadAssetAtPath(InputEventSystem, typeof(Texture));
            icon_PoolingSystem = (Texture)AssetDatabase.LoadAssetAtPath(PoolingSystem, typeof(Texture));
            icon_LogicSystem = (Texture)AssetDatabase.LoadAssetAtPath(LogicSystem, typeof(Texture));
            
            icon_GgCore = (Texture)AssetDatabase.LoadAssetAtPath(GgCore, typeof(Texture));
        }

        private void DrawWelcomeMessage()
        {
            GUI.enabled = false;
            float defaultHeight = EditorStyles.textField.fixedHeight;
            EditorStyles.textField.fixedHeight = 100;
            EditorGUILayout.TextArea("Thank you for installing a Gaskellgames asset, and welcome to the settings hub!\n\n" +
                                     "Any settings options you choose here will be applied to all relevant Gaskellgames packages.\n\n" +
                                     "Links to the Unity Asset Store page, Gaskellgames Discord and Gaskellgames Website are available via the 'options' dropdown\n" +
                                     "menu above. (Note: Please read through each packages documentation pdf before contacting Gaskellgames with any queries.)");
            EditorStyles.textField.fixedHeight = defaultHeight;
            GUI.enabled = true;
        }

        private void HandleShowAtStartUp()
        {
            EditorGUILayout.BeginHorizontal();
            GUIContent label = new GUIContent("Show Hub On Startup", "Option to show/hide this window when loading into a project.");
            int changeCheck = index_ShowAtStartUp;
            index_ShowAtStartUp = EditorGUILayout.Popup(label, index_ShowAtStartUp, PopUp_ShowAtStartUp, GUILayout.Width(Screen.width * 0.4f));
            switch (index_ShowAtStartUp)
            {
                case 0: // always
                    EditorWindowUtility.settings.showHubOnStartup = true;
                    break;
                case 1: // never
                    EditorWindowUtility.settings.showHubOnStartup = false;
                    break;
                /*
                case 2: // on new version
                    break;
                */
            }
            if (changeCheck != index_ShowAtStartUp) { EditorUtility.SetDirty(EditorWindowUtility.settings); }
            EditorGUILayout.EndHorizontal();
        }
        
        private void HandleShowPackageBanners()
        {
            EditorGUILayout.BeginHorizontal();
            GUIContent label = new GUIContent("Show Package Banners", "Show or hide the Gaskellgames package header for component scripts.");
            int changeCheck = index_PackageBanners;
            index_PackageBanners = EditorGUILayout.Popup(label, index_PackageBanners, PopUp_PackageBanners, GUILayout.Width(Screen.width * 0.4f));
            switch (index_PackageBanners)
            {
                case 0: // always
                    EditorWindowUtility.settings.showPackageBanners = true;
                    break;
                case 1: // never
                    EditorWindowUtility.settings.showPackageBanners = false;
                    break;
            }
            if (changeCheck != index_PackageBanners) { EditorUtility.SetDirty(EditorWindowUtility.settings); }
            EditorGUILayout.EndHorizontal();
        }
        
        private void HandleTransformExtension()
        {
            EditorGUILayout.BeginHorizontal();
            GUIContent label = new GUIContent("Transform Inspector", "Enable or Disable the Gaskellgames transform utilities extension.");
            int changeCheck = index_TransformExtension;
            index_TransformExtension = EditorGUILayout.Popup(label, index_TransformExtension, PopUp_TransformExtension, GUILayout.Width(Screen.width * 0.4f));
            switch (index_TransformExtension)
            {
                case 0:
                    EditorWindowUtility.settings.enableTransformInspector = GaskellgamesHubSettings_SO.TransformInspector.TransformUtilities;
                    break;
                case 1:
                    EditorWindowUtility.settings.enableTransformInspector = GaskellgamesHubSettings_SO.TransformInspector.ResetButtons;
                    break;
                case 2:
                    EditorWindowUtility.settings.enableTransformInspector = GaskellgamesHubSettings_SO.TransformInspector.DefaultUnity;
                    break;
            }
            if (changeCheck != index_TransformExtension) { EditorUtility.SetDirty(EditorWindowUtility.settings); }
            EditorGUILayout.EndHorizontal();
        }
        
        private void HandleDownloadedPackages()
        {
            EditorGUILayout.BeginHorizontal();
            
            float xMin = spacing;
            xMin = DrawPackageLogo(icon_AudioController, "Audio Controller", true, xMin);
            xMin = DrawPackageLogo(icon_CameraController, "Camera System", true, xMin);
            xMin = DrawPackageLogo(icon_CharacterController, "Character Controller", true, xMin);
            xMin = DrawPackageLogo(icon_FolderSystem, "Folder System", true, xMin);
            xMin = DrawPackageLogo(icon_InputEventSystem, "Input Event System", true, xMin);
            xMin = DrawPackageLogo(icon_LogicSystem, "Logic System", true, xMin);
            xMin = DrawPackageLogo(icon_PlatformController, "Platform Controller", true, xMin);
            xMin = DrawPackageLogo(icon_PoolingSystem, "Pooling System", true, xMin);
            xMin = DrawPackageLogo(icon_SceneController, "Scene Controller", true, xMin);
            xMin = DrawPackageLogo(icon_SplineSystem, "Spline System", true, xMin);
            
            EditorGUILayout.EndHorizontal();
        }

        private float DrawPackageLogo(Texture packageLogo, string toolTip, bool autoWrap = false, float xMin = 0)
        {
            if (!packageLogo) { return xMin; }
            
            // handle auto wrap
            if (autoWrap && pageWidth < (xMin + logoSize + spacing))
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                xMin = spacing;
            }
            
            // draw package logo
            GUIContent label = new GUIContent(packageLogo, toolTip);
            GUILayout.Box(label, GUILayout.Width(logoSize), GUILayout.Height(logoSize));
            
            return xMin + logoSize + spacing;
        }

        #endregion
        
    } // class end
}

#endif