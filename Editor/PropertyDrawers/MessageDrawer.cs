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

[CustomPropertyDrawer(typeof(Message))]
public class MessageDrawer : DecoratorDrawer
{
	Message messageAttribute
	{
		get { return ((Message)attribute); }
	}

	public override void OnGUI(Rect rect)
	{
		EditorStyles.helpBox.richText = true;

		GUIStyle style = EditorStyles.helpBox;
		style.richText = true;
		style.wordWrap = messageAttribute.wordWrap;

		rect.height = style.CalcHeight(new GUIContent(messageAttribute.message), EditorGUIUtility.currentViewWidth - 55);

		rect.y += messageAttribute.heightOffset;

		switch(messageAttribute.messageType)
        {
			case LogFormats.None:

				EditorGUI.HelpBox(rect, messageAttribute.message, MessageType.None);

				break;

			case LogFormats.Message:

				EditorGUI.HelpBox(rect, messageAttribute.message, MessageType.Info);

				break;

			case LogFormats.Error:

				EditorGUI.HelpBox(rect, messageAttribute.message, MessageType.Error);

				break;

			case LogFormats.Warning:

				EditorGUI.HelpBox(rect, messageAttribute.message, MessageType.Warning);

				break;
        }
	}
}