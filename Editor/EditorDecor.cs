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

namespace OGKEditor
{
    public static class EditorDecor
    {
        public static GUIStyle richBoldLabel = new GUIStyle(EditorStyles.boldLabel) { richText = true };
        public static GUIStyle centeredBoldHelpBox = new GUIStyle(EditorStyles.helpBox) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        public static GUIStyle wrappedMiniLabel = new GUIStyle(GUI.skin.label) { wordWrap = true, fontSize = 10 };
        public static GUIStyle wrappedCenteredGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { wordWrap = true, richText = true };
        public static GUIStyle miniLabel = new GUIStyle(GUI.skin.label) { clipping = TextClipping.Overflow, fontSize = 10 };
        public static GUIStyle centeredMiniLabel = new GUIStyle(GUI.skin.label) { clipping = TextClipping.Overflow, fontSize = 10, alignment = TextAnchor.MiddleCenter };
        public static GUIStyle upperLeftMiniLabel = new GUIStyle(GUI.skin.label) { clipping = TextClipping.Overflow, fontSize = 10, alignment = TextAnchor.UpperLeft };
        public static GUIStyle centeredLabel = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        public static GUIStyle centeredBoldLabel = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
        public static GUIStyle middleLeftBoldLabel = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, fontStyle = FontStyle.Bold };
        public static GUIStyle lowerLeftBoldLabel = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerLeft, fontStyle = FontStyle.Bold };
        public static GUIStyle clippingBoldLabel = new GUIStyle(GUI.skin.label) { clipping = TextClipping.Overflow, fontStyle = FontStyle.Bold };
        public static GUIStyle richClippingBoldLabel = new GUIStyle(clippingBoldLabel) { richText = true };
        public static GUIStyle rightAlignedLabel = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
        public static GUIStyle richText = new GUIStyle(GUI.skin.label) { richText = true };
        public static GUIStyle richMiniButtonMid = new GUIStyle(EditorStyles.miniButtonMid) { richText = true };
        public static GUILayoutOption[] blankLabel = new GUILayoutOption[] { };
        public static GUILayoutOption[] thinLine = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) };
        public static GUILayoutOption[] thickLine = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(3) };

        // #FD6D40
        public static Color editorRed = new Color(0.9921569f, 0.427451f, 0.2509804f, 1f);
        // #B1FD59
        public static Color editorGreen = new Color(0.6941177f, 0.9921569f, 0.3490196f, 1f);
        // #77C3E5
        public static Color editorBlue = new Color(0.482353f, 0.8039216f, 0.9450981f, 1f);

        public static Color darkGrey = new Color(0.1647059f, 0.1647059f, 0.1647059f, 1f);

        /// <summary>
        /// A buffer for when copying <see cref="AnimationCurve"/>s.
        /// </summary>
        public static AnimationCurve curveBuffer;

        public static SerializedProperty properyBuffer;

        /// <summary>
        /// Draws a bold title with a help button that toggles a help box. 
        /// Useage example: myBool = DrawHelpTitleToggle(myBool, "title", "message");
        /// </summary>
        /// <param name="toggle">Boolean to pass in and return.</param>
        /// <param name="title">Text for the bold title.</param>
        /// <param name="text">Text for the help box to display.</param>
        /// <returns>toggle</returns>
        public static bool DrawHelpTitleToggle(bool toggle, string title, string text)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            if (GUILayout.Button(EditorGUIUtility.IconContent("_Help"), GUIStyle.none, GUILayout.Width(20))) { toggle = !toggle; }
            EditorGUILayout.EndHorizontal();
            if (toggle == true)
            {
                EditorGUILayout.HelpBox(text, MessageType.Info);
            }
            GUILayout.Box(string.Empty, thickLine);

            return toggle;
        }

        public static bool DrawFoldout(bool value, string label, bool fancy)
        {
            bool _value;
            if (fancy == true)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
            }

            _value = EditorGUILayout.Toggle(value, EditorStyles.foldout);

            EditorGUILayout.EndVertical();

            var rect = GUILayoutUtility.GetLastRect();
            rect.x += 20;
            rect.width -= 20;

            EditorGUI.LabelField(rect, label, richBoldLabel);
            return _value;
        }

        public static int DrawCompactPopup(string label, int value, string[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(75));
            value = EditorGUILayout.Popup(value, options);
            EditorGUILayout.EndHorizontal();
            return value;
        }

        public static int DrawLabellessCompactPopup(int value, string[] options)
        {
            value = EditorGUILayout.Popup(value, options);
            return value;
        }

        public static void DrawHorizontalLine(bool thick = false)
        {
            if (thick == false)
            {
                GUILayout.Box(string.Empty, thinLine);
            }
            else
            {
                GUILayout.Box(string.Empty, thickLine);
            }
        }

        public static void DrawCopyrightNotice(string supportURL, string reviewURL, string licenseURL, string year, string companyName, ref bool fold)
        {
            EditorGUILayout.Space();
            GUI.backgroundColor = Color.gray;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_UnityEditor.InspectorWindow"), GUIStyle.none, GUILayout.Width(20)))
            {
                fold = !fold;
            }
            EditorGUILayout.LabelField("Copyright © " + year + " " + companyName + ". All Rights Reserved.", wrappedCenteredGreyMiniLabel);
            EditorGUILayout.EndHorizontal();
            if (fold == true)
            {
                // Application.OpenURL() may have a check like this built-in.
                /*if (!Uri.IsWellFormedUriString(supportURL, UriKind.RelativeOrAbsolute) || !Uri.IsWellFormedUriString(reviewURL, UriKind.RelativeOrAbsolute) || !Uri.IsWellFormedUriString(licenseURL, UriKind.RelativeOrAbsolute)) 
                { 
                    return;
                }*/

                DrawHorizontalLine(false);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("<color=#4874E3>Support Site</color>", wrappedCenteredGreyMiniLabel))
                {
                    Application.OpenURL(supportURL);
                }
                EditorGUILayout.LabelField("|", wrappedCenteredGreyMiniLabel);
                if (GUILayout.Button("<color=#4874E3>Write a Review</color>", wrappedCenteredGreyMiniLabel))
                {
                    Application.OpenURL(reviewURL);
                }
                EditorGUILayout.LabelField("|", wrappedCenteredGreyMiniLabel);
                if (GUILayout.Button("<color=#4874E3>View License</color>", wrappedCenteredGreyMiniLabel))
                {
                    Application.OpenURL(licenseURL);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = Color.white;
        }

        public static void GetInspectorWindow(ref EditorWindow inspector)
        {
            EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (EditorWindow editor in windows)
            {
                if (editor.titleContent.text == "Inspector")
                {
                    inspector = editor; break;
                }
            }
        }

        /// <summary>
        /// Draws a progress bar for a 0-1 float.
        /// </summary>
        /// <param name="progress">The current progress.</param>
        public static void Draw01FloatProgressBar(ref float progress)
        {
            EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight), progress, (Mathf.RoundToInt(progress * 100)).ToString() + "%");
        }

        /// <summary>
        /// Draws a colorable animation curve with copy/paste functionality.
        /// </summary>
        /// <param name="property">A reference to the serialized property of the AnimationCurve to modify.</param>
        /// <param name="content">The label and tooltip.</param>
        /// <param name="color">The color of the animation curve.</param>
        /// <param name="rect">Ideally an empty rect.</param>
        /// <param name="evt">Ideally should be Event.Current</param>
        public static void DrawAnimationCurve(ref SerializedProperty property, GUIContent content, Color color, Rect rect)
        {
            property.animationCurveValue = EditorGUILayout.CurveField(content, property.animationCurveValue, color, rect);
            EditorGUI.BeginProperty(GUILayoutUtility.GetLastRect(), GUIContent.none, property);
            EditorGUI.EndProperty();
        }
    }
}