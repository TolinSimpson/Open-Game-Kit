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
    /// <summary>
    /// Tracks variables stored in a hash map.
    /// </summary>
    [DefaultExecutionOrder(-22)]
    [AddComponentMenu("Open Game Kit/Utility/Variable Tracker")]
    public class VariableTracker : MonoBehaviour
    {
        [SerializeReference]
        [SRPicker(typeof(Variable))]
        [NonReorderable]
        public List<Variable> variables = new List<Variable>();

        public Dictionary<int, int> variableLookup = new Dictionary<int, int>();

        private int hash;
        private StringVariable strVar;
        private IntVariable intVar;
        private FloatVariable floatVar;
        private DoubleVariable doubleVar;
        private Vector2Variable v2Var;
        private Vector3Variable v3Var;
        private Vector4Variable v4Var;
        private QuaternionVariable quaternionVar;

        private void Awake()
        {
            foreach (Variable variable in variables)
            {
                variable.Initialize();
            }
            RefreshLookupMap();
        }

        private void OnValidate()
        {
            RefreshLookupMap();
        }

        /// <summary>
        /// Call if changes to the variables list was made.
        /// </summary>
        public void RefreshLookupMap()
        {
            if(variables.Count > 0)
            {
                variableLookup.Clear();
                for (int i = 0; i < variables.Count; i++)
                {
                    if(variables[i] != null)
                    {
                        hash = variables[i].name.GetHashCode();
                        if (!variableLookup.ContainsKey(hash))
                        {
                            variableLookup.Add(hash, i);
                        }
                        else
                        {
                            Debug.LogWarningFormat("Duplicate variable names detected in Variable Tracker; ignoring subsequent variables named: {0}", variables[i].name, gameObject);
                        }
                    }
                }
            }
        }

        public Variable GetVariable(string variableName)
        {
            return variables[variableLookup[variableName.GetHashCode()]];
        }

        public void ResetVariable(string variableName)
        {
            hash = variableLookup[variableName.GetHashCode()];
            if (variables[hash] != null)
            {
                variables[hash].Reset();
            }
        }

        public void ResetAllVariables()
        {
            foreach (Variable variable in variables)
            {
                variable.Reset();
            }
        }

        public int GetInteger(string variableName)
        {
            intVar = (IntVariable)variables[variableLookup[variableName.GetHashCode()]];
            if (intVar != null)
            {
                return intVar.value;
            }
            else
            {
                Debug.LogWarningFormat("Failed to get variable of name: {0}, returning 0.", variableName, gameObject);
                return 0;
            }
        }

        public void SetInteger(string variableName, int value)
        {
            intVar = (IntVariable)variables[variableLookup[variableName.GetHashCode()]];
            if (intVar != null) { intVar.value = value; }
        }

        public void SetInteger(string variableName, NumericalOperators operation, int value)
        {
            intVar = (IntVariable)variables[variableLookup[variableName.GetHashCode()]];
            if (intVar != null) { intVar.value = LogicOperations.PerformNumericalOperation(intVar.value, operation, value); }
        }
    }
}