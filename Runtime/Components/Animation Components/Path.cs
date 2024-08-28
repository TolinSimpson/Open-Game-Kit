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
    [AddComponentMenu("Open Game Kit/Animation/Path")]
    public class Path : MonoBehaviour
    {
        public bool useLocalSpace = false;

        public List<Spline> splines = new List<Spline>();
      
        [HideInInspector]
        public List<Vector3> points = new List<Vector3>();

        private Transform myTransform;
        private List<Vector3> tempPath = new List<Vector3>();
        private Vector3 tempVector;
        private Vector3 previousPosition;

        private void Awake()
        {
            myTransform = transform;
            previousPosition = myTransform.position;
            CalculatePath();
        }

        private void Update()
        {
            if(useLocalSpace == true && myTransform.position != previousPosition)
            {
                CalculatePath();
                previousPosition = myTransform.position;
            }
        }

        private void OnValidate()
        {
            if(myTransform == null)
            {
                myTransform = transform;
                previousPosition = myTransform.position;
            }
            if (useLocalSpace == true && myTransform.position != previousPosition)
            {
                previousPosition = myTransform.position;
            }
            CalculatePath();
        }

        private void OnDrawGizmosSelected()
        {
            // Draw Splines:
            if (splines.Count > 0)
            {
                points.Clear();

                Gizmos.color = Color.cyan;

                foreach (Spline spline in splines)
                {
                    if (useLocalSpace == true)
                    {
                        Gizmos.DrawSphere(myTransform.localPosition + spline.startPoint, 0.15f);
                        Gizmos.DrawWireSphere(myTransform.localPosition + spline.controlPoint1, 0.15f);
                        Gizmos.DrawWireSphere(myTransform.localPosition + spline.controlPoint2, 0.15f);
                        Gizmos.DrawSphere(myTransform.localPosition + spline.endPoint, 0.15f);
                    }
                    else
                    {
                        Gizmos.DrawSphere(spline.startPoint, 0.15f);
                        Gizmos.DrawWireSphere(spline.controlPoint1, 0.15f);
                        Gizmos.DrawWireSphere(spline.controlPoint2, 0.15f);
                        Gizmos.DrawSphere(spline.endPoint, 0.15f);
                    }

                    Splines.CreateSplinePath(ref points, ref tempPath, ref tempVector, spline, myTransform.localPosition, useLocalSpace);
                }

                for (int i = 1; i < points.Count; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(points[i - 1], points[i]);
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(points[i - 1] + Vector3.left, points[i] + Vector3.left);
                    Gizmos.DrawLine(points[i - 1] + Vector3.left, points[i - 1] + Vector3.right);
                    Gizmos.DrawLine(points[i - 1] + Vector3.right, points[i] + Vector3.right);
                    if (i == points.Count - 1)
                    {
                        Gizmos.DrawLine(points[i] + Vector3.left, points[i] + Vector3.right);
                    }
                }
            }
        }

        public void CalculatePath()
        {
            if (splines.Count > 0 && points.Count == 0)
            {
                points.Clear();
                foreach (Spline spline in splines)
                {
                    Splines.CreateSplinePath(ref points, ref tempPath, ref tempVector, spline, myTransform.localPosition, useLocalSpace);
                }
            }
        }
    }
}