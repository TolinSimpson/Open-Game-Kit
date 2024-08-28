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
    public enum UpdateModes { Update, LateUpdate, FixedUpdate, Manually }
    public enum ConditionTypes { Numerical = 0, Boolean = 1 }
    public enum NumericalComparisons { EqualTo, NotEqual, LessThan, GreaterThan, LessThanEqualTo, GreaterThanEqualTo }
    public enum BooleanComparisons { EqualTo, NotEqualTo }
    public enum NumericalOperators { SetEqualTo = 0, Power = 1, Multiply = 2, Divide = 3, Add = 4, Subtract = 5 }
    public enum SplineTypes { Bezier, Hermite, CatmullRom, BSpline, Linear, Raw }
    public enum RaycastTypes { None, Raycast, RaycastRow, CircleCast, GridCast, ScatterCast, FanCast, SphereCast, BoxCast, CapsuleCast, LineCast }
    public enum Directions { Forward, Back, Left, Right, Up, Down }
    public enum LogFormats { None, Message, Warning, Error }
    public enum TimeScales { time, deltaTime, unscaledTime, unscaledDeltaTime, fixedDeltaTime, fixedUnscaledTime, fixedUnscaledDeltaTime }
    public enum BillboardModes { Unconstrained, Direct, Horizonal, HorizontalDirect }
    public enum Resolutions { _1920x1080, _640x360, _836x470, _1031x580 }
    public enum SquareResolutions { _64x64, _128x128, _256x256, _512x512, _1024x1024, _2048x2048, _4096x4096 }
    public enum BuildTargets { Unknown, Standalone, WebGL, Android, IOS, PS4, XboxOne, NintendoSwitch, Stadia, TVOS, LinuxHeadlessSimulation }
    public enum TimeFormats { HourMinuteSecondMillisecond, MinuteSecondMillisecond, MinuteSecond, HourMinuteSecond }
}