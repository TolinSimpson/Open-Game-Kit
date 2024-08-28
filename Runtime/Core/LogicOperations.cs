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
    public static class LogicOperations
    {
        public static bool NumericalComparison(int a, NumericalComparisons operation, int b)
        {
            switch(operation)
            {
                case NumericalComparisons.EqualTo: return a == b;
                case NumericalComparisons.GreaterThan: return a > b;
                case NumericalComparisons.GreaterThanEqualTo: return a >= b;
                case NumericalComparisons.LessThan: return a < b;
                case NumericalComparisons.LessThanEqualTo: return a <= b;
                case NumericalComparisons.NotEqual: return a != b;
                default: return false;
            }
        }
        public static bool NumericalComparison(float a, NumericalComparisons operation, float b)
        {
            switch (operation)
            {
                case NumericalComparisons.EqualTo: return a == b;
                case NumericalComparisons.GreaterThan: return a > b;
                case NumericalComparisons.GreaterThanEqualTo: return a >= b;
                case NumericalComparisons.LessThan: return a < b;
                case NumericalComparisons.LessThanEqualTo: return a <= b;
                case NumericalComparisons.NotEqual: return a != b;
                default: return false;
            }
        }

        public static bool NumericalComparison(double a, NumericalComparisons operation, double b)
        {
            switch (operation)
            {
                case NumericalComparisons.EqualTo: return a == b;
                case NumericalComparisons.GreaterThan: return a > b;
                case NumericalComparisons.GreaterThanEqualTo: return a >= b;
                case NumericalComparisons.LessThan: return a < b;
                case NumericalComparisons.LessThanEqualTo: return a <= b;
                case NumericalComparisons.NotEqual: return a != b;
                default: return false;
            }
        }

        public static bool BooleanComparison(bool a, BooleanComparisons operation, bool b)
        {
            switch (operation)
            {
                case BooleanComparisons.EqualTo: return a == b;
                case BooleanComparisons.NotEqualTo: return a != b;
                default: return false;
            }
        }

        public static void PerformBooleanOperation(ref bool leftSide, BooleanComparisons booleanComparison, bool rightSide)
        {
            switch (booleanComparison)
            {
                case BooleanComparisons.EqualTo: leftSide = rightSide; break;
                case BooleanComparisons.NotEqualTo: leftSide = !rightSide; break;
            }
        }

        public static void PerformNumericalOperation(ref float leftSide, NumericalOperators numericalOperator, float rightSide)
        {
            switch (numericalOperator)
            {
                case NumericalOperators.SetEqualTo:leftSide = rightSide; break;
                case NumericalOperators.Power:leftSide = Mathf.Pow(leftSide, rightSide);break;
                case NumericalOperators.Multiply:leftSide *= rightSide; break;
                case NumericalOperators.Divide: if (rightSide != 0) { leftSide /= rightSide; } else { Debug.LogWarning("Attempted to divide by zero. Skipping operation..."); } break;
                case NumericalOperators.Add: leftSide += rightSide; break;
                case NumericalOperators.Subtract: leftSide -= rightSide; break;
            }
        }

        public static float PerformNumericalOperation(float leftSide, NumericalOperators numericalOperator, float rightSide)
        {
            switch (numericalOperator)
            {
                case NumericalOperators.SetEqualTo: return rightSide;
                case NumericalOperators.Power: return Mathf.Pow(leftSide, rightSide);
                case NumericalOperators.Multiply: return leftSide *= rightSide;
                case NumericalOperators.Divide: if (rightSide != 0) { leftSide /= rightSide; } else { Debug.LogWarning("Attempted to divide by zero. Skipping operation..."); } return leftSide;
                case NumericalOperators.Add: return leftSide += rightSide;
                case NumericalOperators.Subtract: return leftSide -= rightSide;
            }
            return leftSide;
        }

        public static int PerformNumericalOperation(int leftSide, NumericalOperators numericalOperator, int rightSide)
        {
            switch (numericalOperator)
            {
                case NumericalOperators.SetEqualTo: return rightSide;
                case NumericalOperators.Power: return (int)Mathf.Pow(leftSide, rightSide);
                case NumericalOperators.Multiply: return leftSide *= rightSide;
                case NumericalOperators.Divide: if (rightSide != 0) { leftSide /= rightSide; } else { Debug.LogWarning("Attempted to divide by zero. Skipping operation..."); } return leftSide;
                case NumericalOperators.Add: return leftSide += rightSide;
                case NumericalOperators.Subtract: return leftSide -= rightSide;
            }
            return leftSide;
        }

        public static bool Probability(int chance)
        {
            if(chance == 0)
            {
                return false;
            }
            else
            {
                return Random.Range(0, 101) <= Mathf.Clamp(chance, 0, 100);
            }
        }
    }
}