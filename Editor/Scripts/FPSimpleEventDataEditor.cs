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

                Rect drawRect = new Rect(fullRect.x + 8, fullRect.y + 4, fullRect.width - 16, lineHeight);

                GUI.Box(fullRect, GUIContent.none);

                EditorGUI.PropertyField(drawRect, element.FindPropertyRelative("transitionName"));
                drawRect.y += lineHeight + spacing;

                EditorGUI.PropertyField(drawRect, element.FindPropertyRelative("TransitionKey"));
                drawRect.y += lineHeight + spacing;

                EditorGUI.PropertyField(drawRect, element.FindPropertyRelative("Outcome"));
                drawRect.y += lineHeight + spacing;

                SerializedProperty req = element.FindPropertyRelative("RequirementData");
                float reqHeight = EditorGUI.GetPropertyHeight(req, true);
                EditorGUI.PropertyField(new Rect(drawRect.x, drawRect.y, drawRect.width, reqHeight), req, new GUIContent("Requirement Data"), true);
                drawRect.y += reqHeight + spacing;

                EditorGUI.PropertyField(new Rect(drawRect.x, drawRect.y, drawRect.width, lineHeight), element.FindPropertyRelative("UseHelper"));
                drawRect.y += lineHeight + spacing;

                EditorGUI.PropertyField(new Rect(drawRect.x, drawRect.y, drawRect.width, lineHeight), element.FindPropertyRelative("UniqueHelperName"));
                drawRect.y += lineHeight + spacing;

                EditorGUI.PropertyField(new Rect(drawRect.x, drawRect.y, drawRect.width, lineHeight), element.FindPropertyRelative("TimeUntil"));
                drawRect.y += lineHeight + spacing;

                SerializedProperty helperLogic = element.FindPropertyRelative("HelperLogic");
                float helperHeight = EditorGUI.GetPropertyHeight(helperLogic, true);
                EditorGUI.PropertyField(new Rect(drawRect.x, drawRect.y, drawRect.width, helperHeight), helperLogic, new GUIContent("Helper Logic"), true);
            };



            transitionMapperList.elementHeightCallback = (int index) =>
            {
                var element = transitionMapperList.serializedProperty.GetArrayElementAtIndex(index);
                float line = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;

                float height = 0f;

                height += line + spacing; // transitionName
                height += line + spacing; // TransitionKey
                height += line + spacing; // Outcome
                height += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("RequirementData"), true) + spacing;
                height += line + spacing; // UseHelper
                height += line + spacing; // UniqueHelperName
                height += line + spacing; // TimeUntil
                height += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("HelperLogic"), true) + spacing;

                return height + 6f; // safe buffer
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
