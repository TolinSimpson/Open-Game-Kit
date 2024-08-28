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
    /// <summary>
    /// Custom <see cref="Editor"/> for <see cref="AppManager"/>.cs
    /// </summary>
    [CustomEditor(typeof(AppManager))]
    public class AppManagerInspector : Editor
    {
        SerializedProperty updateDelay, throttleUpdates, targetFrameRate, currentFPS, standardUpdateSequencers, fixedUpdateSequencers, lateUpdateSequencers, pauseUpdates, pools, onApplicationQuit;

        private bool showPools, showAppManagerHelp, showCopyright;

        private void OnEnable()
        {
            updateDelay = serializedObject.FindProperty("updateDelay");
            throttleUpdates = serializedObject.FindProperty("throttleUpdates");
            currentFPS = serializedObject.FindProperty("currentFPS");
            targetFrameRate = serializedObject.FindProperty("targetFrameRate");
            standardUpdateSequencers = serializedObject.FindProperty("standardUpdateSequencers");
            fixedUpdateSequencers = serializedObject.FindProperty("fixedUpdateSequencers");
            lateUpdateSequencers = serializedObject.FindProperty("lateUpdateSequencers");
            pauseUpdates = serializedObject.FindProperty("pauseUpdates");
            pools = serializedObject.FindProperty("pools");
            onApplicationQuit = serializedObject.FindProperty("onApplicationQuit");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DrawAppManagerHelp();

            DrawFrameUpdateProperties();

            DrawPoolingUpdateProperties();

            DrawDebugProperties();

            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(onApplicationQuit);
            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
            // EditorDecor.DrawCopyrightNotice("https://github.com/TolinSimpson", "https://github.com/TolinSimpson", "https://github.com/TolinSimpson", "2024", "Kitbashery", ref showCopyright);

            if (serializedObject.hasModifiedProperties == true)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DrawFrameUpdateProperties()
        {
            // Draw frame update properties:
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Frame Updates:", EditorStyles.boldLabel);
            EditorDecor.DrawHorizontalLine(false);

            EditorGUILayout.PropertyField(pauseUpdates);
            EditorGUILayout.PropertyField(throttleUpdates);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(targetFrameRate);
            if (EditorGUI.EndChangeCheck())
            {
                Application.targetFrameRate = targetFrameRate.intValue;
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawPoolingUpdateProperties()
        {
            // Draw pooling properties:
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Pooling:", EditorStyles.boldLabel);
            EditorDecor.DrawHorizontalLine(false);
            showPools = EditorDecor.DrawFoldout(showPools, "Pools", false);
            if (showPools == true)
            {
                EditorGUI.indentLevel++;
                for (int i = pools.arraySize - 1; i >= 0; i--)
                {
                    SerializedProperty prefab = pools.GetArrayElementAtIndex(i).FindPropertyRelative("prefab");
                    if (Application.isPlaying == false)
                    {
                        SerializedProperty amount = pools.GetArrayElementAtIndex(i).FindPropertyRelative("amount");
                        SerializedProperty maxAmount = pools.GetArrayElementAtIndex(i).FindPropertyRelative("maxAmount");

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(prefab);

                        if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(25)))
                        {
                            pools.DeleteArrayElementAtIndex(i);
                            break;
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.PropertyField(pools.GetArrayElementAtIndex(i).FindPropertyRelative("hideFlags"), GUILayout.Width(Screen.width - 68f));
                        EditorGUILayout.PropertyField(pools.GetArrayElementAtIndex(i).FindPropertyRelative("sequencialNaming"));

                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(amount);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (amount.intValue > maxAmount.intValue)
                            {
                                amount.intValue = maxAmount.intValue;
                            }
                        }
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(maxAmount);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (maxAmount.intValue < amount.intValue)
                            {
                                amount.intValue = maxAmount.intValue;
                            }
                        }

                        EditorDecor.DrawHorizontalLine(true);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Prefab Name: " + prefab.objectReferenceValue.name);
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(pools.GetArrayElementAtIndex(i).FindPropertyRelative("pooledObjects"));
                        EditorGUI.EndDisabledGroup();
                        EditorDecor.DrawHorizontalLine(false);
                    }

                    EditorGUILayout.Space();

                }

                if (Application.isPlaying == false)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("Add Pool:");
                    if (GUILayout.Button(string.Empty, "OL Plus", GUILayout.Width(20)))
                    {
                        pools.InsertArrayElementAtIndex(0);
                        SerializedProperty newPool = pools.GetArrayElementAtIndex(0);
                        newPool.FindPropertyRelative("prefab").objectReferenceValue = null;
                        newPool.FindPropertyRelative("amount").intValue = 1;
                        newPool.FindPropertyRelative("maxAmount").intValue = 100;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawDebugProperties()
        {
            // Draw debug properties:
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Debug:", EditorStyles.boldLabel);
            EditorDecor.DrawHorizontalLine(false);
            EditorGUI.BeginDisabledGroup(true);
            if (throttleUpdates.boolValue == true)
            {
                EditorGUILayout.PropertyField(updateDelay);
                EditorGUILayout.PropertyField(currentFPS);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(standardUpdateSequencers, GUILayout.Width(Screen.width - 40));
            EditorGUILayout.PropertyField(fixedUpdateSequencers, GUILayout.Width(Screen.width - 40));
            EditorGUILayout.PropertyField(lateUpdateSequencers, GUILayout.Width(Screen.width - 40));
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            /*if (Application.isPlaying == true && GUILayout.Button("Stress Test (+100k)"))
            {
                throttleUpdates.boolValue = true;
                for (int i = 0; i < 100000; i++)
                {
                    new GameObject("SmartGO").AddComponent<SmartGameObject>();
                }
            }*/

            EditorGUILayout.EndVertical();
        }

        private void DrawAppManagerHelp()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            GUILayout.Space(7);
            EditorDecor.DrawHorizontalLine(true);
            EditorGUILayout.EndVertical();

            if (GUILayout.Button(EditorGUIUtility.IconContent("_help"), GUIStyle.none, GUILayout.Width(20))) { showAppManagerHelp = !showAppManagerHelp; }
            EditorGUILayout.EndHorizontal();

            if (showAppManagerHelp == true)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box(EditorGUIUtility.IconContent("_help@2x"), GUIStyle.none);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Frame Updates:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Frame update options control the frequency of Smart GameObject updates based on the application's framerate target & current FPS. Note: update throttling does not apply to Smart GameObjects with rigidbodies.", EditorDecor.wrappedMiniLabel);
                EditorDecor.DrawHorizontalLine(false);
                EditorGUILayout.LabelField("Pooling:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Pooling recycles GameObjects to save instantiation overhead improving performance. Pools can be accessed by the name of the object that it creates.", EditorDecor.wrappedMiniLabel);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }


        [MenuItem("GameObject/Open Game Kit/Application Manager")]
        static void CreateAppManager()
        {
            AppManager instance = FindObjectOfType<AppManager>();
            if (instance == null)
            {
                Selection.activeGameObject = new GameObject("Application Manager").AddComponent<AppManager>().gameObject;
            }
            else
            {
                Debug.Log("There is already an AppManager in the scene.", instance);
                Selection.activeGameObject = instance.gameObject;
            }
        }
    }
}