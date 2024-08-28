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
    /// Toggles a canvas group.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    [AddComponentMenu("Open Game Kit/UI/Group Toggle")]
    public class UIGroupToggle : MonoBehaviour
    {
        public CanvasGroup group;
        public UIGroupToggle subGroup;

        [Tooltip("If specified the layoutGroup will be updated.")]
        public LayoutGroup layoutGroup;
        public bool gridLayoutGroup = false;
        public bool resetGridLayoutSiblingIndex = false;
        private int originalChildIndex;
        private RectTransform layoutRoot;
        private RectTransform rect;
        private Vector2 originalSize;

        public UnityEvent on;
        public UnityEvent off;
        public UnityEvent toggled;

        private void Start()
        {
            if (layoutGroup != null && rect == null)
            {
                if (gridLayoutGroup == true)
                {
                    originalChildIndex = transform.GetSiblingIndex();
                }
                else
                {
                    rect = GetComponent<RectTransform>();
                    originalSize = rect.sizeDelta;
                    layoutRoot = (RectTransform)layoutGroup.transform;
                }
            }
            else
            {
                if (gridLayoutGroup == true)
                {
                    layoutGroup = transform.parent.GetComponent<GridLayoutGroup>();
                    if (layoutGroup == null)
                    {
                        gridLayoutGroup = false;
                    }
                    else
                    {
                        originalChildIndex = transform.GetSiblingIndex();
                    }
                }
            }

            if (group == null)
            {
                group = GetComponent<CanvasGroup>();
            }

            if (group.alpha == 0)
            {
                RefreshLayout(false);
            }
            else
            {
                RefreshLayout(true);
            }
        }

        public void Toggle()
        {
            if (group.alpha == 0)
            {
                ToggleGroup(true);
            }
            else
            {
                ToggleGroup(false);
            }
        }

        public void ToggleGroup(bool toggle)
        {
            if (toggle == true)
            {
                group.alpha = 1;
                group.interactable = true;
                group.blocksRaycasts = true;

                on.Invoke();
            }
            else
            {
                group.alpha = 0;
                group.interactable = false;
                group.blocksRaycasts = false;

                off.Invoke();
            }

            toggled.Invoke();

            if (subGroup != null)
            {
                subGroup.ToggleGroup(toggle);
            }

            RefreshLayout(toggle);
        }

        private void RefreshLayout(bool toggle)
        {
            if (layoutGroup != null)
            {
                if (gridLayoutGroup == true)
                {
                    if (toggle == true)
                    {
                        if (resetGridLayoutSiblingIndex == true)
                        {
                            transform.SetSiblingIndex(originalChildIndex);
                        }
                    }
                    else
                    {
                        transform.SetSiblingIndex(transform.parent.childCount);
                    }
                }
                else
                {
                    if (toggle == true)
                    {
                        rect.sizeDelta = originalSize;
                    }
                    else
                    {
                        rect.sizeDelta = Vector2.zero;
                    }

                    LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
                }
            }
        }
    }
}