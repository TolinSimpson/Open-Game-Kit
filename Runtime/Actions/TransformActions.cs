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
using SerializeReferenceEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OGK
{
    public static class TransformActions
    {
        public static void MoveInDirection(Transform transform, Directions direction, float distance)
        {
            if (transform != null)
            {
                switch (direction)
                {
                    case Directions.Forward: transform.position += Vector3.forward * distance; break;
                    case Directions.Left: transform.position += Vector3.left * distance; break;
                    case Directions.Right: transform.position += Vector3.right * distance; break;
                    case Directions.Up: transform.position += Vector3.up * distance; break;
                    case Directions.Back: transform.position += Vector3.back * distance; break;
                    case Directions.Down: transform.position += Vector3.down * distance; break;
                }
            }
        }
    }

    [SRName("Transform/Look At")]
    public class LookatAction : ActionModule
    {
        public Transform transform, target;
        public Vector3 worldUp = Vector3.up;

        public override ActionEvent Invoke()
        {
            transform.LookAt(target, worldUp);
            return ActionEvent.Continue;
        }
    }

    [SRName("Transform/Billboard")]
    public class BillboardAction : ActionModule
    {
        public Transform transform, target;
        public BillboardModes mode;
        private Vector3 lookAtPos, currentPosition;
        private Quaternion currentRotation;

        public override ActionEvent Invoke()
        {        
            if (lookAtPos != target.position || currentPosition != transform.position || transform.rotation != currentRotation)
            {
                switch (mode)
                {
                    case BillboardModes.Direct: lookAtPos = 2 * transform.position - target.position; break;
                    case BillboardModes.HorizontalDirect: lookAtPos = 2 * transform.position - target.position; lookAtPos.y = transform.position.y; break;
                    case BillboardModes.Unconstrained: lookAtPos = target.position; break;
                    case BillboardModes.Horizonal: lookAtPos = new Vector3(target.position.x, transform.position.y, target.position.z); break;
                }
                transform.LookAt(lookAtPos);
                currentPosition = transform.position;
                currentRotation = transform.rotation;
            }
            return ActionEvent.Continue;
        }
    }

    [SRName("Transform/Move In Direction")]
    public class MoveInDirectionAction : ActionModule
    {
        public Transform transform;
        public Directions direction;
        public float distance = 1;

        public override ActionEvent Invoke() { TransformActions.MoveInDirection(transform, direction, distance); return ActionEvent.Continue; }
    }

    [SRName("Transform/Move Root In Direction")]
    public class MoveRootInDirectionAction : ActionModule
    {
        public Transform transform;
        public Directions direction;
        public float distance = 1;

        public override ActionEvent Invoke() { TransformActions.MoveInDirection(transform.root, direction, distance); return ActionEvent.Continue; }
    }

    [SRName("Transform/To Transform Position")]
    public class ToTransformPositionAction : ActionModule
    {
        public Transform transform, target;
        public override ActionEvent Invoke() { if (transform != null && target != null) { transform.position = target.position; } return ActionEvent.Continue; }
    }

    [SRName("Transform/To Transform Position XZ")]
    public class ToTransformPositionXZAction : ActionModule
    {
        public Transform transform, target;
        public override ActionEvent Invoke() { if (transform != null && target != null) { transform.position = new Vector3(target.position.x, 0, target.position.z); } return ActionEvent.Continue; }
    }

    [SRName("Transform/To Transform Position Y")]
    public class ToTransformPositionYAction : ActionModule
    {
        public Transform transform, target;
        public override ActionEvent Invoke() { if (transform != null && target != null) { transform.position = Vector3.up * target.position.y; } return ActionEvent.Continue; }
    }

    [SRName("Transform/Modify/Position and Rotation")]
    public class SetTransformAction : ActionModule
    {
        public Transform transform;
        public Vector3 newPosition;
        public Quaternion newRotation = Quaternion.identity;

        public override ActionEvent Invoke() { if (transform != null) { transform.position = newPosition; transform.rotation = newRotation; } return ActionEvent.Continue; }
    }

    [SRName("Transform/Set Position To Screen Point")]
    public class SetTransformPosToScreenPointAction : ActionModule
    {
        public Transform transform;
        [Tooltip("Defaults to main camera if unassigned.")] [SerializeField] private Camera camera;
        [Tooltip("Defaults to world origin if unassigned.")] [SerializeField] private Transform worldPos;

        public override ActionEvent Invoke() 
        { 
            if (transform != null) 
            { 
                if (worldPos != null) 
                { 
                    if(camera != null)
                    {
                        camera.WorldToScreenPoint(worldPos.position);
                    }
                    else
                    {
                        Camera.main.WorldToScreenPoint(worldPos.position);
                    }
                } 
                else 
                {
                    if (camera != null)
                    {
                        camera.WorldToScreenPoint(Vector3.zero);
                    }
                    else
                    {
                        Camera.main.WorldToScreenPoint(Vector3.zero);
                    }
                } 
            } 
            return ActionEvent.Continue; 
        }
    }

 

    [SRName("Transform/Spin")]
    public class SpinTransformAction : ActionModule
    {
        [Message("Spins around axis by angle (delta time)")]
        public Transform transform;
        public Vector3 axis;
        public float angle;
        //public TimeScales timeScale;

        public override ActionEvent Invoke() { if (transform != null) { transform.Rotate(axis, angle * Time.deltaTime); } return ActionEvent.Continue; }
    }

    [SRName("Transform/Modify/Position")]
    public class SetPositionAction : ActionModule
    {
        public Transform transform;
        public Vector3 newPosition;

        public override ActionEvent Invoke() { if (transform != null) { transform.position = newPosition; } return ActionEvent.Continue; }
    }

    [SRName("Transform/Modify/Rotation")]
    public class SetRotationAction : ActionModule
    {
        public Transform transform;
        public Quaternion newRotation = Quaternion.identity;

        public override ActionEvent Invoke() { if (transform != null) { transform.rotation = newRotation; } return ActionEvent.Continue; }
    }

    [SRName("Transform/Modify/Parent")]
    public class SetParentAction : ActionModule
    {
        public Transform transform, parent;

        public override ActionEvent Invoke() { if (transform != null) { transform.SetParent(parent); } return ActionEvent.Continue; }
    }

    [SRName("Transform/Conditions/Has Parent?")]
    public class TransformHasParentAction : ActionModule
    {
        public Transform transform;
        public ActionEvent ifTrue, ifFalse;
        public override ActionEvent Invoke() { return transform.parent ? ifTrue : ifFalse; }
    }

    [SRName("Transform/Conditions/Distance Between Transforms")]
    public class DistanceBetweenTransformsAction : ActionModule
    {
        public Transform transform, target;
        public NumericalComparisons comparison = NumericalComparisons.EqualTo;
        public float distance = 0;
        public ActionEvent ifTrue, ifFalse;
        public override ActionEvent Invoke() 
        {
            if (transform != null && target != null)
            {
                return LogicOperations.NumericalComparison(Vector3.Distance(transform.position, target.position), comparison, distance) ? ifTrue : ifFalse;
            }
            else return ActionEvent.Error;
        }
    }
}