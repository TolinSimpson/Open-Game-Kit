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
    [SRName("Variable Tracker/Compare/Int")]
    public class CompareIntVarAction : ActionModule
    {
        [SerializeField] private VariableTracker tracker;
        [SerializeField] private string variable1;
        [SerializeField] private NumericalComparisons comparison = NumericalComparisons.EqualTo;
        [SerializeField] private string variable2;
        [SerializeField] private ActionEvent onTrue, onFalse;
        public override ActionEvent Invoke() 
        {
            if (tracker != null)
            {
                if(LogicOperations.NumericalComparison(tracker.GetInteger(variable1), comparison, tracker.GetInteger(variable2)) == true) { return onTrue; } else { return onFalse; }
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Variable Tracker/Set/Int")]
    public class SetIntVarAction : ActionModule
    {
        [SerializeField] private VariableTracker tracker;
        [SerializeField] private string variable1;
        [SerializeField] private NumericalOperators operaton;
        [SerializeField] private int value;
        public override ActionEvent Invoke()
        {
            if (tracker != null)
            {
                tracker.SetInteger(variable1, operaton, value);
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }
}