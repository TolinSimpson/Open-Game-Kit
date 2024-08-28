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
    [SRName("Rigidbody/Clear Velocity")]
    public class RigidbodyClearVelocityAction : ActionModule
    {
        public Rigidbody rigid;
        public override ActionEvent Invoke() { if (rigid != null) { rigid.velocity = Vector3.zero; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Rigidbody/Apply Wind Force")]
    public class RigidbodyApplyWindForceAction : ActionModule
    {
        public Rigidbody rigid;
        public WindZone wind;
        public float amount;

        public override ActionEvent Invoke()
        {
            if (rigid != null && wind != null)
            {
                rigid.AddForce(wind.windMain * wind.transform.forward);

                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Rigidbody/Apply Directional Force")]
    public class RigidbodyApplyDirectionalForceAction : ActionModule
    {
        public Rigidbody rigid;
        public ForceMode mode;
        public Directions direction;
        public float amount;

        public override ActionEvent Invoke() 
        { 
            if (rigid != null) 
            { 
                switch(direction)
                {
                    case Directions.Back: rigid.AddForce(Vector3.back * amount, mode); break;
                    case Directions.Down: rigid.AddForce(Vector3.down * amount, mode); break;
                    case Directions.Forward: rigid.AddForce(Vector3.forward * amount, mode); break;
                    case Directions.Left: rigid.AddForce(Vector3.left * amount, mode); break;
                    case Directions.Right: rigid.AddForce(Vector3.right * amount, mode); break;
                    case Directions.Up: rigid.AddForce(Vector3.up * amount, mode); break;
                }
                return ActionEvent.Continue; 
            } 
            else return ActionEvent.Error;
        }
    }

    [SRName("Rigidbody/Apply Force")]
    public class RigidbodyApplyForceAction : ActionModule
    {
        public Rigidbody rigid;
        public ForceMode mode;
        public Vector3 force;

        public override ActionEvent Invoke()
        {
            if (rigid != null) { rigid.AddForce(force, mode); return ActionEvent.Continue; } else return ActionEvent.Error;
        }
    }

    [SRName("Rigidbody/Follow Target")]
    public class RigidbodyFollowTargetAction : ActionModule
    {
        public Rigidbody rigid;
        public Transform target;
        public ForceMode mode;
        public float speed;

        public override ActionEvent Invoke()
        {
            if (rigid != null && target != null) { rigid.AddForce((target.position - rigid.transform.position).normalized * speed, mode); return ActionEvent.Continue; } else return ActionEvent.Error;
        }
    }

    [SRName("Rigidbody/Modify/Kinematic State")]
    public class RigidbodySetKinematicAction : ActionModule
    {
        public Rigidbody rigid;
        public bool state = false;
        public override ActionEvent Invoke() { if (rigid != null) { rigid.isKinematic = state; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Rigidbody/Toggle Kinematic State")]
    public class RigidbodyToggleKinematicAction : ActionModule
    {
        public Rigidbody rigid;
        public override ActionEvent Invoke() { if (rigid != null) { rigid.isKinematic = !rigid.isKinematic; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Rigidbody/Conditions/Is Kinematic?")]
    public class RigidbodyIsKinematicAction : ActionModule
    {
        public Rigidbody rigid;
        public ActionEvent ifTrue, ifFalse;
        public override ActionEvent Invoke() { if (rigid != null) { return rigid.isKinematic ? ifTrue : ifFalse; } else return ActionEvent.Error; }
    }
}