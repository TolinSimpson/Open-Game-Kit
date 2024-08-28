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
using SerializeReferenceEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OGK
{
    [SRName("Events/Event")]
    public class EventAction : ActionModule
    {
        public UnityEvent uEvent;

        public override ActionEvent Invoke() { uEvent.Invoke(); return ActionEvent.Continue; }
    }

    [SRName("Events/Event<bool>")]
    public class EventBoolAction : ActionModule
    {
        public bool argument;
        public UnityEvent<bool> uEvent;

        public override ActionEvent Invoke() { uEvent.Invoke(argument); return ActionEvent.Continue; }
    }

    [SRName("Events/Event<string>")]
    public class EventStringAction : ActionModule
    {
        public string argument;
        public UnityEvent<string> uEvent;

        public override ActionEvent Invoke() { uEvent.Invoke(argument); return ActionEvent.Continue; }
    }

    [SRName("Events/Event<int>")]
    public class EventIntAction : ActionModule
    {
        public int argument;
        public UnityEvent<int> uEvent;

        public override ActionEvent Invoke() { uEvent.Invoke(argument); return ActionEvent.Continue; }
    }
    [SRName("Events/Event<float>")]
    public class EventFloatAction : ActionModule
    {
        public float argument;
        public UnityEvent<float> uEvent;

        public override ActionEvent Invoke() { uEvent.Invoke(argument); return ActionEvent.Continue; }
    }

    [SRName("Events/Event<Vector2>")]
    public class EventV2Action : ActionModule
    {
        public Vector2 argument;
        public UnityEvent<Vector2> uEvent;

        public override ActionEvent Invoke() { uEvent.Invoke(argument); return ActionEvent.Continue; }
    }

    [SRName("Events/Event<Vector3>")]
    public class EventV3Action : ActionModule
    {
        public Vector3 argument;
        public UnityEvent<Vector3> uEvent;

        public override ActionEvent Invoke() { uEvent.Invoke(argument); return ActionEvent.Continue; }
    }

    [SRName("Events/Event<Vector4>")]
    public class EventV4Action : ActionModule
    {
        public Vector4 argument;
        public UnityEvent<Vector4> uEvent;

        public override ActionEvent Invoke() { uEvent.Invoke(argument); return ActionEvent.Continue; }
    }
}