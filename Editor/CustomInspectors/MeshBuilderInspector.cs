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
using UnityEditor;
using OGK;

namespace OGKEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MeshBuilder))]
    public class MeshBuilderInspector : Editor
    {
        MeshBuilder self;

        SerializedProperty meshType, assetMesh, height, radius, width, depth, subdivisions, heightSegments, widthSegments, depthSegments, radialSegments, customVertices, customTriangles, start;

        SerializedProperty unwrapMode, vertexScale, vertexRotation, vertexOffset, noiseStrength, uvRotation, tiling, uvPreview, wireframe, vertexColor, sphereify, sphereifyRadius, weldVertices;

        SerializedProperty top, bottom, leftSide, rightSide, frontSide, backSide;

        private bool showCopyright, showMeshBuilderHelp;

        SerializedProperty xBounds, yBounds, zBounds;

        public GUIStyle wrappedMiniLabel, wrappedCenteredGreyMiniLabel, richBoldLabel;
        public GUILayoutOption[] thinLine, thickLine;
        SerializedProperty topUVOffset, bottomUVOffset, leftUVOffset, rightUVOffset, frontUVOffset, backUVOffset;

        private bool showCustomTriangles;

        private void OnEnable()
        {
            meshType = serializedObject.FindProperty("meshType");
            assetMesh = serializedObject.FindProperty("assetMesh");
            height = serializedObject.FindProperty("height");
            width = serializedObject.FindProperty("width");
            depth = serializedObject.FindProperty("depth");
            subdivisions = serializedObject.FindProperty("subdivisions");
            radius = serializedObject.FindProperty("radius");
            heightSegments = serializedObject.FindProperty("heightSegments");
            widthSegments = serializedObject.FindProperty("widthSegments");
            depthSegments = serializedObject.FindProperty("depthSegments");
            radialSegments = serializedObject.FindProperty("radialSegments");
            customVertices = serializedObject.FindProperty("customVertices");
            customTriangles = serializedObject.FindProperty("customTriangles");
            start = serializedObject.FindProperty("start");
            unwrapMode = serializedObject.FindProperty("unwrapMode");
            vertexScale = serializedObject.FindProperty("vertexScale");
            vertexRotation = serializedObject.FindProperty("vertexRotation");
            vertexOffset = serializedObject.FindProperty("vertexOffset");
            noiseStrength = serializedObject.FindProperty("noiseStrength");
            xBounds = serializedObject.FindProperty("xBounds");
            yBounds = serializedObject.FindProperty("yBounds");
            zBounds = serializedObject.FindProperty("zBounds");
            top = serializedObject.FindProperty("top");
            bottom = serializedObject.FindProperty("bottom");
            leftSide = serializedObject.FindProperty("leftSide");
            rightSide = serializedObject.FindProperty("rightSide");
            frontSide = serializedObject.FindProperty("frontSide");
            backSide = serializedObject.FindProperty("backSide");
            uvRotation = serializedObject.FindProperty("uvRotation");
            tiling = serializedObject.FindProperty("tiling");
            uvPreview = serializedObject.FindProperty("uvPreview");
            wireframe = serializedObject.FindProperty("wireframe");
            topUVOffset = serializedObject.FindProperty("topUVOffset");
            bottomUVOffset = serializedObject.FindProperty("bottomUVOffset");
            leftUVOffset = serializedObject.FindProperty("leftUVOffset");
            rightUVOffset = serializedObject.FindProperty("rightUVOffset");
            frontUVOffset = serializedObject.FindProperty("frontUVOffset");
            backUVOffset = serializedObject.FindProperty("backUVOffset");
            vertexColor = serializedObject.FindProperty("vertexColor");
            sphereify = serializedObject.FindProperty("sphereify");
            sphereifyRadius = serializedObject.FindProperty("sphereifyRadius");
            weldVertices = serializedObject.FindProperty("weldVertices");
        }

        public override void OnInspectorGUI()
        {
            // Initialize Styles:
            wrappedMiniLabel = new GUIStyle(GUI.skin.label) { wordWrap = true, fontSize = 10 };
            wrappedCenteredGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { wordWrap = true, richText = true };
            richBoldLabel = new GUIStyle(EditorStyles.boldLabel) { richText = true };
            thinLine = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) };
            thickLine = new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(3) };

            self = (MeshBuilder)target;
            serializedObject.Update();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DrawMeshBuilderHelp();

            DrawInitialMeshProperties();

            DrawVertexProperties();

            DrawUVUnwrappProperties();

            DrawMeshDebug();

            EditorGUILayout.EndVertical();
            EditorDecor.DrawCopyrightNotice("https://kitbashery.com/docs/smart-gameobjects", "https://assetstore.unity.com/packages/slug/248930", "https://unity.com/legal/as-terms", "2024", "Tolin Simpson", ref showCopyright);

            if (serializedObject.hasModifiedProperties == true)
            {
                serializedObject.ApplyModifiedProperties();
                self.BuildMesh();
            }
        }

        private void DrawVertexProperties()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("<color=#77C3E5>Vertex Modifiers:</color>", richBoldLabel);
            EditorDecor.DrawHorizontalLine(false);

            EditorGUILayout.PropertyField(weldVertices);

            EditorGUILayout.PropertyField(vertexOffset);
            EditorGUILayout.PropertyField(vertexRotation);
            EditorGUILayout.PropertyField(vertexScale);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bounding Area:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(xBounds);
            EditorGUILayout.PropertyField(yBounds);
            EditorGUILayout.PropertyField(zBounds);
            EditorGUILayout.PropertyField(sphereify);
            if (sphereify.boolValue == true)
            {
                EditorGUILayout.PropertyField(sphereifyRadius);
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(noiseStrength);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(vertexColor);
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
        }

        private void DrawInitialMeshProperties()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("<color=#77C3E5>Initial Mesh:</color>", richBoldLabel);
            EditorDecor.DrawHorizontalLine(false);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(meshType);
            if (EditorGUI.EndChangeCheck())
            {
                if (meshType.enumValueIndex == (int)MeshBuilder.MeshTypes.Cone)
                {
                    if (subdivisions.intValue < 10)
                    {
                        subdivisions.intValue = 10;
                    }
                }
            }

            switch ((MeshBuilder.MeshTypes)meshType.enumValueIndex)
            {
                case MeshBuilder.MeshTypes.Asset:

                    EditorGUILayout.PropertyField(assetMesh);

                    break;

                case MeshBuilder.MeshTypes.Cube:

                    EditorGUILayout.PropertyField(width);
                    EditorGUILayout.PropertyField(height);
                    EditorGUILayout.PropertyField(depth);
                    EditorGUILayout.PropertyField(heightSegments);
                    EditorGUILayout.PropertyField(widthSegments);
                    EditorGUILayout.PropertyField(depthSegments);
                    EditorGUILayout.PropertyField(top);
                    if (top.boolValue == true)
                    {
                        EditorGUILayout.PropertyField(topUVOffset);
                    }
                    EditorGUILayout.PropertyField(bottom);
                    if (bottom.boolValue == true)
                    {
                        EditorGUILayout.PropertyField(bottomUVOffset);
                    }
                    EditorGUILayout.PropertyField(leftSide);
                    if (leftSide.boolValue == true)
                    {
                        EditorGUILayout.PropertyField(leftUVOffset);
                    }
                    EditorGUILayout.PropertyField(rightSide);
                    if (rightSide.boolValue == true)
                    {
                        EditorGUILayout.PropertyField(rightUVOffset);
                    }
                    EditorGUILayout.PropertyField(frontSide);
                    if (frontSide.boolValue == true)
                    {
                        EditorGUILayout.PropertyField(frontUVOffset);
                    }
                    EditorGUILayout.PropertyField(backSide);
                    if (backSide.boolValue == true)
                    {
                        EditorGUILayout.PropertyField(backUVOffset);
                    }

                    break;

                case MeshBuilder.MeshTypes.Cylinder:

                    EditorGUILayout.PropertyField(radius);
                    EditorGUILayout.PropertyField(height);
                    EditorGUILayout.PropertyField(radialSegments);
                    EditorGUILayout.PropertyField(heightSegments);
                    EditorGUILayout.PropertyField(top);
                    if (top.boolValue == true)
                    {
                        EditorGUILayout.PropertyField(topUVOffset);
                    }
                    EditorGUILayout.PropertyField(bottom);
                    if (bottom.boolValue == true)
                    {
                        EditorGUILayout.PropertyField(bottomUVOffset);
                    }

                    break;

                case MeshBuilder.MeshTypes.Cone:

                    EditorGUILayout.PropertyField(subdivisions);
                    EditorGUILayout.PropertyField(radius);
                    EditorGUILayout.PropertyField(height);
                    EditorGUILayout.PropertyField(bottom);

                    break;

                case MeshBuilder.MeshTypes.Icosahedron:

                    EditorGUILayout.PropertyField(radius);
                    EditorGUILayout.PropertyField(subdivisions);

                    break;

                case MeshBuilder.MeshTypes.Plane:

                    EditorGUILayout.PropertyField(width);
                    EditorGUILayout.PropertyField(height);
                    EditorGUILayout.PropertyField(widthSegments);
                    EditorGUILayout.PropertyField(heightSegments);

                    break;

                case MeshBuilder.MeshTypes.Sphere:

                    EditorGUILayout.PropertyField(radius);
                    EditorGUILayout.PropertyField(widthSegments);
                    EditorGUILayout.PropertyField(heightSegments);
                    EditorGUILayout.PropertyField(top);
                    EditorGUILayout.PropertyField(bottom);

                    break;

                case MeshBuilder.MeshTypes.Torus:

                    EditorGUILayout.PropertyField(start);
                    EditorGUILayout.PropertyField(radius);
                    EditorGUILayout.PropertyField(depth);
                    EditorGUILayout.PropertyField(radialSegments);
                    EditorGUILayout.PropertyField(widthSegments);

                    break;

                case MeshBuilder.MeshTypes.Custom:

                    if (customTriangles.FindPropertyRelative("Array.size").hasMultipleDifferentValues == false)
                    {
                        if (customVertices.arraySize == 0)
                        {
                            customVertices.arraySize = 1;
                        }
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(customVertices);
                        EditorGUI.indentLevel--;
                        if (customTriangles.arraySize < 3)
                        {
                            customTriangles.arraySize = 3;
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Multi-object editing is not supported for triangle arrays of unequal size.", MessageType.None);
                    }

                    if (customTriangles.FindPropertyRelative("Array.size").hasMultipleDifferentValues == false)
                    {
                        if (customTriangles.arraySize % 3 != 0)
                        {
                            EditorGUILayout.HelpBox("The amount of triangles must be a multiple of 3!", MessageType.Error);
                        }
                        showCustomTriangles = EditorDecor.DrawFoldout(showCustomTriangles, "Custom Triangles", false);
                        if (showCustomTriangles == true)
                        {
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            for (int i = 0; i < customTriangles.arraySize; i++)
                            {
                                SerializedProperty property = customTriangles.GetArrayElementAtIndex(i);
                                if (property.intValue > customVertices.arraySize)
                                {
                                    property.intValue = customVertices.arraySize;
                                }
                                property.intValue = EditorGUILayout.IntSlider("Element " + i.ToString(), property.intValue, 0, customVertices.arraySize);
                            }
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(string.Empty, EditorStyles.boldLabel);
                            if (GUILayout.Button(string.Empty, "OL Plus", GUILayout.Width(25)))
                            {
                                customTriangles.InsertArrayElementAtIndex(customTriangles.arraySize);
                                customTriangles.InsertArrayElementAtIndex(customTriangles.arraySize);
                                customTriangles.InsertArrayElementAtIndex(customTriangles.arraySize);
                            }
                            if (GUILayout.Button(string.Empty, "OL Minus", GUILayout.Width(25)))
                            {
                                customTriangles.DeleteArrayElementAtIndex(customTriangles.arraySize - 3);
                                customTriangles.DeleteArrayElementAtIndex(customTriangles.arraySize - 2);
                                customTriangles.DeleteArrayElementAtIndex(customTriangles.arraySize - 1);
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                        }
                        if (serializedObject.hasModifiedProperties == true)
                        {
                            serializedObject.ApplyModifiedProperties();
                        }

                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Multi-object editing is not supported for triangle arrays of unequal size.", MessageType.None);
                    }


                    break;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawUVUnwrappProperties()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("<color=#77C3E5>UV Unwrapping:</color>", richBoldLabel);
            EditorDecor.DrawHorizontalLine(false);
            EditorGUILayout.PropertyField(unwrapMode);
            EditorGUILayout.PropertyField(uvRotation);
            EditorGUILayout.PropertyField(tiling);
            EditorGUILayout.PropertyField(uvPreview);

            EditorGUILayout.EndVertical();
        }
        private void DrawMeshDebug()
        {
            if (self.vertices != null && self.triangles != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (self.meshfilter != null)
                {
                    if (self.meshfilter.sharedMesh != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Toggle Wireframe", EditorStyles.miniButton))
                        {
                            wireframe.boolValue = !wireframe.boolValue;
                        }
                        if (GUILayout.Button("Bake Mesh", EditorStyles.miniButton))
                        {
                            string path = EditorUtility.SaveFilePanelInProject("Save mesh asset", self.name + "_mesh", "asset", "Please enter a file name to save the mesh as");
                            if (path.Length != 0)
                            {
                                AssetDatabase.CreateAsset(self.meshfilter.sharedMesh, path);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                                self.meshfilter.sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
                            }
                            GUIUtility.ExitGUI();
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Vertices: " + self.vertices.Length, EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.LabelField("Triangles: " + (self.triangles.Length / 3), EditorStyles.centeredGreyMiniLabel);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    self.meshfilter = self.GetComponent<MeshFilter>();
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }

        private void DrawMeshBuilderHelp()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            GUILayout.Space(7);
            EditorDecor.DrawHorizontalLine(true);
            EditorGUILayout.EndVertical();

            if (GUILayout.Button(EditorGUIUtility.IconContent("_help"), GUIStyle.none, GUILayout.Width(20))) { showMeshBuilderHelp = !showMeshBuilderHelp; }
            EditorGUILayout.EndHorizontal();

            if (showMeshBuilderHelp == true)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box(EditorGUIUtility.IconContent("_help@2x"), GUIStyle.none);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Mesh:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Meshes are comprised of vertices, triangles and UVs where triangles are groups of 3 integers that corralate to a vertex and UVs represent vertices on a 2D plane.", wrappedMiniLabel);
                EditorDecor.DrawHorizontalLine(false);
                EditorGUILayout.LabelField("UV Unwrapping:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("UV unwrapping will approximate positions of vertices on a 2D plane by various projection methods. Stretching may occur when there are not enough vertices along a seam or projected faces are distorted.", wrappedMiniLabel);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }

        [MenuItem("GameObject/Open Game Kit/Mesh Builder")]
        static void CreateMeshBuilder()
        {
            Selection.activeGameObject = new GameObject("Mesh Builder").AddComponent<MeshBuilder>().gameObject;
        }
    }
}