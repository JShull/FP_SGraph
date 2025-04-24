namespace FuzzPhyte.SGraph.Editor
{
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    using FuzzPhyte.SGraph;
    [CustomEditor(typeof(FPSimpleEventData))]
    public class FPSimpleEventDataEditor:Editor
    {
        private ReorderableList transitionMapperList;

        private void OnEnable()
        {
            SerializedProperty mapperListProp = serializedObject
                .FindProperty("ModuleEventData")
                .FindPropertyRelative("TransitionMapperData");

            transitionMapperList = new ReorderableList(serializedObject, mapperListProp, true, true, true, true);

            transitionMapperList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Transition Mapper Data");
            };

            transitionMapperList.drawElementCallback = (Rect fullRect, int index, bool isActive, bool isFocused) =>
            {
                var element = transitionMapperList.serializedProperty.GetArrayElementAtIndex(index);
                float lineHeight = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;
                float y = fullRect.y + 4;
                float x = fullRect.x + 8;
                float width = fullRect.width - 16;

                // Draw visual box to contain everything
                GUI.Box(fullRect, GUIContent.none);

                // Start drawing fields inside box
                EditorGUI.PropertyField(new Rect(x, y, width, lineHeight), element.FindPropertyRelative("transitionName"));
                y += lineHeight + spacing;

                EditorGUI.PropertyField(new Rect(x, y, width, lineHeight), element.FindPropertyRelative("TransitionKey"));
                y += lineHeight + spacing;

                EditorGUI.PropertyField(new Rect(x, y, width, lineHeight), element.FindPropertyRelative("Outcome"));
                y += lineHeight + spacing;

                SerializedProperty req = element.FindPropertyRelative("RequirementData");
                float reqHeight = EditorGUI.GetPropertyHeight(req, true);
                EditorGUI.PropertyField(new Rect(x, y, width, reqHeight), req, new GUIContent("Requirement Data"), true);
                y += reqHeight + spacing;

                SerializedProperty useHelper = element.FindPropertyRelative("UseHelper");
                float helperHeight = EditorGUI.GetPropertyHeight(useHelper, true);
                EditorGUI.PropertyField(new Rect(x, y, width, helperHeight), useHelper, new GUIContent("Helper Shit"),true);
                y += helperHeight + spacing;

                if (useHelper.boolValue)
                {
                    EditorGUI.PropertyField(new Rect(x, y, width, lineHeight), element.FindPropertyRelative("UniqueHelperName"));
                    y += lineHeight + spacing;

                    EditorGUI.PropertyField(new Rect(x, y, width, lineHeight), element.FindPropertyRelative("TimeUntil"));
                    y += lineHeight + spacing;

                    SerializedProperty helperLogic = element.FindPropertyRelative("HelperLogic");
                    float usehelperHeight = EditorGUI.GetPropertyHeight(helperLogic, true);
                    EditorGUI.PropertyField(new Rect(x, y, width, usehelperHeight), helperLogic, new GUIContent("Helper Logic"), true);
                }
            };


            transitionMapperList.elementHeightCallback = (int index) =>
            {
                var element = transitionMapperList.serializedProperty.GetArrayElementAtIndex(index);
                float line = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;

                float total = 0f;
                total += line * 3 + spacing * 3; // Name, Key, Outcome
                total += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("RequirementData"), true) + spacing;
                total += line + spacing; // UseHelper

                if (element.FindPropertyRelative("UseHelper").boolValue)
                {
                    total += line * 2 + spacing * 2;
                    total += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("HelperLogic"), true) + spacing;
                }

                return total + 8; // padding inside the GUI.Box
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw top-level fields
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ModuleEventName"));

            SerializedProperty moduleData = serializedObject.FindProperty("ModuleEventData");
            EditorGUILayout.PropertyField(moduleData.FindPropertyRelative("eventName"), new GUIContent("Event Name"));
            EditorGUILayout.PropertyField(moduleData.FindPropertyRelative("StartingEventState"));

            // Draw Transition Mapper List
            transitionMapperList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
