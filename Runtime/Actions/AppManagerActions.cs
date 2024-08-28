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

namespace OGK
{
    [SRName("Application/Pause Sequencers")]
    public class PauseSequencersAction : ActionModule
    {
        public bool paused = false;
        public override ActionEvent Invoke() { AppManager.Singleton.PauseActionSequencers(paused); return ActionEvent.Continue; }
    }

    [SRName("Application/Coroutines/Start Coroutine")]
    public class StartCoroutineAction : ActionModule
    {
        public string coroutineName;
        public override ActionEvent Invoke() { AppManager.Singleton.StartCoroutine(coroutineName); return ActionEvent.Continue; }
    }

    [SRName("Application/Coroutines/Stop Coroutine")]
    public class StopCoroutineAction : ActionModule
    {
        public string coroutineName;
        public override ActionEvent Invoke() { AppManager.Singleton.StopCoroutine(coroutineName); return ActionEvent.Continue; }
    }

    [SRName("Application/Coroutines/Stop All Coroutines")]
    public class StopAllCoroutinesAction : ActionModule
    {
        public override ActionEvent Invoke() { AppManager.Singleton.StopAllCoroutines(); return ActionEvent.Continue; }
    }

    [SRName("Application/Toggle Cursor")]
    public class ToggleCursorAction : ActionModule
    {
        public bool isVisible = false;
        public override ActionEvent Invoke() 
        {
            if(isVisible == true)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            return ActionEvent.Continue; 
        }
    }

    [SRName("Pools/Activate Pool")]
    public class SpawnPooledObjectAction : ActionModule
    {
        [Header("Spawns a pooled prefab.")]
        public string prefabName;
        public override ActionEvent Invoke() { AppManager.Singleton.ActivatePooledObject(prefabName); return ActionEvent.Continue; }
    }

    [SRName("Pools/Spawn At Position")]
    public class SpawnPooledObjectAtPositionAction : ActionModule
    {
        [Header("Spawns a pooled prefab at a position & rotation.")]
        public string prefabName;
        public Vector3 position;
        public Quaternion rotation;
        public override ActionEvent Invoke() { AppManager.Singleton.SpawnAtPosition(position, rotation, prefabName); return ActionEvent.Continue; }
    }

    [SRName("Pools/Spawn At Transform")]
    public class SpawnPooledObjectAtTransformAction : ActionModule
    {
        [Header("Spawns a pooled prefab at the target transform.")]
        public string prefabName;
        public Transform target;
        public bool parent = false;
        public override ActionEvent Invoke() { AppManager.Singleton.SpawnAtTransform(target, prefabName, parent); return ActionEvent.Continue; }
    }

    [SRName("Pools/Spawn At Raycaster Hits")]
    public class SpawnPooledObjectAtRaycasterHitsAction : ActionModule
    {
        [Header("Spawns a pooled prefab at the target transform.")]
        public string prefabName;
        public Raycaster raycaster;
        public override ActionEvent Invoke()
        {
            if (raycaster != null)
            {
                foreach (RaycastHit hit in raycaster.hits)
                {
                    AppManager.Singleton.SpawnAtRaycastHit(hit, prefabName);
                }
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Application/Log Message")]
    public class DebugLogAction : ActionModule
    {
        public string message;
        public LogFormats format = LogFormats.Message;
        public override ActionEvent Invoke()
        {
            switch (format)
            {
                case LogFormats.None: break;
                case LogFormats.Message: Debug.Log(message); break;
                case LogFormats.Warning: Debug.LogWarning(message); break;
                case LogFormats.Error: Debug.LogError(message); break;
            }
            return ActionEvent.Continue;
        }
    }
}