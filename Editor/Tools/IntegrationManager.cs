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
    public class IntegrationManager : EditorWindow
    {
        static IntegrationManager window;
        private static List<Integration> integrations = new List<Integration>();

        [MenuItem("Tools/Open Game Kit/Integration Manager")]
        static void ShowIntegrationManagerWindow()
        {
            window = (IntegrationManager)GetWindow(typeof(IntegrationManager));
            window.Show();
            window.GetIntegrations();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Integrations:", EditorStyles.centeredGreyMiniLabel);

            if (integrations.Count == 0)
            {
                if (GUILayout.Button("Refresh"))
                {
                    GetIntegrations();
                }
            }
            else
            {
                foreach (Integration integration in integrations)
                {

                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginChangeCheck();

                    integration.enabled = EditorGUILayout.Toggle(integration.integrationName, integration.enabled);

                    if (EditorGUI.EndChangeCheck())
                    {
                        ToggleIntegration(integration, integration.enabled);
                    }
                    if (integration.url != string.Empty)
                    {
                        if (GUILayout.Button(EditorGUIUtility.IconContent("_help"), GUIStyle.none, GUILayout.Width(20))) { Application.OpenURL(integration.url); }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.LabelField(integration.defineSymbol, EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.HelpBox("Notice: Changes will trigger script compilation.", MessageType.None);
            }
        }

        private void GetIntegrations()
        {
            integrations.Clear();
            string[] guid = AssetDatabase.FindAssets("t:Integration");
            foreach (string id in guid)
            {
                integrations.Add((Integration)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(id), typeof(Integration)));
            }
        }

        private void ToggleIntegration(Integration integration, bool toggle)
        {
            Integration obj = CreateInstance<Integration>();
            obj = integration;
            SerializedObject serializedIntegration = new SerializedObject(obj);
            SerializedProperty enabled = serializedIntegration.FindProperty("enabled");
            string symbols = string.Empty;
            enabled.boolValue = toggle;
            symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (string.IsNullOrEmpty(integration.defineSymbol) == false)
            {
                if (toggle == true)
                {
                    symbols = symbols.Insert(symbols.Length, integration.defineSymbol); //TODO: test if a comma needs to be added.
                }
                else
                {
                    if (string.IsNullOrEmpty(symbols) == false)
                    {
                        symbols = symbols.Replace(integration.defineSymbol, string.Empty);
                    }
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                serializedIntegration.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
        }
    }
}