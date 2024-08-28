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

    [SRName("Raycaster/Raycast")]
    public class RaycasterAction : ActionModule
    {
        public Raycaster raycaster;
        public RaycastTypes type = RaycastTypes.Raycast;
        public override ActionEvent Invoke()
        {
            if (raycaster != null)
            {
                raycaster.Raycast(type);
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Raycaster/Modify/Scatter Cast Settings")]
    public class RaycasterModifyScatterCastAction : ActionModule
    {
        public Raycaster raycaster;

        [Range(0, 360), Tooltip("The vertical range when invoking the scatter raycast action.")]
        public float verticalScatter = 45;
        [Range(0, 360), Tooltip("The horizontal range when invoking the scatter raycast action.")]
        public float horizontalScatter = 45;
        public override ActionEvent Invoke()
        {
            if (raycaster != null)
            {
                raycaster.verticalScatter = verticalScatter;
                raycaster.horizontalScatter = horizontalScatter;
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Raycaster/Modify/Ray Settings")]
    public class RaycasterModifyRaySettingsAction : ActionModule
    {
        public Raycaster raycaster;
        [Min(0.001f), Tooltip("The length of a raycast.")]
        public float maxRayDistance = 1000;
        [Tooltip("Spacing for grid, line and circle raycast types & the size for sphere, capsule and box casts.")]
        public float raySpacing = 1f;
        public override ActionEvent Invoke()
        {
            if (raycaster != null)
            {
                raycaster.maxRayDistance = maxRayDistance;
                raycaster.raySpacing = raySpacing;
                raycaster.UpdateRaycastHitCache();
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Raycaster/Modify/Physics Settings")]
    public class RaycasterModifyPhysicsSettingsAction : ActionModule
    {
        public Raycaster raycaster;

        [Tooltip("The layer mask that the next raycast will detect objects in.")]
        public LayerMask layerMask = 1;
        [Tooltip("How raycasts and colliders interact with colliders marked as triggers.")]
        public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
        public override ActionEvent Invoke()
        {
            if (raycaster != null)
            {
                raycaster.layerMask = layerMask;
                raycaster.triggerInteraction = triggerInteraction;
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Raycaster/Modify/All Settings")]
    public class RaycasterModifyAllSettingsAction : ActionModule
    {
        public Raycaster raycaster;
        [Min(0.001f), Tooltip("The length of a raycast.")]
        public float maxRayDistance = 1000;
        [Tooltip("Spacing for grid, line and circle raycast types & the size for sphere, capsule and box casts.")]
        public float raySpacing = 1f;
        [Range(0, 360), Tooltip("The vertical range when invoking the scatter raycast action.")]
        public float verticalScatter = 45;
        [Range(0, 360), Tooltip("The horizontal range when invoking the scatter raycast action.")]
        public float horizontalScatter = 45;
        [Tooltip("The layer mask that the next raycast will detect objects in.")]
        public LayerMask layerMask = 1;
        [Tooltip("How raycasts and colliders interact with colliders marked as triggers.")]
        public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
        public override ActionEvent Invoke()
        {
            if (raycaster != null)
            {
                raycaster.maxRayDistance = maxRayDistance;
                raycaster.raySpacing = raySpacing;
                raycaster.UpdateRaycastHitCache();
                raycaster.verticalScatter = verticalScatter;
                raycaster.horizontalScatter = horizontalScatter;
                raycaster.layerMask = layerMask;
                raycaster.triggerInteraction = triggerInteraction;
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Raycaster/Conditions/Has Hits?")]
    public class RaycasterHasHitsAction : ActionModule
    {
        public Raycaster raycaster;
        public ActionEvent ifTrue, ifFalse = ActionEvent.Continue;
        public override ActionEvent Invoke() { if (raycaster != null) { return raycaster.hits.Length > 0 ? ifTrue : ifFalse; } else return ActionEvent.Error; }
    }

    [SRName("Raycaster/Clear Hits")]
    public class RaycasterClearHitsAction : ActionModule
    {
        public Raycaster raycaster;
        public override ActionEvent Invoke() { if (raycaster != null) { raycaster.hits = new RaycastHit[raycaster.rayCount]; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }
}