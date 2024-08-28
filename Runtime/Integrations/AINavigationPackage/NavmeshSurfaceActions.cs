#if ENABLE_UNITY_AI_NAVIGATION

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
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace OGK
{
    [SRName("NavMeshSurface/Build")]
    public class NavMeshSurfaceBuildAction : ActionModule
    {
        public NavMeshSurface navMesh;
        public override ActionEvent Invoke()
        {
            if (navMesh != null)
            {
                navMesh.BuildNavMesh();
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("NavMeshSurface/Set Settings")]
    public class NavMeshSurfaceSetSettingsAction : ActionModule
    {
        public NavMeshSurface navMesh;
        public NavMeshCollectGeometry useGeometry = NavMeshCollectGeometry.RenderMeshes;
        public CollectObjects collectObjects = CollectObjects.Volume;
        public Vector3 size = Vector3.one;
        public Vector3 center;
        public LayerMask includeLayers;

        public override ActionEvent Invoke()
        {
            if (navMesh != null)
            {
                navMesh.useGeometry = useGeometry;
                navMesh.collectObjects = collectObjects;
                navMesh.size = size;
                navMesh.center = center;
                navMesh.layerMask = includeLayers;
                  
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("NavMeshSurface/Set Advanced Settings")]
    public class NavMeshSurfaceSetAdvancedSettingsAction : ActionModule
    {
        public NavMeshSurface navMesh;
        public bool overrideVoxelSize = false;
        [Min(0.01f)]
        public float voxelSize = 0.1666667f;
        public bool overrideTileSize = false;
        [Range(16, 1024)]
        public int tileSize = 256;
        [Min(0)]
        public float minimumRegionArea = 2;
        public bool buildHeightMesh = false;

        public override ActionEvent Invoke()
        {
            if (navMesh != null)
            {
                if(overrideVoxelSize == true)
                {
                    navMesh.voxelSize = voxelSize;
                }
                if(overrideTileSize == true)
                {
                    navMesh.tileSize = tileSize;
                }
                navMesh.minRegionArea = minimumRegionArea;
                navMesh.buildHeightMesh = buildHeightMesh;
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }
}
#endif