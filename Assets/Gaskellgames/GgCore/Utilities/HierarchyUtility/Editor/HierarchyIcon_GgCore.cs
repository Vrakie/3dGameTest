#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Code created by Gaskellgames
/// </summary>

namespace Gaskellgames.EditorOnly
{
    [InitializeOnLoad]
    public class HierarchyIcon_GgCore
    {
        #region Variables

        private static readonly Texture2D icon_Comment;
        private static readonly Texture2D icon_LockToLayer;
        private static readonly Texture2D icon_SelectionTarget;

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Editor Loop

        static HierarchyIcon_GgCore()
        {
            icon_Comment = AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/GgCore/Utilities/HierarchyUtility/Editor/Icons/Icon_Comment.png", typeof(Texture2D)) as Texture2D;
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon_Comment;
            
            icon_LockToLayer = AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/GgCore/Utilities/HierarchyUtility/Editor/Icons/Icon_LayerLock.png", typeof(Texture2D)) as Texture2D;
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon_LockToLayer;
            
            icon_SelectionTarget = AssetDatabase.LoadAssetAtPath("Assets/Gaskellgames/GgCore/Utilities/HierarchyUtility/Editor/Icons/Icon_SelectionTarget.png", typeof(Texture2D)) as Texture2D;
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon_SelectionTarget;
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        private static void DrawHierarchyIcon_Comment(int instanceID, Rect position)
        {
            HierarchyUtility.DrawHierarchyIcon<Comment>(instanceID, position, icon_Comment);
        }

        private static void DrawHierarchyIcon_LockToLayer(int instanceID, Rect position)
        {
            int offset = HierarchyUtility.CheckForHierarchyIconOffset<Comment>(instanceID);
            HierarchyUtility.DrawHierarchyIcon<LockToLayer>(instanceID, position, icon_LockToLayer, offset);
        }

        private static void DrawHierarchyIcon_SelectionTarget(int instanceID, Rect position)
        {
            int offset = HierarchyUtility.CheckForHierarchyIconOffset<Comment>(instanceID);
            offset += HierarchyUtility.CheckForHierarchyIconOffset<LockToLayer>(instanceID);
            HierarchyUtility.DrawHierarchyIcon<SelectionTarget>(instanceID, position, icon_SelectionTarget, offset);
        }

        #endregion
        
    } // class end
}

#endif