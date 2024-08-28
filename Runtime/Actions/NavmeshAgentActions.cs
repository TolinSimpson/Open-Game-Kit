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
using UnityEngine.AI;

namespace OGK
{
    public static class NavMeshAgentActions
    {
        public static bool DestinationReached(NavMeshAgent agent)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    [SRName("NavMeshAgent/Move To Position")]
    public class AgentMoveToPositionAction : ActionModule
    {
        public NavMeshAgent agent;
        public Vector3 target;
        public override ActionEvent Invoke() { if (agent != null) { agent.SetDestination(target); } return ActionEvent.Continue; }
    }

    [SRName("NavMeshAgent/Move To Transform")]
    public class AgentMoveToTransformAction : ActionModule
    {
        public NavMeshAgent agent;
        public Transform target;
        public override ActionEvent Invoke() { if (agent != null && target != null) { agent.SetDestination(target.position); } return ActionEvent.Continue; }
    }

    [SRName("NavMeshAgent/Idle")]
    public class AgentIdleAction : ActionModule
    {
        public NavMeshAgent agent;
        public override ActionEvent Invoke()
        {
            if (agent != null)
            {
                agent.autoBraking = true;
                agent.SetDestination(agent.transform.position);
            }
            return ActionEvent.Continue; }
    }

    [SRName("NavMeshAgent/Follow")]
    public class AgentFollowAction : ActionModule
    {
        public NavMeshAgent agent;
        public Transform target;
        public float followDistance = 1;
        public override ActionEvent Invoke()
        {
            if (agent != null && target != null && (followDistance <= Vector3.Distance(agent.transform.position, target.transform.position)))
            {
                agent.SetDestination(target.transform.position + (-target.transform.forward * followDistance));
                agent.transform.LookAt(target.transform.position);
                agent.stoppingDistance = agent.radius * followDistance;
            }
            return ActionEvent.Continue;
        }
    }

    [SRName("NavMeshAgent/Wander")]
    public class AgentWanderAction : ActionModule
    {
        public NavMeshAgent agent;
        public float wanderRange = 1;
        private NavMeshHit hit;
        public override ActionEvent Invoke()
        {
            if (agent != null && agent.gameObject.activeSelf == true)
            { 
                if (NavMesh.SamplePosition((UnityEngine.Random.insideUnitSphere * wanderRange) + agent.transform.position, out hit, wanderRange, -1))
                {
                    agent.SetDestination(hit.position);
                }
            }
            return ActionEvent.Continue;
        }
    }

    [SRName("NavMeshAgent/Flee")]
    public class AgentFleeAction : ActionModule
    {
        public NavMeshAgent agent;
        public Transform target;
        public float fleeDistance = 5;
        public override ActionEvent Invoke()
        {
            if (agent != null && target != null)
            {
                agent.SetDestination(fleeDistance * target.forward);
            }
            return ActionEvent.Continue;
        }
    }

    [SRName("NavMeshAgent/Follow Path")]
    public class AgentFollowPathAction : ActionModule
    {
        public NavMeshAgent agent;
        public Path path;

        [Min(0)]
        [Tooltip("The current path point being navigated to.")]
        public int progress = 0;
        public override ActionEvent Invoke()
        {
            if (agent != null && path.points.Count > 0)
            {
                if (agent.remainingDistance <= Mathf.Epsilon && progress < path.points.Count)
                {
                    agent.SetDestination(path.points[progress++]);
                }
                else
                {
                    if (progress >= path.points.Count)
                    {
                        progress = 0;
                    }
                    agent.SetDestination(path.points[progress]);
                }
            }
            return ActionEvent.Continue;
        }
    }

    [SRName("NavMeshAgent/Modify/Speed")]
    public class AgentModifySpeedAction : ActionModule
    {
        public NavMeshAgent agent;
        public NumericalOperators operation;
        [Min(0)][Tooltip("Maximum movement speed when following a path.")]
        public float value;
        private float a;
        public override ActionEvent Invoke() 
        { 
            if (agent != null) 
            { 
                a = agent.speed; LogicOperations.PerformNumericalOperation(ref a, operation, value); agent.speed = a; 
            } 
            return ActionEvent.Continue; 
        }
    }

    [SRName("NavMeshAgent/Modify/Angular Speed")]
    public class AgentModifyAngularSpeedAction : ActionModule
    {
        public NavMeshAgent agent;
        public NumericalOperators operation;
        [Min(0)][Tooltip("Maximum turning speed in (deg/s) while following a path.")]
        public float value;
        private float a;
        public override ActionEvent Invoke() 
        { 
            if (agent != null) 
            { 
                a = agent.angularSpeed; LogicOperations.PerformNumericalOperation(ref a, operation, value); agent.angularSpeed = a; 
            } 
            return ActionEvent.Continue; 
        }
    }

    [SRName("NavMeshAgent/Modify/Radius")]
    public class AgentModifyRadiusAction : ActionModule
    {
        public NavMeshAgent agent;
        public NumericalOperators operation;
        public float value;
        private float a;
        public override ActionEvent Invoke() 
        { 
            if (agent != null) 
            {
                a = agent.radius; LogicOperations.PerformNumericalOperation(ref a, operation, value); agent.radius = a; 
            }
            return ActionEvent.Continue; 
        }
    }

    [SRName("NavMeshAgent/Modify/Acceleration")]
    public class AgentModifyAcceleration : ActionModule
    {
        public NavMeshAgent agent;
        public NumericalOperators operation;
        [Min(0)][Tooltip("The maximum acceleration of an agent as it follows its path, given in units/sec^2.")]
        public float value;
        private float a;
        public override ActionEvent Invoke() 
        { 
            if (agent != null) 
            { 
                a = agent.acceleration; LogicOperations.PerformNumericalOperation(ref a, operation, value); agent.acceleration = a; 
            } 
            return ActionEvent.Continue; 
        }
    }

    [SRName("NavMeshAgent/Modify/Stopping Distance")]
    public class AgentModifyStoppingDistance : ActionModule
    {
        public NavMeshAgent agent;
        public NumericalOperators operation;
        [Min(0)][Tooltip("Stop within this distance of the target position.")]
        public float value;
        private float a;
        public override ActionEvent Invoke()
        {
            if (agent != null)
            {
                a = agent.stoppingDistance; LogicOperations.PerformNumericalOperation(ref a, operation, value); agent.stoppingDistance = a;
            }
            return ActionEvent.Continue;
        }
    }

    #region Conditions:

    [SRName("NavMeshAgent/Conditions/Has Path?")]
    public class AgentHasPathAction : ActionModule
    {
        public NavMeshAgent agent;
        public BooleanComparisons comparison = BooleanComparisons.EqualTo;
        public bool value = false;
        public ActionEvent ifTrue, ifFalse;
        public override ActionEvent Invoke() 
        { 
            if (agent != null) 
            { 
                return LogicOperations.BooleanComparison(agent.hasPath, comparison, value) ? ifTrue : ifFalse; 
            } 
            else return ActionEvent.Continue; 
        }
    }

    [SRName("NavMeshAgent/Conditions/Destination Reached?")]
    public class AgentDestinationReachedAction : ActionModule
    {
        public NavMeshAgent agent;
        public ActionEvent ifTrue, ifFalse;
        public override ActionEvent Invoke()
        {
            if (agent != null)
            {
                return NavMeshAgentActions.DestinationReached(agent) ? ifTrue : ifFalse;
            }
            else return ActionEvent.Continue;
        }
    }

    #endregion
}