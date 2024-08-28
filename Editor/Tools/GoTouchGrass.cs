using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

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

namespace OGKEditor
{
    /// <summary>
    /// Go Touch Grass is a notification system to remind developers to take a break.
    /// </summary>
    [InitializeOnLoad]
    public class GoTouchGrass : EditorWindow
    {
        private static float timeInterval = 2700f;
        private static float nextExecutionTime = 0f;
        private static LocalizationOptions localization = LocalizationOptions.English;

        #region Messages:

        private static string[] messagesEnglish = new string[12]
        {
        "It's time to get up and stretch!",
        "Break time! Go touch some grass.",
        "You've been sitting a while, time to stretch.",
        "Time to give your eyes some rest.",
        "Error: Human may be tired, time to get up and stretch.",
        "It's time to take a break and stretch.",
        "Stand up and move around a bit, you've been sitting a while.",
        "Break time! Get a snack, and move around.",
        "Bet all that clicking is making you thirsty, get some water.",
        "Maybe if you take a break the solution will come to you.",
        "Don't let your hands cramp up, its time for a break.",
        "TIME TO TOUCH GRASS!",
        };

        private static string[] messagesJapanese = new string[12]
        {
        "起き上がってストレッチする時が来ました!",
        "休憩時間!草に触れてください。",
        "あなたはしばらく座っています、ストレッチする時間です。",
        "目を休ませる時間です。",
        "エラー:人間は疲れているかもしれません、起き上がってストレッチする時間。",
        "休憩してストレッチする時が来ました。",
        "立ち上がって少し動き回ると、しばらく座っています。",
        "休憩時間!おやつを食べて、動き回ってください。",
        "クリックすると喉が渇くことに賭けて、水を手に入れてください。",
        "たぶんあなたが休憩を取るならば、解決策はあなたに来るでしょう。",
        "あなたの手をけいれんさせないでください、休憩の時間です。",
        "草に触れる時が来ました!",
        };

        private static string[] messagesChineese = new string[12]
        {
        "是时候起床伸展身体了！",
        "休息时间！去摸点草。",
        "你已经坐了一会儿了，是时候伸展一下了。",
        "是时候让你的眼睛休息一下了。",
        "错误：人类可能累了，是时候起床伸展了。",
        "是时候休息一下，伸展一下了。",
        "站起来走动一下，你已经坐了一会儿了。",
        "休息时间！吃点零食，四处走动。",
        "打赌所有的点击都会让你口渴，喝点水。",
        "也许如果你休息一下，解决方案就会来找你。",
        "不要让你的手抽筋，是时候休息一下了。",
        "是时候摸草了！",
        };

        #endregion

        static GoTouchGrass()
        {
            EditorApplication.update += OnEditorUpdate;
            timeInterval = EditorPrefs.GetFloat("GoTouchGrass_TimeInterval", timeInterval);
            nextExecutionTime = EditorPrefs.GetFloat("GoTouchGrass_NextExecutionTime", (float)EditorApplication.timeSinceStartup + timeInterval);
            localization = (LocalizationOptions)EditorPrefs.GetInt("GoTouchGrass_Localization");
        }

        static void OnEditorUpdate()
        {
            if (EditorApplication.timeSinceStartup > nextExecutionTime)
            {
                ShowPopupWindow();
            }
        }

        static void OnDisable()
        {
            nextExecutionTime = 0f;
            EditorPrefs.SetFloat("GoTouchGrass_NextExecutionTime", nextExecutionTime);
        }

        public static void ShowPopupWindow()
        {
            EditorApplication.Beep();
            int dialogResult = 0;
            switch (localization)
            {
                case LocalizationOptions.English:

                    dialogResult = EditorUtility.DisplayDialogComplex("Go Touch Grass", messagesEnglish[Random.Range(0, messagesEnglish.Length)], "Next reminder in 30 minutes", timeInterval / 60 + " minutes", "1 hour");

                    break;

                case LocalizationOptions.日本語:

                    dialogResult = EditorUtility.DisplayDialogComplex("ゴータッチグラス", messagesJapanese[Random.Range(0, messagesJapanese.Length)], "30分後の次のリマインダー", timeInterval / 60 + " 議事録", "1 時");

                    break;

                case LocalizationOptions.简体中文:

                    dialogResult = EditorUtility.DisplayDialogComplex("去摸草", messagesChineese[Random.Range(0, messagesChineese.Length)], "30 分钟内的下一次提醒", timeInterval / 60 + " 纪要", "1小时");

                    break;
            }

            float delay;
            switch (dialogResult)
            {
                case 0: delay = 1800; break;
                case 1: delay = timeInterval; break;
                case 2: delay = 3600; break;
                default: delay = timeInterval; break;
            }

            nextExecutionTime = (float)EditorApplication.timeSinceStartup + delay;
            EditorPrefs.SetFloat("GoTouchGrass_NextExecutionTime", nextExecutionTime);
            Debug.LogFormat("Next break in {0} minutes.", delay / 60);
        }

        [SettingsProvider]
        public static SettingsProvider CreateGoTouchGrassSettingsProvider()
        {

            var provider = new SettingsProvider("Project/Go Touch Grass", SettingsScope.Project)
            {

                label = "Go Touch Grass",
                guiHandler = (searchContext) =>
                {
                    GUILayout.Box(string.Empty, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                    EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);
                    GUILayout.Space(10);

                    EditorGUILayout.BeginHorizontal();
                    timeInterval = EditorGUILayout.FloatField(new GUIContent("Time Interval (Seconds)", "The time in seconds between break notifications."), timeInterval);
                    EditorPrefs.SetFloat("GoTouchGrass_TimeInterval", timeInterval);
                    if (GUILayout.Button("Reset Timer", EditorStyles.miniButton))
                    {
                        nextExecutionTime = 0;
                        EditorPrefs.SetFloat("GoTouchGrass_NextExecutionTime", nextExecutionTime);
                    }
                    EditorGUILayout.EndHorizontal();

                    localization = (LocalizationOptions)EditorGUILayout.EnumPopup("Message Localization", localization);
                    EditorPrefs.SetInt("GoTouchGrass_Localization", (int)localization);
                    EditorGUILayout.Space();

                    GUIStyle wrappedCenteredGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { wordWrap = true, richText = true };

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField("Copyright © 2024 Tolin Simpson. All Rights Reserved.", wrappedCenteredGreyMiniLabel);
                    GUILayout.Box(string.Empty, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Support Site", EditorStyles.miniButtonLeft))
                    {
                        Application.OpenURL("https://kitbashery.com/");
                    }
                    if (GUILayout.Button("Write a Review", EditorStyles.miniButtonMid))
                    {
                        Application.OpenURL("https://assetstore.unity.com/packages/slug/254731");
                    }
                    if (GUILayout.Button("View License", EditorStyles.miniButtonRight))
                    {
                        Application.OpenURL("https://unity.com/legal/as-terms");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    GUI.backgroundColor = Color.white;
                },
                keywords = new HashSet<string>(new[] { "Go Touch Grass", "Settings" })
            };

            return provider;
        }

        private enum LocalizationOptions { English = 0, 日本語 = 1, 简体中文 = 2 }
    }
}