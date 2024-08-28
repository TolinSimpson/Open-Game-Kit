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
    [AddComponentMenu("Open Game Kit/Animation/Transform Tweener")]
    public class TransformTweener : MonoBehaviour
    {
        #region Properties:

        /// <summary>
        /// Cache to get around the GetComponent call that's under the hood.
        /// </summary>
        [HideInInspector]
        private Transform myTransform;
        private Vector3 initialPos;
        private Quaternion initialRot;
        private Vector3 initialScale;
        public bool translating { get; private set; } = false;
        public bool translationPaused { get; private set; } = false;
        public AnimationCurve translationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Tooltip("The position to translate to during a translation tween. (world space)")]
        public Vector3 translationTarget;
        [Tooltip("The initial position to translate to when a translation tween starts. (world space)")]
        private Vector3 initialTranslationTarget;
        private Vector3 previousPosition;
        /// <summary>
        /// The next position to be determined by a translation tween (useful for networking/physics).
        /// </summary>
        [HideInInspector]
        public Vector3 nextPosition { get; private set; }

        public bool rotating { get; private set; } = false;
        public bool rotationPaused { get; private set; } = false;
        public AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Tooltip("The target to rotate to during a rotation tween.")]
        public Quaternion rotationTarget = Quaternion.identity;
        [Tooltip("The initial target to rotate to when a rotation tween starts.")]
        private Quaternion initialRotationTarget;
        private Quaternion previousRotation;
        /// <summary>
        /// The next rotation to be determined by a rotation tween (useful for networking/physics).
        /// </summary>
        [HideInInspector]
        public Quaternion nextRotation { get; private set; }

        public bool scaling { get; private set; } = false;
        public bool scalingPaused { get; private set; } = false;
        public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [Tooltip("The target to scale to during a scale tween.")]
        public Vector3 scaleTarget = Vector3.one;
        [Tooltip("The initial target to scale to when a scale tween starts.")]
        private Vector3 initialScaleTarget;
        private Vector3 previousScale;
        /// <summary>
        /// The next scale to be determined by a scale tween (useful for networking/physics).
        /// </summary>
        [HideInInspector]
        public Vector3 nextScale { get; private set; }
        [Tooltip("If enabled scale will be rounded so the scale snaps between increments.")]
        public bool snappyScale = false;
        /// <summary>
        /// The current progress of <see cref="translationCurve"/>'s evaluation.
        /// </summary>
        public float translationTime { get; private set; } = 0;
        /// <summary>
        /// The current progress of <see cref="rotationCurve"/>'s evaluation.
        /// </summary>
        public float rotationTime { get; private set; } = 0;
        /// <summary>
        /// The current progress of <see cref="scaleCurve"/>'s evaluation.
        /// </summary>
        public float scaleTime { get; private set; } = 0;

        #endregion

        #region Initialization & Updates:

        private void Awake()
        {
            myTransform = transform;
            initialPos = myTransform.position;
            initialRot = myTransform.rotation;
            initialScale = myTransform.localScale;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTween();
        }

        private void OnDrawGizmosSelected()
        {

        }

        #endregion

        public void UpdateTween()
        {
            Translate();
            Rotate();
            Scale();
        }

        public void ClearTransformValues()
        {
            myTransform.position = Vector3.zero;
            myTransform.rotation = Quaternion.identity;
            myTransform.localScale = Vector3.one;
        }
        public void SetTransformValuesToInitial()
        {
            myTransform.position = initialPos;
            myTransform.rotation = initialRot;
            myTransform.localScale = initialScale;
        }

        public void StartTranslation()
        {
            if (translating == false)
            {
                previousPosition = myTransform.position;
                initialTranslationTarget = translationTarget;
                translating = true;
            }
            else
            {
                Debug.LogWarning("|Transform Tweener|: Tried to start a translation while translating, stop the translation first or pause the translation instead.", gameObject);
            }
        }

        public void PauseTranslation()
        {
            translationPaused = true;
        }

        public void ResumeTranslation()
        {
            translationPaused = false;
        }

        public void StopTranslation()
        {
            translating = false;
            translationPaused = false;
            translationTime = 0;
            translationTarget = initialTranslationTarget;
            previousPosition = Vector3.negativeInfinity;
        }
        public void StartRotation()
        {
            if (rotating == false)
            {
                previousRotation = myTransform.rotation;
                initialRotationTarget = rotationTarget;
                rotating = true;
            }
            else
            {
                Debug.LogWarning("|Transform Tweener|: Tried to start a rotation while rotateing, stop the rotation first or pause the rotation instead.", gameObject);
            }
        }

        public void PauseRotation()
        {
            rotationPaused = true;
        }

        public void ResumeRotation()
        {
            rotationPaused = false;
        }

        public void StopRotation()
        {
            rotating = false;
            rotationPaused = false;
            rotationTime = 0;
            rotationTarget = initialRotationTarget;
            previousRotation = Quaternion.identity;
        }

        public void StartScale()
        {
            if (scaling == false)
            {
                previousScale = myTransform.localScale;
                initialScaleTarget = scaleTarget;
                scaling = true;
            }
            else
            {
                Debug.LogWarning("|Transform Tweener|: Tried to start a scale while scaling, stop the scale first or pause the scale instead.", gameObject);
            }
        }

        public void PauseScale()
        {
            scalingPaused = true;
        }

        public void ResumeScale()
        {
            scalingPaused = false;
        }

        public void StopScale()
        {
            scaling = false;
            scalingPaused = false;
            scaleTime = 0;
            scaleTarget = initialScaleTarget;
            previousScale = Vector3.negativeInfinity;
        }

        private void Rotate()
        {
            if (rotating == true && rotationPaused == false)
            {
                if (rotationTime < rotationCurve.keys[rotationCurve.length - 1].time)
                {
                    rotationTime += Time.deltaTime;
                    nextRotation = Quaternion.Lerp(transform.rotation, rotationTarget, rotationCurve.Evaluate(rotationTime));
                    transform.rotation = nextRotation;
                }
                else
                {
                    switch (rotationCurve.postWrapMode)
                    {
                        case WrapMode.PingPong:

                            if (rotationTarget == previousRotation)
                            {
                                rotationTarget = initialRotationTarget;
                            }
                            else
                            {
                                rotationTarget = previousRotation;
                            }
                            rotationTime = 0;

                            break;


                        case WrapMode.Loop:

                            myTransform.rotation = initialRotationTarget;
                            rotationTime = 0;

                            break;


                        case WrapMode.Once:

                            myTransform.rotation = initialRotationTarget;
                            StopRotation();

                            break;

                        case WrapMode.ClampForever:

                            myTransform.rotation = rotationTarget;

                            break;

                        case WrapMode.Default:

                            rotationTime = 0;

                            break;

                        default:

                            rotationTime = 0;

                            break;
                    }
                }
            }
        }

        private void Translate()
        {
            // Translate:
            if (translating == true && translationPaused == false)
            {
                if (translationTime < translationCurve.keys[translationCurve.length - 1].time)
                {
                    translationTime += Time.deltaTime;
                    nextPosition = Vector3.Lerp(myTransform.position, translationTarget, translationCurve.Evaluate(translationTime));
                    myTransform.position = nextPosition;

                }
                else
                {
                    switch (translationCurve.postWrapMode)
                    {
                        case WrapMode.PingPong:

                            if (translationTarget == previousPosition)
                            {
                                translationTarget = initialTranslationTarget;
                            }
                            else
                            {
                                translationTarget = previousPosition;
                            }
                            translationTime = 0;

                            break;


                        case WrapMode.Loop:

                            myTransform.position = initialTranslationTarget;
                            translationTime = 0;

                            break;


                        case WrapMode.Once:

                            myTransform.position = initialTranslationTarget;
                            StopTranslation();

                            break;

                        case WrapMode.ClampForever:

                            myTransform.position = translationTarget;

                            break;

                        case WrapMode.Default:

                            translationTime = 0;

                            break;

                        default:

                            translationTime = 0;

                            break;
                    }
                }
            }
        }

        private void Scale()
        {
            // Scale:
            if (scaling == true && scalingPaused == false)
            {
                if (scaleTime < scaleCurve.keys[scaleCurve.length - 1].time)
                {
                    scaleTime += Time.deltaTime;
                    nextScale = Vector3.Lerp(transform.localScale, scaleTarget, scaleCurve.Evaluate(scaleTime));
                    if (snappyScale == true)
                    {
                        nextScale = new Vector3(Mathf.RoundToInt(nextScale.x), Mathf.RoundToInt(nextScale.y), Mathf.RoundToInt(nextScale.z));
                    }
                    transform.localScale = nextScale;
                }
                else
                {
                    switch (scaleCurve.postWrapMode)
                    {
                        case WrapMode.PingPong:

                            if (scaleTarget == previousScale)
                            {
                                scaleTarget = initialScaleTarget;
                            }
                            else
                            {
                                scaleTarget = previousScale;
                            }
                            scaleTime = 0;

                            break;


                        case WrapMode.Loop:

                            myTransform.localScale = initialScaleTarget;
                            scaleTime = 0;

                            break;


                        case WrapMode.Once:

                            myTransform.localScale = initialScaleTarget;
                            StopScale();

                            break;

                        case WrapMode.ClampForever:

                            myTransform.localScale = scaleTarget;

                            break;

                        case WrapMode.Default:

                            scaleTime = 0;

                            break;

                        default:

                            scaleTime = 0;

                            break;
                    }
                }
            }
        }
    }
}