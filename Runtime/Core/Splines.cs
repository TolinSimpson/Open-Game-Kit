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
    public struct Spline
    {
        public SplineTypes splineType;
        public Vector3 startPoint;
        public Vector3 controlPoint1;
        public Vector3 controlPoint2;
        public Vector3 endPoint;
        [Tooltip("If enabled the path will have its segments equally spaced. This may reduce acceleration around corners.")]
        public bool equalize;
        [Min(2), Tooltip("Determines the amount of segments of the equalized path.")]
        public int equalizedSegments;
    }

    public static class Splines
    {
        public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }

        public static Vector3 CalculateHermitePoint(float t, Vector3 p0, Vector3 p1, Vector3 m0, Vector3 m1)
        {
            float tt = t * t;
            float ttt = tt * t;
            float h1 = 2 * ttt - 3 * tt + 1;
            float h2 = -2 * ttt + 3 * tt;
            float h3 = ttt - 2 * tt + t;
            float h4 = ttt - tt;

            return h1 * p0 + h2 * p1 + h3 * m0 + h4 * m1;
        }

        public static Vector3 CalculateCatmullRomPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float tt = t * t;
            float ttt = tt * t;

            return 0.5f * (2 * p1 + (-p0 + p2) * t + (2 * p0 - 5 * p1 + 4 * p2 - p3) * tt + (-p0 + 3 * p1 - 3 * p2 + p3) * ttt);
        }

        public static Vector3 CalculateBSplinePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float tt = t * t;
            float ttt = tt * t;

            return (1 / 6f) * (p0 * (-ttt + 3 * tt - 3 * t + 1) + p1 * (3 * ttt - 6 * tt + 4) + p2 * (-3 * ttt + 3 * tt + 3 * t + 1) + p3 * (ttt));
        }

        public static Vector3 CalculateLinearPoint(float t, Vector3 p0, Vector3 p1)
        {
            return Vector3.Lerp(p0, p1, t);
        }

        /// <summary>
        /// Calculates arch length parameterization.
        /// </summary>
        /// <param name="splinePoints">Must have at least 2 points.</param>
        /// <param name="numPoints">Must be at least 2.</param>
        /// <returns>Evenly spaced points.</returns>
        public static List<Vector3> GetEvenlySpacedPoints(List<Vector3> splinePoints, int numPoints)
        {
            float[] cumulativeLengths = new float[splinePoints.Count];
            cumulativeLengths[0] = 0f;
            float totalLength = 0f;

            for (int i = 1; i < splinePoints.Count; i++)
            {
                float segmentLength = (splinePoints[i] - splinePoints[i - 1]).magnitude;
                cumulativeLengths[i] = cumulativeLengths[i - 1] + segmentLength;
                totalLength += segmentLength;
            }

            List<Vector3> evenlySpacedPoints = new List<Vector3>();
            float step = totalLength / (numPoints - 1);
            float currentLength = 0f;
            int currentIndex = 0;

            for (int i = 0; i < numPoints - 1; i++)
            {
                while (currentIndex < splinePoints.Count - 2 && currentLength > cumulativeLengths[currentIndex + 1])
                {
                    currentIndex++;
                }

                evenlySpacedPoints.Add(Vector3.Lerp(splinePoints[currentIndex], splinePoints[currentIndex + 1], (currentIndex == splinePoints.Count - 2) ? 1f : (currentLength - cumulativeLengths[currentIndex]) / (cumulativeLengths[currentIndex + 1] - cumulativeLengths[currentIndex])));
                currentLength += step;
            }

            evenlySpacedPoints.Add(splinePoints[splinePoints.Count - 1]);
            return evenlySpacedPoints;
        }

        /// <summary>
        /// Adds a spline to a path represented as a list of points.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="tempPath"></param>
        /// <param name="tempVector"></param>
        /// <param name="spline"></param>
        /// <param name="localPosition"></param>
        /// <param name="useLocalSpace"></param>
        public static void CreateSplinePath(ref List<Vector3> path, ref List<Vector3> tempPath, ref Vector3 tempVector, Spline spline, Vector3 localPosition, bool useLocalSpace = false)
        {
            tempPath.Clear();
            for (float t = 0f; t <= 1.001f; t += 0.05f)
            {
                switch (spline.splineType)
                {
                    case SplineTypes.Bezier:

                        tempVector = CalculateBezierPoint(t, spline.startPoint, spline.controlPoint2, spline.controlPoint1, spline.endPoint);

                        break;

                    case SplineTypes.Hermite:

                        tempVector = CalculateHermitePoint(t, spline.startPoint, spline.endPoint, spline.controlPoint1, spline.controlPoint2);

                        break;

                    case SplineTypes.CatmullRom:

                        tempVector = CalculateCatmullRomPoint(t, spline.controlPoint1, spline.startPoint, spline.endPoint, spline.controlPoint2);

                        break;

                    case SplineTypes.BSpline:

                        tempVector = CalculateBSplinePoint(t, spline.controlPoint1, spline.controlPoint2, spline.startPoint, spline.endPoint);

                        break;

                    case SplineTypes.Linear:

                        tempVector = CalculateLinearPoint(t, spline.startPoint, spline.endPoint);

                        break;

                    case SplineTypes.Raw:

                        if (useLocalSpace == true)
                        {
                            tempVector = localPosition;
                            tempPath.Add(spline.startPoint + tempVector);
                            tempPath.Add(spline.controlPoint1 + tempVector);
                            tempPath.Add(spline.controlPoint2 + tempVector);
                            tempVector = spline.endPoint + localPosition;
                        }
                        else
                        {
                            tempPath.Add(spline.startPoint);
                            tempPath.Add(spline.controlPoint1);
                            tempPath.Add(spline.controlPoint2);
                            tempVector = spline.endPoint;
                        }

                        break;
                }

                if (useLocalSpace == true)
                {
                    tempVector += localPosition;
                }
                tempPath.Add(tempVector);
            }

            if (spline.equalize == true)
            {
                if (spline.equalizedSegments == 0)
                {
                    spline.equalizedSegments = tempPath.Count * 2;
                }

                if (tempPath.Count >= 2 && spline.equalizedSegments > 1)
                {
                    tempPath = GetEvenlySpacedPoints(tempPath, spline.equalizedSegments);
                }
                else
                {
                    Debug.LogError("|CreateSplinePath|: Failed to equalize path, path points and resolution should be equal to or greater than 2.");
                }
            }

            foreach (Vector3 point in tempPath)
            {
                path.Add(point);
            }
        }
    }
}