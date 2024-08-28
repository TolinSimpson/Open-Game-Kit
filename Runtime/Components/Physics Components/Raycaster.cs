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
    [AddComponentMenu("Open Game Kit/Physics/Raycaster")]
    public class Raycaster : MonoBehaviour
    {
        private Ray tempRay = new Ray(Vector3.zero, Vector3.forward);

        public RaycastHit[] hits = new RaycastHit[0];

        [Min(1), Tooltip("The amount of rays to cast.")]
        public int rayCount = 1;

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

        private Transform myTransform;

#if UNITY_EDITOR

        [Tooltip("Visualize approximations of raycasts based on the current raycast settings.")]
        public RaycastTypes raycastDebug = RaycastTypes.None;
        private Mesh debugCapsule;

        private void OnDrawGizmosSelected()
        {
            if (myTransform == null)
            {
                myTransform = transform;
            }

            Gizmos.color = Color.green;
            switch (raycastDebug)
            {
                case RaycastTypes.None:

                    break;

                case RaycastTypes.Raycast:

                    Gizmos.DrawRay(myTransform.position, myTransform.forward * maxRayDistance);

                    break;

                case RaycastTypes.RaycastRow:

                    for (int i = 0; i < rayCount; i++)
                    {
                        Gizmos.DrawRay(myTransform.position + ((i - (rayCount - 1) / 2f) * raySpacing * myTransform.right), myTransform.forward * maxRayDistance);
                    }

                    break;

                case RaycastTypes.ScatterCast:

                    for (int i = 0; i < rayCount; i++)
                    {
                        Gizmos.DrawRay(myTransform.position, (Quaternion.Euler(UnityEngine.Random.Range(-verticalScatter, verticalScatter), UnityEngine.Random.Range(-horizontalScatter, horizontalScatter), 0) * myTransform.forward) * maxRayDistance);
                    }

                    break;

                case RaycastTypes.SphereCast:

                    Gizmos.DrawWireSphere(myTransform.position, raySpacing / 2);

                    break;

                case RaycastTypes.GridCast:

                    int gridSize = (int)Mathf.Sqrt(rayCount);
                    Vector3[] rayOrigins = new Vector3[gridSize * gridSize];

                    for (int i = 0; i < gridSize; i++)
                    {
                        for (int j = 0; j < gridSize; j++)
                        {
                            rayOrigins[i * gridSize + j] = myTransform.position + new Vector3((i - (gridSize - 1) / 2f) * raySpacing, (j - (gridSize - 1) / 2f) * raySpacing, 0);
                        }
                    }

                    for (int i = 0; i < rayOrigins.Length; i++)
                    {
                        Gizmos.DrawRay(rayOrigins[i], myTransform.forward * maxRayDistance);
                    }

                    break;

                case RaycastTypes.FanCast:

                    for (int i = 0; i < rayCount; i++)
                    {
                        Gizmos.DrawRay(myTransform.position, (Quaternion.AngleAxis(Mathf.Clamp(180f / rayCount * i - 90f, -180f, 180f), myTransform.up) * myTransform.forward) * maxRayDistance);
                    }

                    break;

                case RaycastTypes.CircleCast:

                    for (int i = 0; i < rayCount; i++)
                    {
                        Gizmos.DrawRay(myTransform.position + Quaternion.LookRotation(myTransform.up, myTransform.forward) * Quaternion.Euler(0, i * (360f / rayCount), 0) * myTransform.right * rayCount * raySpacing, myTransform.forward * maxRayDistance);
                    }

                    break;

                case RaycastTypes.CapsuleCast:

                    if (debugCapsule == null)
                    {
                        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                        capsule.hideFlags = HideFlags.HideInHierarchy;
                        debugCapsule = capsule.GetComponent<MeshFilter>().sharedMesh;
                        if (Application.isPlaying == true)
                        {
                            Destroy(capsule);
                        }
                        else
                        {
                            DestroyImmediate(capsule);
                        }
                    }
                    else
                    {
                        Gizmos.DrawWireMesh(debugCapsule, 0, myTransform.position, Quaternion.Euler(myTransform.forward), Vector3.one * (raySpacing / 2));
                    }

                    break;

                case RaycastTypes.BoxCast:

                    Gizmos.DrawWireCube(myTransform.position, Vector3.one * raySpacing);

                    break;

                case RaycastTypes.LineCast:

                    Gizmos.DrawLine(myTransform.position, Vector3.forward * maxRayDistance);

                    break;

                default:

                    break;
            }

            if (Application.isPlaying == true && hits.Length > 0)
            {
                raycastDebug = RaycastTypes.None;
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider != null)
                    {
                        Gizmos.DrawLine(tempRay.origin, hit.point);
                        Gizmos.DrawSphere(hit.point, 0.15f);
                    }
                }
            }
        }

#endif

        private void Awake()
        {
            myTransform = transform;
        }

        public void UpdateRaycastHitCache()
        {
            if (hits.Length != rayCount)
            {
                hits = new RaycastHit[rayCount];
            }
        }

        public void RaycastSingle()
        {
            tempRay.origin = myTransform.position;
            tempRay.direction = myTransform.forward;
            Physics.RaycastNonAlloc(tempRay, hits, maxRayDistance, layerMask, triggerInteraction);
        }

        public void RaycastAll()
        {
            tempRay.origin = myTransform.position;
            tempRay.direction = myTransform.forward;
            hits = Physics.RaycastAll(tempRay);
        }

        public void SphereCast()
        {
            tempRay.origin = myTransform.position;
            tempRay.direction = myTransform.forward;
            Physics.SphereCastNonAlloc(tempRay, (raySpacing / 2), hits);
        }

        public void SphereCastAll()
        {
            tempRay.origin = myTransform.position;
            tempRay.direction = myTransform.forward;
            hits = Physics.SphereCastAll(tempRay, (raySpacing / 2));
        }

        public void BoxCastAll()
        {
            hits = Physics.BoxCastAll(myTransform.position, Vector3.one * (raySpacing / 2), myTransform.forward, Quaternion.identity, maxRayDistance, layerMask, triggerInteraction);
        }

        public void BoxCast()
        {
            Physics.BoxCastNonAlloc(myTransform.position, Vector3.one * (raySpacing / 2), myTransform.forward, hits, Quaternion.identity, maxRayDistance, layerMask, triggerInteraction);
        }

        public void CapsuleCast()
        {
            Physics.CapsuleCastNonAlloc(myTransform.position, myTransform.up, (raySpacing / 2), Vector3.forward, hits);
        }

        public void LineCast()
        {
            Physics.Linecast(myTransform.position, Vector3.forward * maxRayDistance, layerMask, triggerInteraction);
        }

        public void RaycastScatter()
        {
            UpdateRaycastHitCache();
            for (int i = 0; i < rayCount; i++)
            {
                tempRay.origin = myTransform.position;
                tempRay.direction = Quaternion.Euler(UnityEngine.Random.Range(-verticalScatter, verticalScatter), UnityEngine.Random.Range(-horizontalScatter, horizontalScatter), 0) * myTransform.forward;
                Physics.RaycastNonAlloc(tempRay, hits, maxRayDistance, layerMask, triggerInteraction);
            }
        }

        public void RaycastRow()
        {
            UpdateRaycastHitCache();
            for (int i = 0; i < rayCount; i++)
            {
                tempRay.origin = myTransform.position + ((i - (rayCount - 1) / 2f) * raySpacing * myTransform.right);
                tempRay.direction = myTransform.forward;
                Physics.RaycastNonAlloc(tempRay, hits, maxRayDistance, layerMask, triggerInteraction);
            }
        }

        public void RaycastGrid()
        {
            int gridSize = (int)Mathf.Sqrt(rayCount);
            hits = new RaycastHit[gridSize * gridSize];
            Vector3[] rayOrigins = new Vector3[gridSize * gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    rayOrigins[i * gridSize + j] = myTransform.position + new Vector3((i - (gridSize - 1) / 2f) * raySpacing, (j - (gridSize - 1) / 2f) * raySpacing, 0);
                }
            }

            for (int i = 0; i < rayOrigins.Length; i++)
            {
                tempRay.origin = rayOrigins[i];
                tempRay.direction = myTransform.forward;
                Physics.RaycastNonAlloc(tempRay, hits, maxRayDistance, layerMask, triggerInteraction);
            }
        }

        public void RaycastCircle()
        {
            UpdateRaycastHitCache();
            for (int i = 0; i < rayCount; i++)
            {
                tempRay.origin = myTransform.position + Quaternion.LookRotation(myTransform.up, myTransform.forward) * Quaternion.Euler(0, i * (360f / rayCount), 0) * myTransform.right * rayCount * raySpacing;
                tempRay.direction = myTransform.forward;
                Physics.RaycastNonAlloc(tempRay, hits, maxRayDistance, layerMask, triggerInteraction);
            }
        }

        public void RaycastFan()
        {
            UpdateRaycastHitCache();
            for (int i = 0; i < rayCount; i++)
            {
                tempRay.origin = myTransform.position;
                tempRay.direction = Quaternion.AngleAxis(Mathf.Clamp(180f / rayCount * i - 90f, -180f, 180f), myTransform.up) * myTransform.forward;
                Physics.RaycastNonAlloc(tempRay, hits, maxRayDistance, layerMask, triggerInteraction);
            }
        }

        public void Raycast(RaycastTypes type)
        {
            switch(type)
            {
                case RaycastTypes.BoxCast: BoxCast(); break;
                case RaycastTypes.CapsuleCast: CapsuleCast(); break;
                case RaycastTypes.CircleCast: RaycastCircle(); break;
                case RaycastTypes.FanCast: RaycastFan(); break;
                case RaycastTypes.GridCast: RaycastGrid(); break;
                case RaycastTypes.LineCast: LineCast(); break;
                case RaycastTypes.Raycast: RaycastSingle(); break;
                case RaycastTypes.RaycastRow: RaycastRow(); break;
                case RaycastTypes.ScatterCast: RaycastScatter(); break;
                case RaycastTypes.SphereCast: SphereCast(); break;
                case RaycastTypes.None: break;
            }
        }
    }
}