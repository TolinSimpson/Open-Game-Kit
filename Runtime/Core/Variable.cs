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
    public abstract class Variable
    {
        public string name = "New Variable";

        public abstract void Initialize();
        public abstract void Reset();
    }

    [SRName("String")]
    public class StringVariable : Variable
    {
        private string initalValue = string.Empty;
        public string value = string.Empty;

        public override void Initialize() { initalValue = value; }
        public override void Reset() { value = initalValue; }
    }

    [SRName("Integer")]
    public class IntVariable : Variable
    {
        private int initalValue = 0;
        public int value = 0;

        public override void Initialize() { initalValue = value; }
        public override void Reset() { value = initalValue; }
    }

    [SRName("Float")]
    public class FloatVariable : Variable
    {
        private float initalValue = 0f;
        public float value = 0f;

        public override void Initialize() { initalValue = value; }
        public override void Reset() { value = initalValue; }
    }

    [SRName("Double")]
    public class DoubleVariable : Variable
    {
        private double initalValue = 0d;
        public double value = 0d;

        public override void Initialize() { initalValue = value; }
        public override void Reset() { value = initalValue; }
    }

    [SRName("Vector2")]
    public class Vector2Variable : Variable
    {
        private Vector2 initalValue = Vector2.zero;
        public Vector2 value = Vector2.zero;

        public override void Initialize() { initalValue = value; }
        public override void Reset() { value = initalValue; }
    }

    [SRName("Vector3")]
    public class Vector3Variable : Variable
    {
        private Vector3 initalValue = Vector3.zero;
        public Vector3 value = Vector3.zero;

        public override void Initialize() { initalValue = value; }
        public override void Reset() { value = initalValue; }
    }

    [SRName("Vector4")]
    public class Vector4Variable : Variable
    {
        private Vector4 initalValue = Vector4.zero;
        public Vector4 value = Vector4.zero;

        public override void Initialize() { initalValue = value; }
        public override void Reset() { value = initalValue; }
    }

    [SRName("Quaternion")]
    public class QuaternionVariable : Variable
    {
        private Quaternion initalValue = Quaternion.identity;
        public Quaternion value = Quaternion.identity;

        public override void Initialize() { initalValue = value; }
        public override void Reset() { value = initalValue; }
    }
}