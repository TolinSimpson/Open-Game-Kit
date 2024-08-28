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
    [SRName("Animator/Set Bool")]
    public class AnimatorSetBoolAction : ActionModule
    {
        public Animator animator;
        public string parameterName;
        public bool state = false;

        public override ActionEvent Invoke() { if (animator != null) { animator.SetBool(parameterName, state); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Animator/Set Float")]
    public class AnimatorSetFloatAction : ActionModule
    {
        public Animator animator;
        public string parameterName;
        public float value = 0;

        public override ActionEvent Invoke() { if (animator != null) { animator.SetFloat(parameterName, value); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Animator/Set Int")]
    public class AnimatorSetIntAction : ActionModule
    {
        public Animator animator;
        public string parameterName;
        public int value = 0;

        public override ActionEvent Invoke() { if (animator != null) { animator.SetInteger(parameterName, value); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Animator/Set IK Look At Weight")]
    public class AnimatorSetIKLookAtWeightAction : ActionModule
    {
        public Animator animator;
        [Range(0, 1)]
        public float value = 0;

        public override ActionEvent Invoke() { if (animator != null) { animator.SetLookAtWeight(value); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Animator/Set IK Goal Weight")]
    public class AnimatorSetIKWeightAction : ActionModule
    {
        public Animator animator;
        public AvatarIKGoal goal;
        [Range(0, 1)]
        public float value = 0;

        public override ActionEvent Invoke() { if (animator != null) { animator.SetIKPositionWeight(goal, value); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Animator/Set IK Target")]
    public class AnimatorSetIKTargetAction : ActionModule
    {
        public Animator animator;
        public Transform target;
        public AvatarIKGoal goal;

        public override ActionEvent Invoke() 
        { 
            if (animator != null && target != null)
            {
                animator.SetIKPosition(goal, target.position);
                animator.SetIKRotation(goal, target.rotation);
                return ActionEvent.Continue; 
            } 
            else return ActionEvent.Error;
        }
    }

    [SRName("Animator/Set IK Look At Target")]
    public class AnimatorSetIKLookAtTargetAction : ActionModule
    {
        public Animator animator;
        public Transform target;

        public override ActionEvent Invoke()
        {
            if (animator != null)
            {
                animator.SetLookAtPosition(target.position);
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Animator/Set IK Look At Cursor")]
    public class AnimatorIKLookAtCursorAction : ActionModule
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Camera camera;

        private Vector3 targetPos;

        public override ActionEvent Invoke()
        {
            if (animator != null)
            {
                if(camera == null)
                {
                    camera = Camera.main;
                }
                targetPos = Input.mousePosition;
                targetPos.z = camera.nearClipPlane + 1;
                animator.SetLookAtPosition(camera.ScreenToWorldPoint(targetPos));
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Animator/Clear All IK Weights")]
    public class AnimatorClearIKWeightsAction : ActionModule
    {
        [SerializeField] private Animator animator;

        public override ActionEvent Invoke()
        {
            if (animator != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetLookAtWeight(0);
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }
}