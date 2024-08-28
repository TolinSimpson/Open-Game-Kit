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

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OGK
{
    /// <summary>
    /// Toggles between a group of UI buttons, useful for tabular menus.
    /// </summary>
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Open Game Kit/UI/Button Toggle")]
    public class UIButtonToggle : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        #region Properties:

        public bool toggleState = false;
        [Tooltip("If true the button will be able to untoggle itself when clicked while toggled on.")]
        public bool allowSelfUntoggle = false;
        [Tooltip("If true the button will be unclickable.")]
        public bool disabled = false;
        public Image image;

        [Tooltip("Other UIButtonToggles that if enabled when this one is toggled on will be toggled off.")]
        public List<UIButtonToggle> toggleGroup = new List<UIButtonToggle>();

        [Tooltip("Autofills the toggleGroup when playmode is entered by getting UIButtonToggles in this object's parent's child heirarchy.")]
        public bool autofillFromParent = false;

        [Tooltip("When toggled these other toggles will be set to match.")]
        public List<UIButtonToggle> linkedToggles = new List<UIButtonToggle>();

        public Color onColor = Color.gray;
        public Color offColor = Color.white;
        public Color hoverColor = Color.gray * 1.5f;
        public Color disabledColor = new Color(0.1f, 0.1f, 0.1f, 1f);

        public UnityEvent on;
        public UnityEvent off;
        public UnityEvent toggled;
        public UnityEvent hover;

        #endregion

        #region Initialization & Updates:

        private void Awake()
        {
            if (autofillFromParent == true)
            {
                toggleGroup = transform.parent.GetComponentsInChildren<UIButtonToggle>().ToList();
            }

            if (toggleGroup.Count > 0)
            {
                for (int i = toggleGroup.Count - 1; i > 0; i--)
                {
                    if (toggleGroup != null)
                    {
                        if (toggleGroup[i] == this)
                        {
                            toggleGroup.RemoveAt(i);
                        }
                        else if (toggleGroup[i].toggleState == true)
                        {
                            toggleGroup[i].toggleState = false;
                            toggleGroup[i].ToggleColor();
                        }
                    }
                    else
                    {
                        toggleGroup.RemoveAt(i);
                    }
                }
            }
        }

        void Start()
        {
            if (disabled == false && toggleState == true)
            {
                InvokeToggleEvents();
            }
            ToggleColor();
        }

        #endregion

        #region Methods:

        public void Toggle()
        {
            ToggleGroup();

            toggleState = !toggleState;
            ToggleColor();
            InvokeToggleEvents();

            SyncLinkedToggles();
        }

        public void Toggle(bool toggle)
        {
            toggleState = toggle;

            ToggleGroup();

            ToggleColor();
            InvokeToggleEvents();

            SyncLinkedToggles();
        }

        private void ToggleGroup()
        {
            foreach (UIButtonToggle btnToggle in toggleGroup)
            {
                if (btnToggle != this && btnToggle.toggleState == true)
                {
                    btnToggle.Toggle();
                }
            }
        }

        public void SyncLinkedToggles()
        {
            foreach (UIButtonToggle btnToggle in linkedToggles)
            {
                if (btnToggle != null)
                {
                    btnToggle.SyncState(this);
                }
            }
        }

        public void SyncState(UIButtonToggle other)
        {
            toggleState = other.toggleState;
            foreach (UIButtonToggle btnToggle in toggleGroup)
            {
                if (btnToggle != this && btnToggle.toggleState == true)
                {
                    btnToggle.Toggle();
                }
            }
            ToggleColor();
            InvokeToggleEvents();
        }

        private void InvokeToggleEvents()
        {
            if (toggleState == true)
            {
                on.Invoke();
            }
            else
            {
                off.Invoke();
            }

            toggled.Invoke();
        }

        public void ToggleColor()
        {
            if (image != null)
            {
                if (toggleState == true)
                {
                    image.color = onColor;
                }
                else
                {
                    image.color = offColor;
                }

                if (disabled == true)
                {
                    image.color = disabledColor;
                }
            }
            else
            {
                image = GetComponent<Image>();
                ToggleColor();
            }

        }

        public void DisableButton(bool toggle)
        {
            disabled = toggle;
            ToggleColor();
        }

        public void DisableAllButtons(bool toggle)
        {
            foreach (UIButtonToggle btn in toggleGroup)
            {
                btn.DisableButton(toggle);
                btn.ToggleColor();
            }
            DisableButton(toggle);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (disabled == false)
            {
                if (allowSelfUntoggle == true)
                {
                    Toggle();
                }
                else
                {
                    if (toggleState == false)
                    {
                        Toggle();
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (disabled == false)
            {
                image.color = hoverColor;
                hover.Invoke();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToggleColor();
        }

        #endregion
    }
}