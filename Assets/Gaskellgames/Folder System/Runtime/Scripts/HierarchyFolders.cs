#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// Code updated by Gaskellgames
/// </summary>

namespace Gaskellgames.FolderSystem
{
    [AddComponentMenu("Gaskellgames/Folder System/Hierarchy Folders")]
    public class HierarchyFolders : GGMonoBehaviour
    {
        #region Variables
        
        public enum TextAlignment
        {
            Left,
            Center
        }
        
        [SerializeField]
        [Tooltip("Font style to be applied to HierarchyFolder text.")]
        public FontStyle textStyle = FontStyle.BoldAndItalic;
        
        [SerializeField]
        [Tooltip("Alignment to be applied to HierarchyFolder text.")]
        public TextAlignment textAlignment = TextAlignment.Left;
        
        [SerializeField]
        [Tooltip("Toggles whether to use a custom color for the text.")]
        public bool customText;
        
        [SerializeField]
        [Tooltip("Custom color to be used for the text.")]
        public Color32 textColor = InspectorExtensions.textNormalColor;
        
        [SerializeField]
        [Tooltip("Toggles whether to use a custom color for the icon.")]
        public bool customIcon;
        
        [SerializeField]
        [Tooltip("Custom color to be used for the icon.")]
        public Color32 iconColor = InspectorExtensions.textNormalColor;
        
        [SerializeField]
        [Tooltip("Toggles whether to use a custom color for the highlight (Background).")]
        public bool customHighlight;
        
        [SerializeField]
        [Tooltip("Custom color to be used for the highlight (Background).")]
        public Color32 highlightColor = InspectorExtensions.cyanColor;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Editor Loop

        private void OnValidate()
        {
            // remove all other components
            foreach (Component component in gameObject.GetComponents<Component>())
            {
                if ( !(component is Transform || component is HierarchyFolders) )
                {
                    Log(component.name + "destroyed: Folders cannot contain other components.", LogType.Warning);
                    DestroyImmediate(component);
                }
            }
        }

        #endregion
        
    } // class end
}
        
#endif