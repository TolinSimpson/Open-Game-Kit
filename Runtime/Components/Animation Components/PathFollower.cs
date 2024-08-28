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
    [AddComponentMenu("Open Game Kit/Animation/Path Follower")]
    public class PathFollower : MonoBehaviour
    {
        #region Properties:

        [Tooltip("The path to follow.")]
        public Path path;

        [Tooltip("If enabled the SmartGameObject will translate along its path of splines (disabled for its own local space paths).")]
        public bool usePath = false;
        [Tooltip("If enabled a SmartGameObject will look at the next path segment while following a path.points.")]
        public bool lookAtPath = false;
        public AnimationCurve pathCurve = AnimationCurve.Linear(0, 1, 0.1f, 1);


        [Tooltip("The position to translate to during a translation tween. (world space)")]
        public Vector3 translationTarget;

        private int currentPathPoint = 0;
        /// <summary>
        /// Is the path being moved along in reverse? (wrap mode ping-pong)
        /// </summary>
        private bool pathReversed = false;

        /// <summary>
        /// The next position to be determined by a translation tween (useful for networking/physics).
        /// </summary>
        [HideInInspector]
        public Vector3 nextPosition { get; private set; }

        /// <summary>
        /// The current progress of <see cref="translationCurve"/>'s evaluation.
        /// </summary>
        public float translationTime { get; private set; } = 0;

        public bool translating { get; private set; } = false;

        public bool translationPaused { get; private set; } = false;

        private Transform myTransform;

        #endregion

        private void Awake()
        {
            myTransform = transform;
        }

        // Update is called once per frame
        void Update()
        {
            FollowPath();
        }

        public void RefreshPath()
        {
            path.CalculatePath();
            if (currentPathPoint > path.points.Count)
            {
                currentPathPoint = path.points.Count - 1;
            }
        }

        public void StartPathTranslation()
        {
            if (path.splines.Count > 0 && path.useLocalSpace == false)
            {
                if (path.points.Count > 0)
                {
                    usePath = true;
                    translationTime = 0;
                    currentPathPoint = 0;
                    translationTarget = path.points[0];
                    translating = true;
                }
                else
                {
                    Debug.LogWarning("|Path Follower|: Tried to start a path translation but a path has not been generated, try calling RefreshPath().", gameObject);
                }
            }
            else
            {
                Debug.LogWarning("|Path Follower|: Tried to start a path translation but no splines have been defined to create a path from.", gameObject);
            }
        }

        public void StopPathTranslation()
        {
            translating = false;
            translationPaused = false;
            translationTime = 0;
            currentPathPoint = 0;
            translationTarget = path.points[0];
            pathReversed = false;
        }

        private void FollowPath()
        {
            if(translating == true && translationPaused == false)
            {
                // Translate along path:
                if (translationTime < pathCurve.keys[pathCurve.length - 1].time)
                {
                    translationTime += Time.deltaTime;
                    nextPosition = Vector3.Lerp(myTransform.position, path.points[currentPathPoint], pathCurve.Evaluate(translationTime));
                    if (lookAtPath == true)
                    {
                        myTransform.LookAt(path.points[currentPathPoint]);
                    }
                    myTransform.position = nextPosition;
                }
                else
                {
                    if (pathReversed == false)
                    {
                        if (currentPathPoint == path.points.Count - 1)
                        {
                            switch (pathCurve.postWrapMode)
                            {
                                case WrapMode.PingPong:

                                    pathReversed = true;

                                    break;

                                case WrapMode.Clamp:

                                    StopPathTranslation();

                                    break;

                                case WrapMode.Loop:

                                    currentPathPoint = (currentPathPoint + 1) % path.points.Count;

                                    break;
                            }
                        }
                        else
                        {
                            currentPathPoint = (currentPathPoint + 1) % path.points.Count;
                        }
                    }
                    else
                    {
                        if (currentPathPoint == 0)
                        {
                            pathReversed = false;
                        }
                        else
                        {
                            currentPathPoint--;
                        }
                    }
                    translationTime = 0;
                }
            }      
        }
    }
}