#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    [InitializeOnLoad]
    public static class EditorWindowUtility
    {
        #region Variables

        private static string filePath = "Assets/Gaskellgames/GgCore/Utilities/ProjectUtility/Data/";
        private static string asset = "GaskellgamesHubSettings.asset";
        
        public static GaskellgamesHubSettings_SO settings;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Constructor

        static EditorWindowUtility()
        {
            Initialisation();
            EditorApplication.update += RunOnceOnStartup;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private static void Initialisation()
        {
            string completePath;
            if (Directory.Exists(filePath))
            {
                completePath = filePath + asset;
                settings = AssetDatabase.LoadAssetAtPath<GaskellgamesHubSettings_SO>(completePath);
            }
            else
            {
                string[] results = AssetDatabase.FindAssets("GaskellgamesHubSettings");
                foreach (string guid in results)
                {
                    completePath = AssetDatabase.GUIDToAssetPath(guid);
                    settings = AssetDatabase.LoadAssetAtPath<GaskellgamesHubSettings_SO>(completePath);
                }
            }
        }
        
        private static void RunOnceOnStartup()
        {
            if (!settings) { Initialisation(); }
            if (!settings || !settings.showHubOnStartup || SessionState.GetBool("EditorWindowUtilityFirstInit", false)) { return; }
            GaskellgamesHub.OpenWindow_WindowMenu();
            SessionState.SetBool("EditorWindowUtilityFirstInit", true);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Public Functions

        public static Texture LoadInspectorBanner(string textureFilepath)
        {
            return (Texture)AssetDatabase.LoadAssetAtPath(textureFilepath, typeof(Texture));
        }
        
        public static bool TryDrawBanner(Texture banner, bool editorWindow = false, bool forceShow = false)
        {
            // null and condition check
            if (settings == null) { return false; }
            if (!forceShow) { if (!settings.showPackageBanners) { return false; } }
            
            float imageWidth = EditorGUIUtility.currentViewWidth;
            float imageHeight = imageWidth * banner.height / banner.width;
            Rect rect = GUILayoutUtility.GetRect(imageWidth, imageHeight);
            
            // adjust rect to account for offsets in inspectors
            if (!editorWindow)
            {
                float paddingTop = -4;
                float paddingLeft = -18;
                float paddingRight = -4;
                
                // calculate rect size
                float xMin = rect.x + paddingLeft;
                float yMin = rect.y + paddingTop;
                float width = rect.width - (paddingLeft + paddingRight);
                float height = rect.height;

                rect = new Rect(xMin, yMin, width, height);
            }
            
            GUI.DrawTexture(rect, banner, ScaleMode.ScaleToFit);
            return true;
        }

        #endregion
        
        
    } // class end
}

#endif