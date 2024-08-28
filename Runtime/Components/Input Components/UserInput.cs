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
using UnityEngine.Events;

namespace OGK
{
    [AddComponentMenu("Open Game Kit/Input/User Input")]
    public class UserInput : MonoBehaviour
    {
        [Serializable]
        public struct Keybind
        {
            public string name;
            public KeyCode key;
            public UnityEvent onKeyPressed;
        }

        public List<Keybind> keybinds = new List<Keybind>();

        [HideInInspector]
        public float horizontalInput;
        [HideInInspector]
        public float verticalInput;

        private Dictionary<int, Keybind> keybindCache = new Dictionary<int, Keybind>();
        private Keybind tempBind;
        private int tempKeyHash;

        public static UserInput Instance;

        private void Awake()
        {
            foreach (Keybind keybind in keybinds)
            {
                if (keybind.name != string.Empty)
                {
                    keybindCache.Add(keybind.name.GetHashCode(), keybind);
                }
            }

            LoadKeybinds();
        }

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        void Update()
        {
            foreach (Keybind keybind in keybindCache.Values)
            {
                if (Input.GetKeyDown(keybind.key))
                {
                    keybind.onKeyPressed.Invoke();
                }
            }

            horizontalInput = Input.GetAxis("Horizontal");

            verticalInput = Input.GetAxis("Vertical");
        }

        public void ChangeKeybind(string keyName, KeyCode key)
        {
            tempKeyHash = keyName.GetHashCode();
            if (keybindCache.ContainsKey(tempKeyHash) == true)
            {
                tempBind = keybindCache[tempKeyHash];
                tempBind.key = key;
                keybindCache[tempKeyHash] = tempBind;
            }
        }

        public string SaveKeybinds()
        {
            return JsonUtility.ToJson(keybindCache);// wont work on dictionaries
        }

        public void LoadKeybinds()
        {

        }
    }
}