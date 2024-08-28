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
using UnityEngine.SceneManagement;

namespace OGK
{
    [SRName("Scene/Load")]
    public class LoadSceneAction : ActionModule
    {
        [SerializeField] private string sceneName;
        [SerializeField] private LoadSceneMode mode = LoadSceneMode.Single;

        public override ActionEvent Invoke()
        {
            SceneManager.LoadScene(sceneName, mode);
            return ActionEvent.Stop;
        }
    }

    [SRName("Scene/Load Random")]
    public class LoadRandomSceneAction : ActionModule
    {
        [SerializeField] private string[] scenes;
        [SerializeField] private LoadSceneMode mode = LoadSceneMode.Single;

        public override ActionEvent Invoke()
        {
            SceneManager.LoadScene(Random.Range(0, scenes.Length), mode);
            return ActionEvent.Stop;
        }
    }

    [SRName("Scene/Conditions/Is Scene Active?")]
    public class IsSceneActiveAction : ActionModule
    {
        [SerializeField] private string sceneName;
        [SerializeField] private ActionEvent ifTrue, ifFalse;

        public override ActionEvent Invoke()
        {
            if(SceneManager.GetActiveScene().name == sceneName)
            {
                return ifTrue;
            }
            else
            {
                return ifFalse;
            }
        }
    }
}