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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OGK
{

    [DisallowMultipleComponent]
    [HelpURL("https://kitbashery.com/docs/smart-gameobjects/smart-mesh.html")]
    [AddComponentMenu("Open Game Kit/Rendering/Mesh Builder")]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshBuilder : MonoBehaviour
    {
        #region Properties:

        [HideInInspector]
        public MeshFilter meshfilter;

        public MeshTypes meshType = MeshTypes.Asset;
        public Mesh assetMesh;
        [Min(0)]
        public int subdivisions = 1;
        public float height = 1;
        public float width = 1;
        public float depth = 1;
        public float radius = 1;
        public float start = 0;
        [Range(2, 32)]
        public int heightSegments = 2;
        [Range(2, 32)]
        public int widthSegments = 2;
        [Range(2, 32)]
        public int depthSegments = 2;
        [Range(3, 32)]
        public int radialSegments = 3;
        public Vector3[] customVertices;
        public int[] customTriangles;

        public bool top = true;
        public bool bottom = true;
        public bool leftSide = true;
        public bool rightSide = true;
        public bool frontSide = true;
        public bool backSide = true;
        public Vector3 topUVOffset;
        public Vector3 bottomUVOffset;
        public Vector3 leftUVOffset;
        public Vector3 rightUVOffset;
        public Vector3 frontUVOffset;
        public Vector3 backUVOffset;

        public Vector3 vertexOffset;
        public Vector3 vertexRotation;
        public Vector3 vertexScale = Vector3.one;
        public float noiseStrength = 0;
        public Vector2 xBounds = new Vector2(-3, 3);
        public Vector2 yBounds = new Vector2(-3, 3);
        public Vector2 zBounds = new Vector2(-3, 3);
        public bool sphereify = false;
        public float sphereifyRadius = 1;

        public UVProjections unwrapMode = UVProjections.Original;

        public Vector3[] vertices;
        public int[] triangles;
        public Vector3[] normals;
        public Vector2[] uv;
        public Color[] vertexColors;
        public Color vertexColor;

        public float uvRotation;
        public Vector2 tiling = Vector2.one;
        public bool uvPreview = false;

        public bool weldVertices = false;

        /// <summary>
        /// The generated mesh.
        /// </summary>
        public Mesh mesh;

        #endregion

#if UNITY_EDITOR
        public bool wireframe = false;
        private void OnDrawGizmosSelected()
        {
            if (wireframe == true && meshfilter.sharedMesh != null)
            {
                Gizmos.DrawWireMesh(meshfilter.sharedMesh, 0, transform.localPosition);
            }
        }

#endif

        #region Main Methods:

        public void BuildMesh()
        {
            if (meshfilter == null)
            {
                meshfilter = GetComponent<MeshFilter>();
            }

            if (mesh == null)
            {
                mesh = new Mesh();
            }

            mesh.Clear();
            InitializeMesh();
            if (weldVertices == true)
            {
                WeldVertices(ref vertices, ref triangles);
            }
            ModifyVertices();
            UnwrapMesh();
            // NormalizeUVs(ref vertices, ref triangles, ref uv);
            ModifyUVs();
            if (uvPreview == true && uv.Length > 0)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = uv[i];
                }
            }
            mesh.SetVertices(vertices);
            vertexColors = new Color[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertexColors[i] = vertexColor;
            }
            mesh.SetColors(vertexColors);
            mesh.SetTriangles(triangles, 0);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uv);
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
            mesh.Optimize();
            meshfilter.sharedMesh = mesh;
        }

        private void InitializeMesh()
        {
            switch (meshType)
            {
                case MeshTypes.Asset:

                    if (assetMesh != null)
                    {
                        vertices = assetMesh.vertices;
                        triangles = assetMesh.triangles;
                        normals = assetMesh.normals;
                        uv = assetMesh.uv;
                        vertexColors = assetMesh.colors;
                    }

                    break;

                case MeshTypes.Cube:

                    CreateCube();

                    break;

                case MeshTypes.Cylinder:

                    CreateCylinder();

                    break;

                case MeshTypes.Cone:

                    CreateCone();

                    break;

                case MeshTypes.Icosahedron:

                    CreateIcosahedron();

                    break;

                case MeshTypes.Plane:

                    CreatePlane();

                    break;

                case MeshTypes.Sphere:

                    CreateSphere();

                    break;

                case MeshTypes.Torus:

                    CreateTorus();

                    break;

                case MeshTypes.Custom:

                    if (customVertices.Length == 0)
                    {
                        customVertices = new Vector3[1];
                    }
                    vertices = customVertices;
                    if (customTriangles.Length < 3)
                    {
                        customTriangles = new int[3];
                    }
                    triangles = customTriangles;
                    normals = new Vector3[vertices.Length];
                    vertexColors = new Color[vertices.Length];
                    uv = new Vector2[vertices.Length];

                    break;
            }
        }

        private void ModifyVertices()
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertexRotation != Vector3.zero)
                {
                    vertices[i] = (Quaternion.Euler(vertexRotation) * vertices[i]).normalized;
                }

                if (vertexOffset != Vector3.zero)
                {
                    vertices[i] += vertexOffset;
                }

                if (vertexScale != Vector3.one)
                {
                    vertices[i] = new Vector3(vertices[i].x * vertexScale.x, vertices[i].y * vertexScale.y, vertices[i].z * vertexScale.z);
                }

                // Clamp to bounds:
                vertices[i] = new Vector3(Mathf.Clamp(vertices[i].x, xBounds.x, xBounds.y), Mathf.Clamp(vertices[i].y, yBounds.x, yBounds.y), Mathf.Clamp(vertices[i].z, zBounds.x, zBounds.y));

                if (sphereify == true)
                {
                    normals[i] = vertices[i].normalized;
                    vertices[i] = normals[i] * sphereifyRadius;
                }

                if (noiseStrength != 0)
                {
                    vertices[i] = vertices[i] * (Mathf.PerlinNoise(i * UnityEngine.Random.Range(0, noiseStrength / 1000), i * UnityEngine.Random.Range(0, noiseStrength / 1000)));
                }
            }

        }

        private void ModifyUVs()
        {
            for (int i = 0; i < uv.Length; i++)
            {
                if (uvRotation != 0)
                {
                    uv[i] = new Vector2(uv[i].x * Mathf.Cos(uvRotation) - uv[i].y * Mathf.Sin(uvRotation), uv[i].x * Mathf.Sin(uvRotation) + uv[i].y * Mathf.Cos(uvRotation));
                }

                uv[i] = Vector2.Scale(uv[i], tiling);
            }
        }

        private void UnwrapMesh()
        {
            switch (unwrapMode)
            {
                case UVProjections.Original:

                    break;

                case UVProjections.Planar:

                    PlanarProject(ref uv, vertices, Vector3.forward);

                    break;

                case UVProjections.Spherical:

                    SphericalProject(ref uv, vertices, false);

                    break;

                case UVProjections.SphericalNormalized:

                    SphericalProject(ref uv, vertices, true);

                    break;

                case UVProjections.Box:

                    BoxProject(ref uv, normals, vertices);

                    break;

                case UVProjections.Polar:

                    PolarProject(ref uv, vertices);

                    break;

                case UVProjections.Cylindrical:

                    CylindricalProject(ref uv, vertices);

                    break;

                case UVProjections.Conical:

                    ConicalProject(ref uv, vertices);

                    break;

                case UVProjections.None:

                    uv = new Vector2[0];

                    break;

                default:

                    break;
            }
        }

        #endregion

        #region Mesh Primitives:

        private void CreateCube()
        {
            widthSegments = Mathf.Max(2, widthSegments);
            heightSegments = Mathf.Max(2, heightSegments);
            depthSegments = Mathf.Max(2, depthSegments);

            List<Vector3> verts = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            float hw = width * 0.5f;
            float hh = height * 0.5f;
            float hd = depth * 0.5f;

            if (backSide == true)
            {
                // back
                CalculatePlane(verts, uvs, tris, Vector3.forward * -hd, Vector3.right * width, Vector3.up * height, frontUVOffset, widthSegments, heightSegments);
            }

            if (rightSide == true)
            {
                // right
                CalculatePlane(verts, uvs, tris, Vector3.right * hw, Vector3.forward * depth, Vector3.up * height, rightUVOffset, depthSegments, heightSegments);
            }

            if (frontSide == true)
            {
                // front
                CalculatePlane(verts, uvs, tris, Vector3.forward * hd, Vector3.left * width, Vector3.up * height, backUVOffset, widthSegments, heightSegments);
            }

            if (leftSide == true)
            {
                // left
                CalculatePlane(verts, uvs, tris, Vector3.right * -hw, Vector3.back * depth, Vector3.up * height, leftUVOffset, depthSegments, heightSegments);
            }

            if (top == true)
            {
                // top
                CalculatePlane(verts, uvs, tris, Vector3.up * hh, Vector3.right * width, Vector3.forward * depth, topUVOffset, widthSegments, depthSegments);
            }

            if (bottom == true)
            {
                // bottom
                CalculatePlane(verts, uvs, tris, Vector3.up * -hh, Vector3.right * width, Vector3.back * depth, bottomUVOffset, widthSegments, depthSegments);
            }


            vertices = verts.ToArray();
            uv = uvs.ToArray();
            triangles = tris.ToArray();
            normals = new Vector3[vertices.Length];
        }
        private void CreateCone()
        {
            subdivisions = Mathf.Max(4, subdivisions);
            vertices = new Vector3[subdivisions + 2];
            uv = new Vector2[vertices.Length];
            triangles = new int[(subdivisions * 2) * 3];

            vertices[0] = Vector3.zero;
            uv[0] = new Vector2(0.5f, 0f);
            for (int i = 0, n = subdivisions - 1; i < subdivisions; i++)
            {
                float ratio = (float)i / n;
                float r = ratio * (Mathf.PI * 2f);
                float x = Mathf.Cos(r) * radius;
                float z = Mathf.Sin(r) * radius;
                vertices[i + 1] = new Vector3(x, 0f, z);
                uv[i + 1] = new Vector2(ratio, 0f);
            }
            vertices[subdivisions + 1] = new Vector3(0f, height, 0f);
            uv[subdivisions + 1] = new Vector2(0.5f, 1f);

            // Bottom
            if (bottom == true)
            {
                for (int i = 0, n = subdivisions - 1; i < n; i++)
                {
                    int offset = i * 3;
                    triangles[offset] = 0;
                    triangles[offset + 1] = i + 1;
                    triangles[offset + 2] = i + 2;
                }
            }

            // Sides
            int bottomOffset = subdivisions * 3;
            for (int i = 0, n = subdivisions - 1; i < n; i++)
            {
                int offset = i * 3 + bottomOffset;
                triangles[offset] = i + 1;
                triangles[offset + 1] = subdivisions + 1;
                triangles[offset + 2] = i + 2;
            }

            normals = new Vector3[vertices.Length];
        }

        private void CreateIcosahedron()
        {
            radius = Mathf.Max(Mathf.Epsilon, radius);
            subdivisions = Mathf.Min(4, subdivisions);

            List<Vector3> verts = new List<Vector3>();
            Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

            // create 12 vertices of a icosahedron
            float t = (1f + Mathf.Sqrt(5f)) / 2f;

            verts.Add(new Vector3(-1f, t, 0f).normalized * radius);
            verts.Add(new Vector3(1f, t, 0f).normalized * radius);
            verts.Add(new Vector3(-1f, -t, 0f).normalized * radius);
            verts.Add(new Vector3(1f, -t, 0f).normalized * radius);

            verts.Add(new Vector3(0f, -1f, t).normalized * radius);
            verts.Add(new Vector3(0f, 1f, t).normalized * radius);
            verts.Add(new Vector3(0f, -1f, -t).normalized * radius);
            verts.Add(new Vector3(0f, 1f, -t).normalized * radius);

            verts.Add(new Vector3(t, 0f, -1f).normalized * radius);
            verts.Add(new Vector3(t, 0f, 1f).normalized * radius);
            verts.Add(new Vector3(-t, 0f, -1f).normalized * radius);
            verts.Add(new Vector3(-t, 0f, 1f).normalized * radius);


            // create 20 triangles of the icosahedron
            List<TriangleIndices> faces = new List<TriangleIndices>();

            // 5 faces around point 0
            faces.Add(new TriangleIndices(0, 11, 5)); // -X
            faces.Add(new TriangleIndices(0, 5, 1)); // Z
            faces.Add(new TriangleIndices(0, 1, 7)); // Z
            faces.Add(new TriangleIndices(0, 7, 10)); // -X
            faces.Add(new TriangleIndices(0, 10, 11)); // -X

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(1, 5, 9)); // Z
            faces.Add(new TriangleIndices(5, 11, 4)); // -X?
            faces.Add(new TriangleIndices(11, 10, 2)); // -X
            faces.Add(new TriangleIndices(10, 7, 6)); // -X
            faces.Add(new TriangleIndices(7, 1, 8)); // X/Y

            // 5 faces around point 3
            faces.Add(new TriangleIndices(3, 9, 4)); // -Y
            faces.Add(new TriangleIndices(3, 4, 2)); // -Y
            faces.Add(new TriangleIndices(3, 2, 6)); // -Z
            faces.Add(new TriangleIndices(3, 6, 8)); // -Z
            faces.Add(new TriangleIndices(3, 8, 9)); // X

            // 5 adjacent faces 
            faces.Add(new TriangleIndices(4, 9, 5)); // X/Z?
            faces.Add(new TriangleIndices(2, 4, 11)); // -X
            faces.Add(new TriangleIndices(6, 2, 10)); // -X
            faces.Add(new TriangleIndices(8, 6, 7)); // Y
            faces.Add(new TriangleIndices(9, 8, 1)); // X

            // refine triangles
            for (int i = 0; i < subdivisions; i++)
            {
                List<TriangleIndices> faces2 = new List<TriangleIndices>();
                foreach (TriangleIndices tri in faces)
                {
                    // replace triangle by 4 triangles
                    int a = getMiddlePoint(tri.v1, tri.v2, ref verts, ref middlePointIndexCache, radius);
                    int b = getMiddlePoint(tri.v2, tri.v3, ref verts, ref middlePointIndexCache, radius);
                    int c = getMiddlePoint(tri.v3, tri.v1, ref verts, ref middlePointIndexCache, radius);

                    faces2.Add(new TriangleIndices(tri.v1, a, c));
                    faces2.Add(new TriangleIndices(tri.v2, b, a));
                    faces2.Add(new TriangleIndices(tri.v3, c, b));
                    faces2.Add(new TriangleIndices(a, b, c));
                }
                faces = faces2;
            }

            vertices = verts.ToArray();
            uv = new Vector2[vertices.Length];

            // Create UVs:
            for (int i = 0; i < vertices.Length; i++)
            {
                // Convert the vertex from spherical coordinates to Cartesian coordinates
                float x = Mathf.Sin(vertices[i].y) * Mathf.Cos(vertices[i].x);
                float y = Mathf.Sin(vertices[i].y) * Mathf.Sin(vertices[i].x);
                float z = Mathf.Cos(vertices[i].y);

                if (subdivisions % 2 == 0)
                {
                    uv[i] = new Vector2(x * 0.5f + 0.5f, z * 0.5f + 0.5f);
                }
                else
                {
                    uv[i] = new Vector2(y * 0.5f + 0.5f, z * 0.5f + 0.5f);
                }
            }

            List<int> triList = new List<int>();
            for (int i = 0; i < faces.Count; i++)
            {
                triList.Add(faces[i].v1);
                triList.Add(faces[i].v2);
                triList.Add(faces[i].v3);
            }

            triangles = triList.ToArray();
            normals = vertices;
        }

        private void CreateCylinder()
        {
            radialSegments = Mathf.Max(3, radialSegments);
            heightSegments = Mathf.Max(1, heightSegments);

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float pi2 = Mathf.PI * 2f;
            float hh = height * 0.5f;

            float invR = 1f / radialSegments;
            float invH = 1f / heightSegments;
            float uy = 1f * height * invH;

            Action<float, float> AddWall = (float fx, float fy) =>
            {
                float rad = fx * pi2;
                var py = fy * height - hh;

                float c = Mathf.Cos(rad), s = Mathf.Sin(rad);
                var top = new Vector3(c * radius, py + uy, s * radius);
                var bottom = new Vector3(c * radius, py, s * radius);

                verts.Add(top); uvs.Add(new Vector2(fx, fy + invH));
                verts.Add(bottom); uvs.Add(new Vector2(fx, fy));
                norms.Add(new Vector3(c, 0f, s)); norms.Add(new Vector3(c, 0f, s));
            };

            for (int y = 0; y < heightSegments; y++)
            {
                var fy = 1f * y * invH;

                for (int x = 0; x < radialSegments; x++)
                {
                    float fx = 1f * x * invR;
                    int idx = verts.Count;

                    AddWall(fx, fy);

                    int a = idx, b = idx + 1, c = idx + 2, d = idx + 3;
                    tris.Add(a);
                    tris.Add(c);
                    tris.Add(b);

                    tris.Add(c);
                    tris.Add(d);
                    tris.Add(b);
                }

                AddWall(1f, fy);
            }

            if (top == true)
            {
                int top = verts.Count;
                verts.Add(new Vector3(0f, hh, 0f)); // top
                norms.Add(Vector3.up);
                uvs.Add(new Vector2(0.5f, 1f));

                // top side
                for (int x = 0; x <= radialSegments; x++)
                {
                    var fx = 1f * x * invR;
                    var rad = fx * pi2;
                    var v = new Vector3(Mathf.Cos(rad) * radius, hh, Mathf.Sin(rad) * radius);
                    verts.Add(v);
                    norms.Add(Vector3.up);
                    var fy = 0.5f + topUVOffset.y;
                    fx += topUVOffset.x;
                    uvs.Add(new Vector2(fx * Mathf.Cos(topUVOffset.z) - fy * Mathf.Sin(topUVOffset.z), fx * Mathf.Sin(topUVOffset.z) + fy * Mathf.Cos(topUVOffset.z)));
                }

                for (int x = 0; x < radialSegments; x++)
                {
                    tris.Add(top);
                    tris.Add(top + 1 + (x + 1) % radialSegments);
                    tris.Add(top + 1 + x);
                }
            }

            if (bottom == true)
            {
                int bottom = verts.Count;
                verts.Add(new Vector3(0f, -hh, 0f)); // bottom
                norms.Add(Vector3.down);
                uvs.Add(new Vector2(0.5f, 0f));

                // bottom side
                for (int x = 0; x <= radialSegments; x++)
                {
                    var fx = 1f * x * invR;
                    var rad = fx * pi2;
                    var v = new Vector3(Mathf.Cos(rad) * radius, -hh, Mathf.Sin(rad) * radius);
                    verts.Add(v);
                    norms.Add(Vector3.down);
                    fx += bottomUVOffset.x;
                    var fy = 0.5f + bottomUVOffset.y;
                    uvs.Add(new Vector2(fx * Mathf.Cos(bottomUVOffset.z) - fy * Mathf.Sin(bottomUVOffset.z), fx * Mathf.Sin(bottomUVOffset.z) + fy * Mathf.Cos(bottomUVOffset.z)));
                }

                for (int x = 0; x < radialSegments; x++)
                {
                    tris.Add(bottom);
                    tris.Add(bottom + 1 + x);
                    tris.Add(bottom + 1 + (x + 1) % radialSegments);
                }
            }

            vertices = verts.ToArray();
            normals = norms.ToArray();
            uv = uvs.ToArray();
            triangles = tris.ToArray();
        }

        private void CreatePlane()
        {
            widthSegments = Mathf.Max(1, widthSegments);
            heightSegments = Mathf.Max(1, heightSegments);

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float hinv = 1f / (heightSegments - 1);
            float winv = 1f / (widthSegments - 1);
            for (int y = 0; y < heightSegments; y++)
            {
                float ry = y * hinv;
                for (int x = 0; x < widthSegments; x++)
                {
                    float rx = x * winv;
                    verts.Add(new Vector3((rx - 0.5f) * width, (ry - 0.5f) * height, 0));
                    uvs.Add(new Vector2(rx, ry));
                }

                if (y < heightSegments - 1)
                {
                    var offset = y * widthSegments;
                    for (int x = 0, n = widthSegments - 1; x < n; x++)
                    {
                        tris.Add(offset + x);
                        tris.Add(offset + x + widthSegments);
                        tris.Add(offset + x + 1);

                        tris.Add(offset + x + 1);
                        tris.Add(offset + x + widthSegments);
                        tris.Add(offset + x + 1 + widthSegments);
                    }
                }
            }

            vertices = verts.ToArray();
            uv = uvs.ToArray();
            triangles = tris.ToArray();
            normals = new Vector3[vertices.Length];
        }

        private void CreateTorus()
        {
            radialSegments = Mathf.Max(2, radialSegments);
            widthSegments = Mathf.Max(3, widthSegments);

            var verts = new List<Vector3>();
            var norms = new List<Vector3>();
            var uvs = new List<Vector2>();
            var tris = new List<int>();

            var tInterval = ((Mathf.PI * 2) - start);

            for (int y = 0; y <= radialSegments; y++)
            {
                var v = 1f * y / radialSegments * Mathf.PI * 2;
                for (int x = 0; x <= widthSegments; x++)
                {
                    var u = start + 1f * x / widthSegments * tInterval;

                    var vertex = new Vector3(
                        (radius + depth * Mathf.Cos(v)) * Mathf.Cos(u),
                        (radius + depth * Mathf.Cos(v)) * Mathf.Sin(u),
                        depth * Mathf.Sin(v)
                    );
                    verts.Add(vertex);

                    var center = new Vector3(
                        radius * Mathf.Cos(u),
                        radius * Mathf.Sin(u),
                        0f
                    );
                    norms.Add((vertex - center).normalized);
                    uvs.Add(new Vector2(1f * x / widthSegments, 1f * y / radialSegments));
                }
            }

            for (int y = 1; y <= radialSegments; y++)
            {
                for (int x = 1; x <= widthSegments; x++)
                {
                    var a = (widthSegments + 1) * y + x - 1;
                    var b = (widthSegments + 1) * (y - 1) + x - 1;
                    var c = (widthSegments + 1) * (y - 1) + x;
                    var d = (widthSegments + 1) * y + x;
                    tris.Add(a); tris.Add(b); tris.Add(d);
                    tris.Add(b); tris.Add(c); tris.Add(d);
                }
            }

            vertices = verts.ToArray();
            uv = uvs.ToArray();
            triangles = tris.ToArray();
            normals = norms.ToArray();
        }

        private void CreateSphere()
        {
            vertices = new Vector3[(widthSegments + 1) * heightSegments + 2];

            float pi2 = Mathf.PI * 2f;

            vertices[0] = Vector3.up * radius;
            for (int lat = 0; lat < heightSegments; lat++)
            {
                float a1 = Mathf.PI * (lat + 1) / (heightSegments + 1);
                float sin = Mathf.Sin(a1);
                float cos = Mathf.Cos(a1);

                for (int lon = 0; lon <= widthSegments; lon++)
                {
                    float a2 = pi2 * (lon == widthSegments ? 0 : lon) / widthSegments;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);
                    vertices[lon + lat * (widthSegments + 1) + 1] = new Vector3(sin * cos2, cos, sin * sin2) * radius;
                }
            }
            vertices[vertices.Length - 1] = Vector3.up * -radius;

            int len = vertices.Length;

            uv = new Vector2[len];
            uv[0] = Vector2.up;
            uv[uv.Length - 1] = Vector2.zero;
            for (int lat = 0; lat < heightSegments; lat++)
            {
                for (int lon = 0; lon <= widthSegments; lon++)
                {
                    uv[lon + lat * (widthSegments + 1) + 1] = new Vector2((float)lon / widthSegments, 1f - (float)(lat + 1) / (heightSegments + 1));
                }
            }

            triangles = new int[len * 2 * 3];

            int acc = 0;
            if (top == true)
            {
                // top cap
                for (int lon = 0; lon < widthSegments; lon++)
                {
                    triangles[acc++] = lon + 2;
                    triangles[acc++] = lon + 1;
                    triangles[acc++] = 0;
                }
            }


            // middle
            for (int lat = 0; lat < heightSegments - 1; lat++)
            {
                for (int lon = 0; lon < widthSegments; lon++)
                {
                    int current = lon + lat * (widthSegments + 1) + 1;
                    int next = current + widthSegments + 1;

                    triangles[acc++] = current;
                    triangles[acc++] = current + 1;
                    triangles[acc++] = next + 1;

                    triangles[acc++] = current;
                    triangles[acc++] = next + 1;
                    triangles[acc++] = next;
                }
            }

            // bottom cap
            if (bottom == true)
            {
                for (int lon = 0; lon < widthSegments; lon++)
                {
                    triangles[acc++] = len - 1;
                    triangles[acc++] = len - (lon + 2) - 1;
                    triangles[acc++] = len - (lon + 1) - 1;
                }
            }

            normals = new Vector3[vertices.Length];
        }

        #endregion

        #region UV Methods:

        public void PolarProject(ref Vector2[] uvs, Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                uvs[i] = new Vector2(Mathf.Atan2(points[i].z, points[i].x) / (2 * Mathf.PI) + 0.5f, points[i].y * 0.5f + 0.5f);
            }
        }

        public void SphericalProject(ref Vector2[] uvs, Vector3[] points, bool normalize)
        {
            if (normalize == true)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    Vector3 normVertex = points[i].normalized;
                    uvs[i] = new Vector2(0.5f + Mathf.Atan2(normVertex.z, normVertex.x) / (2 * Mathf.PI), 0.5f - Mathf.Asin(normVertex.y) / Mathf.PI);
                }
            }
            else
            {
                for (int i = 0; i < points.Length; i++)
                {
                    uvs[i] = new Vector2(Mathf.Atan2(points[i].y, points[i].z) / (-2f * Mathf.PI), Mathf.Asin(points[i].x) / Mathf.PI + 0.5f);
                }
            }
        }

        public void CylindricalProject(ref Vector2[] uvs, Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                uvs[i] = new Vector2(0.5f + Mathf.Atan2(points[i].z, points[i].x) / (2 * Mathf.PI), points[i].y + 0.5f);
            }
        }

        public void ConicalProject(ref Vector2[] uvs, Vector3[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 normVertex = points[i].normalized;

                uvs[i] = new Vector2(0.5f + Mathf.Atan2(normVertex.z, normVertex.x) / (2 * Mathf.PI), Mathf.Pow(normVertex.y + 1, 0.5f));
            }
        }

        public void PlanarProject(ref Vector2[] uvs, Vector3[] points, Vector3 direction)
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 projectedVertex = Vector3.ProjectOnPlane(points[i], direction);

                uvs[i] = new Vector2(projectedVertex.x, projectedVertex.y);
            }
        }

        public void BoxProject(ref Vector2[] uvs, Vector3[] norms, Vector3[] verts)
        {
            for (var i = 0; i < uvs.Length; i++)
            {
                if (Mathf.Abs(norms[i].x) > Mathf.Abs(norms[i].y) && Mathf.Abs(norms[i].x) > Mathf.Abs(norms[i].z))
                {
                    uvs[i] = new Vector2(verts[i].y, verts[i].z);
                }

                if (Mathf.Abs(norms[i].y) > Mathf.Abs(norms[i].x) && Mathf.Abs(norms[i].y) > Mathf.Abs(norms[i].z))
                {
                    uvs[i] = new Vector2(verts[i].x, verts[i].z);
                }

                if (Mathf.Abs(norms[i].z) > Mathf.Abs(norms[i].x) && Mathf.Abs(norms[i].z) > Mathf.Abs(norms[i].y))
                {
                    uvs[i] = new Vector2(verts[i].x, verts[i].y);
                }
            }
        }

        public void NormalizeUVs(ref Vector3[] verts, ref int[] tris, ref Vector2[] uvs, float scaleFactor = 0.5f)
        {
            //https://gamedev.stackexchange.com/questions/139950/generating-a-uv-map-for-a-procedural-mesh 

            // Iterate over each face (here assuming triangles)
            for (int index = 0; index < tris.Length; index += 3)
            {
                // Get the three vertices bounding this triangle.
                Vector3 v1 = verts[triangles[index]];
                Vector3 v2 = verts[triangles[index + 1]];
                Vector3 v3 = verts[triangles[index + 2]];

                // Compute a vector perpendicular to the face.
                Vector3 normal = Vector3.Cross(v3 - v1, v2 - v1);

                // Form a rotation that points the z+ axis in this perpendicular direction.
                // Multiplying by the inverse will flatten the triangle into an xy plane.
                Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(normal));

                // Assign the uvs, applying a scale factor to control the texture tiling.
                uvs[triangles[index]] = (Vector2)(rotation * v1) * scaleFactor;
                uvs[triangles[index + 1]] = (Vector2)(rotation * v2) * scaleFactor;
                uvs[triangles[index + 2]] = (Vector2)(rotation * v3) * scaleFactor;
            }
        }

        #endregion

        #region Helper Methods:

        private void CalculatePlane(List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, Vector3 origin, Vector3 right, Vector3 up, Vector3 uvOffset, int rSegments = 2, int uSegments = 2)
        {
            float rInv = 1f / (rSegments - 1);
            float uInv = 1f / (uSegments - 1);

            int triangleOffset = vertices.Count;

            for (int y = 0; y < uSegments; y++)
            {
                float ru = y * uInv;
                for (int x = 0; x < rSegments; x++)
                {
                    float rr = x * rInv;
                    vertices.Add(origin + right * (rr - 0.5f) + up * (ru - 0.5f));
                    rr += uvOffset.x;
                    ru += uvOffset.y;
                    uvs.Add(new Vector2(rr * Mathf.Cos(uvOffset.z) - ru * Mathf.Sin(uvOffset.z), rr * Mathf.Sin(uvOffset.z) + ru * Mathf.Cos(uvOffset.z)));
                    ru -= uvOffset.y;
                }

                if (y < uSegments - 1)
                {
                    var offset = y * rSegments + triangleOffset;
                    for (int x = 0, n = rSegments - 1; x < n; x++)
                    {
                        triangles.Add(offset + x);
                        triangles.Add(offset + x + rSegments);
                        triangles.Add(offset + x + 1);

                        triangles.Add(offset + x + 1);
                        triangles.Add(offset + x + rSegments);
                        triangles.Add(offset + x + 1 + rSegments);
                    }
                }
            }

        }

        private int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
        {
            // first check if we have it already
            bool firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            long greaterIndex = firstIsSmaller ? p2 : p1;
            long key = (smallerIndex << 32) + greaterIndex;

            int ret;
            if (cache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it
            Vector3 point1 = vertices[p1];
            Vector3 point2 = vertices[p2];
            Vector3 middle = new Vector3
            (
                (point1.x + point2.x) / 2f,
                (point1.y + point2.y) / 2f,
                (point1.z + point2.z) / 2f
            );

            // add vertex makes sure point is on unit sphere
            int i = vertices.Count;
            vertices.Add(middle.normalized * radius);

            // store it, return index
            cache.Add(key, i);

            return i;
        }

        public static void WeldVertices(ref Vector3[] vertices, ref int[] triangles, float threshold = 0.001f)
        {
            List<int>[] vertexGroups = new List<int>[vertices.Length];

            // Initialize vertex groups
            for (int i = 0; i < vertexGroups.Length; i++)
            {
                vertexGroups[i] = new List<int>();
                vertexGroups[i].Add(i);
            }

            // Iterate over all vertices and group them based on proximity
            for (int i = 0; i < vertices.Length; i++)
            {
                for (int j = i + 1; j < vertices.Length; j++)
                {
                    if (Vector3.Distance(vertices[i], vertices[j]) < threshold)
                    {
                        // Add the vertex to the group
                        vertexGroups[i].Add(j);
                        // Merge the groups
                        vertexGroups[j] = vertexGroups[i];
                    }
                }
            }

            // Create a new array of vertices based on the vertex groups
            Vector3[] newVertices = new Vector3[vertexGroups.Length];
            for (int i = 0; i < vertexGroups.Length; i++)
            {
                if (vertexGroups[i].Count > 0)
                {
                    Vector3 avg = Vector3.zero;
                    foreach (int index in vertexGroups[i])
                    {
                        avg += vertices[index];
                    }
                    avg /= vertexGroups[i].Count;
                    newVertices[i] = avg;
                }
            }

            // Create a new array of triangles based on the updated indices
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = vertexGroups[triangles[i]][0];
            }

            // Update the arrays with the new vertices and triangles
            vertices = newVertices;
        }

        #endregion

        public enum MeshTypes { Asset, Cube, Cylinder, Cone, Icosahedron, Plane, Sphere, Torus, Custom }

        public enum UVProjections { Original, Planar, Spherical, SphericalNormalized, Box, Polar, Cylindrical, Conical, None }

        private struct TriangleIndices
        {
            public int v1;
            public int v2;
            public int v3;

            public TriangleIndices(int v1, int v2, int v3)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
            }
        }
    }
}
