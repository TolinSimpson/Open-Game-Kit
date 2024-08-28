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
using TMPro;

namespace OGK
{
    [SRName("UI/Set Element Size")]
    public class SetUISizeAction : ActionModule
    {
        [SerializeField] private RectTransform rect;
        [Tooltip("If the Rect is in a LayoutGroup assign it here to be updated.")]
        [SerializeField] private LayoutGroup layoutGroup;
        [SerializeField] private Vector2 size;

        private RectTransform layoutRoot;

        public override ActionEvent Invoke()
        {
            if (rect != null)
            {
                rect.sizeDelta = size;

                // Ensure the layout group scales to fit:
                if (layoutGroup != null)
                {
                    if (layoutRoot == null)
                    {
                        layoutRoot = (RectTransform)layoutGroup.transform;
                    }

                    if (layoutRoot.gameObject.activeSelf == true)
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRoot);
                    }
                }

                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("UI/Toggle Canvas Group")]
    public class ToggleCanvasGroupAction : ActionModule
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private bool toggle;

        public override ActionEvent Invoke()
        {
            if (group != null)
            {
                if (toggle == true)
                {
                    group.alpha = 1;
                    group.interactable = true;
                    group.blocksRaycasts = true;
                }
                else
                {
                    group.alpha = 0;
                    group.interactable = false;
                    group.blocksRaycasts = false;
                }
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("UI/Legacy/Set Text")]
    public class SetTextAction : ActionModule
    {
        [SerializeField] private Text textUI;
        [SerializeField] private string text;
        public override ActionEvent Invoke() { if (textUI != null) { textUI.text = text; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("UI/Set TMP Text")]
    public class SetTMPTextAction : ActionModule
    {
        [SerializeField] private TMP_Text textUI;
        [SerializeField] private string text;
        public override ActionEvent Invoke() { if (textUI != null) { textUI.text = text; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("UI/Fade")]
    public class UIFadeInAction : ActionModule
    {
        [Message("Play from Update loop.")]
        [SerializeField] private CanvasGroup group;
        [SerializeField] [Tooltip("true = in | false = out")] private bool fadeIn = true;
        [SerializeField] private float speed = 0.1f;
        [SerializeField] private ActionEvent onPlaying = ActionEvent.Hold;
        [SerializeField] private ActionEvent onComplete = ActionEvent.Release;
        public override ActionEvent Invoke() 
        { 
            if (group != null) 
            { 
                if(fadeIn == true)
                {
                    group.alpha += Time.deltaTime * speed;
                    if(group.alpha >= 1)
                    {
                        group.alpha = 1;
                        return onComplete;
                    }
                }
                else
                {
                    group.alpha -= Time.deltaTime * speed;
                    if(group.alpha <= 0)
                    {
                        group.alpha = 0;
                        return onComplete;
                    }
                }
                return onPlaying;
            } 
            else return ActionEvent.Error; 
        }
    }

    [SRName("UI/Time Display")]
    public class UITimeDisplayAction : ActionModule
    {
        [Message("Use in an Update Loop.")]
        [SerializeField] private TMP_Text textUI;
        [SerializeField] [Min(0)] private float time = 60;
        [SerializeField] private bool countDown = true;
        [SerializeField] private TimeFormats format = TimeFormats.MinuteSecond;

        [SerializeField] private ActionEvent onCounting, onEnd;

        private float originalTime = -1;

        public override ActionEvent Invoke() 
        {
            if(time != originalTime && originalTime == -1)
            {
                originalTime = time;
            }
          
            if (textUI != null)
            {

                if(countDown == true)
                {
                    if (time > 0)
                    {
                        time -= Time.deltaTime;
                    }
                    else
                    {
                        time = originalTime;
                        originalTime = -1;
                        FormatTimeText(originalTime, format);
                        return onEnd;
                    }                       
                }
                else
                {
                    if(time < originalTime)
                    {
                        time += Time.deltaTime;
                    }
                    else
                    {
                        time = 0;
                        originalTime = -1;
                        FormatTimeText(0, format);
                        return onEnd;
                    }         
                }

                FormatTimeText(time, format);

                return onCounting;
            } 
            else return ActionEvent.Error; 
        }

        private void FormatTimeText(float t, TimeFormats format)
        {
            switch(format)
            {
                case TimeFormats.HourMinuteSecond:

                    textUI.text = string.Format("{0:00}:{1:00}", Mathf.FloorToInt(time / 60), Mathf.FloorToInt(time % 60));

                    break;

                case TimeFormats.HourMinuteSecondMillisecond:

                    textUI.text = string.Format("{0:00}:{1:00}:{2:000}", Mathf.FloorToInt(time / 60), Mathf.FloorToInt(time % 60), (t % 1) * 1000);

                    break;

                case TimeFormats.MinuteSecond:

                    textUI.text = string.Format("{00}:{1:00}", Mathf.FloorToInt(time / 60), Mathf.FloorToInt(time % 60));

                    break;

                case TimeFormats.MinuteSecondMillisecond:

                    textUI.text = string.Format("{00}:{1:00}:{2:000}", Mathf.FloorToInt(time / 60), Mathf.FloorToInt(time % 60), (t % 1) * 1000);

                    break;

            }
        }
    }

    [SRName("UI/Set Canvas Resolution")]
    public class UISetResolutionAction : ActionModule
    {
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private Resolutions resolution = Resolutions._1920x1080;

        public override ActionEvent Invoke()
        {
            if(canvasScaler != null)
            {
                switch(resolution)
                {
                    case Resolutions._1920x1080: canvasScaler.referenceResolution = new Vector2(1920, 1080); break;
                    case Resolutions._1031x580: canvasScaler.referenceResolution = new Vector2(1031, 580); break;
                    case Resolutions._836x470: canvasScaler.referenceResolution = new Vector2(836, 470); break;
                    case Resolutions._640x360: canvasScaler.referenceResolution = new Vector2(640, 360); break;
                }
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("UI/Set App Version")]
    public class SetAppVersionAction : ActionModule
    {
        [SerializeField] private TMP_Text textUI;
        public override ActionEvent Invoke() { if (textUI != null) { textUI.text = Application.version; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("UI/Scale Tween")]
    public class UIScaleTweenAction : ActionModule
    {
        [Message("Play from Update loop.")]
        [SerializeField] private RectTransform rect;
        [SerializeField] private float scaleFactor = 2;
        [SerializeField, Min(0)] private float speed = 0.1f;
        [SerializeField] private ActionEvent onPlaying = ActionEvent.Hold;
        [SerializeField] private ActionEvent onComplete = ActionEvent.Release;
        private Vector2 originalSize = new Vector2(-9999, -9999);
        public override ActionEvent Invoke()
        {
            if (rect != null)
            {
                if (originalSize.x == -9999)
                {
                    originalSize = rect.sizeDelta;
                }

                rect.sizeDelta = Vector2.Lerp(rect.sizeDelta, originalSize * scaleFactor, Time.time * speed);
                if(rect.sizeDelta.x <= originalSize.x * scaleFactor)
                {
                    return onComplete;
                }

                return onPlaying;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("UI/Switch Image Color")]
    public class UISwitchColorAction : ActionModule
    {
        [Message("Each time called switches to the next color.")]
        [SerializeField] private Image image;
        [SerializeField] private Color[] colors;
        private int currentColor = 0;
        public override ActionEvent Invoke()
        {
            if (image != null)
            {
                image.color = colors[currentColor];
                currentColor++;
                if(currentColor == colors.Length)
                {
                    currentColor = 0;
                }
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("UI/Sync Slider to Blendshape")]
    public class UISliderBlendshapeSyncAction : ActionModule
    {
        [SerializeField] private Slider slider;
        [SerializeField] private SkinnedMeshRenderer skin;
        [SerializeField, Min(0)] private int blendshape = 0;
        public override ActionEvent Invoke()
        {
            if (slider != null && skin != null)
            {
                slider.value = skin.GetBlendShapeWeight(blendshape);
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }
}