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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OGK
{
    [Serializable]
    public class ActionSequence
    {
        #region Properties:

        [HideInInspector]
        public bool enabled = true;

        [HideInInspector]
        public bool isPlaying = false;

        [SerializeReference]
        [SRPicker(typeof(ActionModule))]
        public List<ActionModule> actions = new List<ActionModule>();

        private bool playAsCoroutine = false;

        private bool stopSequence = false;

        [Min(-1)]
        private int heldActionIndex = -1;

        [Min(0)]
        private float delay = 0;

        private int delaySwitch = -1;

        #endregion

        public IEnumerator PlayAsCoroutine()
        {
            if (enabled == true && isPlaying == false && actions.Count > 0)
            {
                stopSequence = false;
                isPlaying = true;
                for (int i = 0; i < actions.Count; i++)
                {
                    if(actions[i] != null)
                    {
                        if (delaySwitch > -1)
                        {
                            i = delaySwitch;
                            delaySwitch = -1;
                        }

                        if (heldActionIndex > -1)
                        {
                            i = heldActionIndex;
                        }

                        delay = actions[i].delay;
                        if (delay > 0)
                        {
                            //Get or Set a WaitForSeconds delay from the cache then run it:
                            if (AppManager.Singleton.delays.ContainsKey(delay))
                            {
                                yield return AppManager.Singleton.delays[delay];
                            }
                            else
                            {
                                AppManager.Singleton.delays.Add(actions[i].delay, new WaitForSeconds(Mathf.Max(0, delay)));
                                yield return AppManager.Singleton.delays[delay];
                            }
                        }

                        switch (actions[i].Invoke())
                        {
                            case ActionModule.ActionEvent.Continue: break;
                            case ActionModule.ActionEvent.Stop: stopSequence = true; break;
                            case ActionModule.ActionEvent.Hold: heldActionIndex = i; stopSequence = true; break;
                            case ActionModule.ActionEvent.Release: heldActionIndex = -1; break;
                            case ActionModule.ActionEvent.GoBack: if (i > 0) { i--; } break;
                            case ActionModule.ActionEvent.SkipNext: if (i < actions.Count + 1) { i += 2; } break;
                            case ActionModule.ActionEvent.SkipToEnd: i = actions.Count - 1; break;
                            case ActionModule.ActionEvent.Restart: i = 0; break;
                            case ActionModule.ActionEvent.ToRandom: i = UnityEngine.Random.Range(0, actions.Count); break;
                            case ActionModule.ActionEvent.Error: Debug.LogWarningFormat("Action {0} resulted in an error. Continuing...", i); break;
                        }
                        if (stopSequence == true) { isPlaying = false; break; }
                    }
                }
                isPlaying = false;
            }
        }

        public void Play()
        {
            if (playAsCoroutine == true)
            {
                AppManager.Singleton.StartCoroutine(PlayAsCoroutine());
            }
            else
            {
                if (enabled == true && isPlaying == false && actions.Count > 0)
                {
                    stopSequence = false;
                    isPlaying = true;
                    for (int i = 0; i < actions.Count; i++)
                    {
                        if(actions[i] != null)
                        {
                            if (heldActionIndex > -1)
                            {
                                i = heldActionIndex;
                            }

                            //If there is a delay then switch to a coroutine instance.
                            delay = actions[i].delay;
                            if (delay > 0)
                            {
                                delaySwitch = i;
                                AppManager.Singleton.StartCoroutine(PlayAsCoroutine());
                                playAsCoroutine = true; //Note: this will always be a coroutine instance even if all delays are removed.
                                break;
                            }

                            switch (actions[i].Invoke())
                            {
                                case ActionModule.ActionEvent.Continue: break;
                                case ActionModule.ActionEvent.Stop: stopSequence = true; break;
                                case ActionModule.ActionEvent.Hold: heldActionIndex = i; stopSequence = true; break;
                                case ActionModule.ActionEvent.Release: heldActionIndex = -1; break;
                                case ActionModule.ActionEvent.GoBack: if (i > 0) { i--; } break;
                                case ActionModule.ActionEvent.SkipNext: if (i < actions.Count + 1) { i += 2; } break;
                                case ActionModule.ActionEvent.SkipToEnd: i = actions.Count - 1; break;
                                case ActionModule.ActionEvent.Restart: i = 0; break;
                                case ActionModule.ActionEvent.ToRandom: i = UnityEngine.Random.Range(0, actions.Count); break;
                                case ActionModule.ActionEvent.Error: Debug.LogWarningFormat("Action {0} resulted in an error. Continuing...", i); break;
                            }
                            if (stopSequence == true) { isPlaying = false; break; }
                        }
                    }
                    isPlaying = false;
                }
            }
        }
    }
}