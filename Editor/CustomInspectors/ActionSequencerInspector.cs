/*
MIT License

Copyright (c) 2024 Tolin Simpson

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OGK;

namespace OGKEditor
{
    [CustomEditor(typeof(ActionSequencer))]
    public class ActionSequencerInspector : Editor
    {
        SerializedProperty updateMode, sequences;

        ActionSequencer self;

        private void OnEnable()
        {
            updateMode = serializedObject.FindProperty("updateMode");
            sequences = serializedObject.FindProperty("sequences");
            self = (ActionSequencer)target;
            if(sequences.arraySize == 0)
            {
                sequences.arraySize++;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if(sequences.arraySize == 0)
            {
                sequences.arraySize++;
                self.pagination = sequences.arraySize - 1;
            }
            else
            {
                DrawPagination();
                EditorGUILayout.Space();
                SerializedProperty currentSequence = sequences.GetArrayElementAtIndex(self.pagination - 1);

                EditorDecor.DrawHorizontalLine(true);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(currentSequence.FindPropertyRelative("actions"));
                EditorGUI.indentLevel--;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);
                EditorDecor.DrawHorizontalLine(false);
                EditorGUI.BeginDisabledGroup(Application.isPlaying == true);
                EditorGUILayout.PropertyField(updateMode);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
            }

           // base.DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPagination()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.Space();
            /*if (GUILayout.Button(new GUIContent(string.Empty, EditorGUIUtility.IconContent("ScriptableObject Icon").image, "Convert sequence to/from ScriptableObject"), GUILayout.Width(25), GUILayout.Height(18)))
            {
                self.loadingSO = !self.loadingSO;
            }*/
            self.previousPage = self.pagination;
            if (GUILayout.Button(new GUIContent("←", "Back"), EditorStyles.miniButton, GUILayout.Width(23)))
            {
                self.pagination--;
            }
            self.pagination = EditorGUILayout.IntField(self.pagination, GUILayout.Width(25));
            EditorGUILayout.LabelField("/ " + sequences.arraySize, EditorStyles.whiteLabel, GUILayout.Width(22));
            if (GUILayout.Button(new GUIContent("→", "Next"), EditorStyles.miniButtonLeft, GUILayout.Width(23)))
            {
                self.pagination++;
            }

            if(sequences.arraySize > 1)
            {
                if (GUILayout.Button(new GUIContent("+", "Add Sequence"), EditorStyles.miniButtonMid, GUILayout.Width(22)))
                {
                    sequences.InsertArrayElementAtIndex(sequences.arraySize);
                }
                if (GUILayout.Button(new GUIContent("-", "Remove Sequence"), EditorStyles.miniButtonRight, GUILayout.Width(22)))
                {
                    sequences.MoveArrayElement(self.pagination - 1, 0);
                    sequences.DeleteArrayElementAtIndex(0);
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("+", "Add Sequence"), EditorStyles.miniButtonRight, GUILayout.Width(22)))
                {
                    sequences.InsertArrayElementAtIndex(sequences.arraySize);
                }
            }

            if (self.pagination > sequences.arraySize)
            {
                self.pagination = sequences.arraySize;
            }
            if (self.pagination < 1)
            {
                self.pagination = 1;
            }
            string label = string.Empty;
            SerializedProperty enabled = sequences.GetArrayElementAtIndex(self.pagination - 1).FindPropertyRelative("enabled");
            if (enabled.boolValue == false)
            {
                label = "Disabled";
            }
            else
            {
                label = "Enabled";
            }
            enabled.boolValue = EditorGUILayout.ToggleLeft(label, enabled.boolValue, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            // Enable right-click copy/paste:
            EditorGUI.BeginProperty(GUILayoutUtility.GetLastRect(), GUIContent.none, sequences);// sequences.GetArrayElementAtIndex(self.pagination - 1)); // Alternatevly copy a single behavior.
            EditorGUI.EndProperty();

            // Debug current behavior evaluation state:
            // if (Application.isPlaying == true) { EditorGUILayout.LabelField("Evaluating: # " + self.currentBehavior, EditorStyles.centeredGreyMiniLabel); }
        }


        [MenuItem("GameObject/Open Game Kit/Action Sequencer")]
        static void CreateActionSequencer()
        {
            if (FindObjectOfType<AppManager>() == null)
            {
                GameObject instance = new GameObject("Application Manager").AddComponent<AppManager>().gameObject;
                Debug.Log("A AppManager was missing, creating one...", instance);
            }
            Selection.activeGameObject = new GameObject("Action Sequencer").AddComponent<ActionSequencer>().gameObject;
        }
    }
}