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
using UnityEngine.UI;
using UnityEngine.Events;

namespace OGK
{
    /// <summary>
    /// Toggles a UI element between two sizes.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Open Game Kit/UI/Size Toggle")]
    public class UISizeToggle : MonoBehaviour
    {
        #region Properties:

        public RectTransform rect;
        [Tooltip("Note: Elements with size driven by GridLayoutGroup will not resize.")]
        public LayoutGroup layoutGroup;
        public bool toggleState;
        public Vector2 onSize;
        public Vector2 offSize;

        public UnityEvent on;
        public UnityEvent off;
        public UnityEvent toggled;

        private Vector2 onCache;
        private Vector2 offCache;
        private RectTransform layoutRoot;

        #endregion

        #region Initialization & Updates:

        void Start()
        {
            InitializeRectToggle();
            if (toggleState == false)
            {
                SetSizeByState(false);
            }
        }

        #endregion

        #region Methods:

        public void Toggle()
        {
            toggleState = !toggleState;
            SetSizeByState(toggleState);
        }

        public void SetSizeByState(bool toggle)
        {
            if (toggle == false)
            {
                rect.sizeDelta = offCache;

                off.Invoke();
            }
            else
            {
                rect.sizeDelta = onCache;

                on.Invoke();
            }

            toggled.Invoke();

            if (layoutGroup != null && layoutRoot.gameObject.activeSelf == true)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
            }
        }

        private void InitializeRectToggle()
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
            }

            if (layoutGroup != null)
            {
                if (layoutRoot == null)
                {
                    layoutRoot = (RectTransform)layoutGroup.transform;
                }
            }

            onCache = new Vector2(onSize.x, onSize.y);
            offCache = new Vector2(onSize.x, offSize.y);
        }

        #endregion
    }
}