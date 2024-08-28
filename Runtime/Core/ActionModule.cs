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
    public abstract class ActionModule
    {
        [HideInInspector] [Min(0)] public float delay = 0;
        public virtual ActionEvent Invoke() { return ActionEvent.Continue; }

        /// <summary>
        /// The action event determines what happens in an <see cref="ActionSequence"/> after the action is done invoking.
        /// </summary>
        public enum ActionEvent { Continue, Stop, SkipNext, GoBack, SkipToEnd, Restart, ToRandom, Hold, Release, Error }
    }

    [SRName("Action Sequence/Action Event")]
    public class ActionEventAction : ActionModule
    {
        [SerializeField] private ActionEvent actionEvent = ActionEvent.Continue;
        public override ActionEvent Invoke() { return actionEvent; }
    }

    [SRName("Action Sequence/Action Probability")]
    public class ProbablityAction : ActionModule
    {
        [SerializeField] [Range(0, 100)] private int chance = 100;
        [SerializeField] private ActionEvent onSuccess = ActionEvent.Continue, onFail = ActionEvent.Continue;
        public override ActionEvent Invoke() { if (LogicOperations.Probability(chance) == true) { return onSuccess; } else { return onFail; } }
    }

    [SRName("Action Sequence/Action Probability Range")]
    public class ProbablityRangeAction : ActionModule
    {
        [SerializeField] [Range(0, 100)] private int minChance = 0, maxChance = 100;
        [SerializeField] private ActionEvent onSuccess = ActionEvent.Continue, onFail = ActionEvent.Continue;
        public override ActionEvent Invoke() { if (LogicOperations.Probability(UnityEngine.Random.Range(minChance, maxChance)) == true) { return onSuccess; } else { return onFail; } }
    }
}