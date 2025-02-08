#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code updated by Gaskellgames: https://github.com/Gaskellgames
    /// Original code created by ghysc: https://github.com/ghysc/SwitchAttribute
    /// </summary>
	
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfDrawer : PropertyDrawer
    {
        #region Variables

        private HideIfAttribute attributeAsType;
        private readonly float removeSpacing = -EditorGUIUtility.standardVerticalSpacing;

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Height

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            attributeAsType = attribute as HideIfAttribute;

            if (!GetConditionalResult(attributeAsType, property))
            {
                PropertyDrawer customProperty = PropertyDrawerExtensions.Find(property);
                if (customProperty != null)
                {
                    // custom property
                    return customProperty.GetPropertyHeight(property, label);
                }
                
                // 'built in' property
                return EditorGUI.GetPropertyHeight(property, label);
            }
            
            // hidden
            return removeSpacing;
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region OnGUI

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // open property and get reference to attribute instance
            EditorGUI.BeginProperty(position, label, property);

            // cache default values
            bool defaultGui = GUI.enabled;
			
            // draw property if not hidden
            if (!GetConditionalResult(attributeAsType, property))
            {
                PropertyDrawer customDrawer = PropertyDrawerExtensions.Find(property);
                if (customDrawer == null)
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
                else
                {
                    customDrawer.OnGUI(position, property, label);
                }
            }

            // reset default values
            GUI.enabled = defaultGui;
            
            // close property
            EditorGUI.EndProperty();
        }

        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Private Functions

        /// <summary>
        /// Calculates the logic gate result of all comparisons
        /// </summary>
        /// <param name="hideIf"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        private bool GetConditionalResult(HideIfAttribute hideIf, SerializedProperty property)
        {
            // get the logic result from the first condition
            bool logicResult = SerializedPropertyExtensions.Equals(property.GetField(hideIf.conditions[0].field), hideIf.conditions[0].comparison);
            
            // logic gate logic on the 'previous' condition along with the current condition
            for (var i = 1; i < hideIf.conditions.Length; i++)
            {
                bool thisResult = SerializedPropertyExtensions.Equals(property.GetField(hideIf.conditions[i].field), hideIf.conditions[i].comparison);
                logicResult = GgMaths.LogicGateOutputValue(logicResult, thisResult, hideIf.LogicGate);
            }

            return logicResult;
        }

        #endregion
		
    } // class end
}

#endif