#if OGK_PUPPETMASTER_INTEGRATION

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
using RootMotion.Dynamics;

namespace OGK
{
    [SRName("Puppet Master/Conditions/Is Alive?")]
    public class PuppetMasterIsAliveAction : ActionModule
    {
        public PuppetMaster puppet;
        public ActionEvent ifTrue, ifFalse;
        public override ActionEvent Invoke() { if (puppet != null) { return puppet.isAlive ? ifTrue : ifFalse; } else return ActionEvent.Error; }
    }

    [SRName("Puppet Master/Set State")]
    public class PuppetMasterSetStateAction : ActionModule
    {
        [SerializeField] private PuppetMaster puppet;
        [SerializeField] private PuppetMaster.State state = PuppetMaster.State.Alive;
        public override ActionEvent Invoke() { if (puppet != null) {
                puppet.state = state;
                    switch(state)
                {
                    case PuppetMaster.State.Frozen: puppet.state = state;  break;
                }

                        return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Puppet Master/Set Mode")]
    public class PuppetMasterSetModeAction : ActionModule
    {
        [SerializeField] private PuppetMaster puppet;
        [SerializeField] private PuppetMaster.Mode mode = PuppetMaster.Mode.Active;
        public override ActionEvent Invoke() { if (puppet != null) { puppet.mode = mode; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Puppet Master/Clear Ragdoll Velocity")]
    public class PuppetMasterClearRagdollVelocityAction : ActionModule
    {
        [SerializeField] private PuppetMaster puppet;
        public override ActionEvent Invoke() { if (puppet != null) { foreach (Muscle m in puppet.muscles) { m.rigidbody.velocity = Vector3.zero; } return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

}
#endif