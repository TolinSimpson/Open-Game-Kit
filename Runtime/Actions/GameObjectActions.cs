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
    [SRName("GameObject/Destroy")]
    public class DestroyGameObjectAction : ActionModule
    {
        public GameObject go;
        public override ActionEvent Invoke() { if (go != null) { Object.Destroy(go); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("GameObject/Modify/Active State")]
    public class GameObjectSetActiveAction : ActionModule
    {
        public GameObject go;
        public bool enabled = true;
        public override ActionEvent Invoke() { if (go != null) { go.SetActive(enabled); return ActionEvent.Continue; } else return ActionEvent.Error;}
}

    [SRName("GameObject/Toggle Active State")]
    public class GameObjectToggleActiveAction : ActionModule
    {
        public GameObject go;
        public override ActionEvent Invoke() { if (go != null) { go.SetActive(!go.activeSelf); return ActionEvent.Continue; } else return ActionEvent.Error;}
    }

    [SRName("GameObject/Conditions/Has Tag?")]
    public class GameObjectHasTagAction : ActionModule
    {
        public GameObject go;
        public string tag;
        public ActionEvent ifTrue, ifFalse;
        public override ActionEvent Invoke() { if (go != null) { return go.CompareTag(tag) ? ifTrue : ifFalse; } else return ActionEvent.Error; }
    }

    [SRName("GameObject/Modify/Tag")]
    public class ModifyGameObjectTagAction : ActionModule
    {
        public GameObject go;
        public string newTag;
        public override ActionEvent Invoke() { if (go != null) { go.tag = newTag; return ActionEvent.Continue; } else return ActionEvent.Error;}
    }

    [SRName("GameObject/Modify/Name")]
    public class ModifyGameObjectNameAction : ActionModule
    {
        public GameObject go;
        public string newName;
        public override ActionEvent Invoke() { if (go != null) { go.name = newName; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("GameObject/Modify/Layer")]
    public class ModifyGameObjectLayerAction : ActionModule
    {
        public GameObject go;
        public int layer = 0;
        public override ActionEvent Invoke() { if (go != null) { go.layer = layer; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("GameObject/Conditions/Is Active?")]
    public class GameObjectIsActiveAction : ActionModule
    {
        public GameObject go;
        public ActionEvent ifTrue, ifFalse;
        public override ActionEvent Invoke() { if (go != null) { return go.activeSelf ? ifTrue : ifFalse; } else return ActionEvent.Error; }
    }

    [SRName("GameObject/Instantiate/At World Origin")]
    public class InstantiateGameObjectAtOriginAction : ActionModule
    {
        public GameObject go;
        public override ActionEvent Invoke() { if (go != null) { Object.Instantiate(go); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("GameObject/Instantiate/With Settings")]
    public class InstantiateGameObjectWithSettingsAction : ActionModule
    {
        public GameObject go;
        public Transform parent;
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public override ActionEvent Invoke() { if (go != null) { Object.Instantiate(go, position, rotation, parent); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("GameObject/Instantiate/At Transform")]
    public class InstantiateGameObjectAtTransformAction : ActionModule
    {
        public GameObject go;
        public Transform target;

        public override ActionEvent Invoke() { if (go != null && target != null) { Object.Instantiate(go, target.position, target.rotation); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("GameObject/Instantiate/Random At Transform")]
    public class InstantiateRandomGameObjectAtTransformAction : ActionModule
    {
        public List<GameObject> gameObjects = new List<GameObject>();
        public Transform target;

        public override ActionEvent Invoke() { if (target != null && gameObjects.Count > 0) { Object.Instantiate(gameObjects[Random.Range(0, gameObjects.Count)], target.position, target.rotation); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }
}