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
    public static class RendererActions
    {
        public static void RandomizeBlendshapes(ref SkinnedMeshRenderer skin, int[] indices, float min, float max)
        {
            foreach (int i in indices)
            {
                if(i <= skin.sharedMesh.blendShapeCount)
                {
                    skin.SetBlendShapeWeight(i, Random.Range(min, max));
                }
            }
        }
        public static void SetMaterialColor(ref Material mat, string name, Color col)
        {
            mat.SetColor(name, col);
        }
        public static void SetMaterialColorRandom(ref Material mat, string name, List<Color> colors)
        {
            mat.SetColor(name, colors[Random.Range(0, colors.Count)]);
        }
    }

    [SRName("Renderer/Toggle Visibility")]
    public class ToggleRendererVisibilityAction : ActionModule
    {
        public Renderer rend;
        public override ActionEvent Invoke() { if (rend != null) { rend.enabled = !rend.enabled; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Renderer/Skinned Mesh/Set Blendshape Weight")]
    public class SkinnedMeshSetBlendshapeAction : ActionModule
    {
        public SkinnedMeshRenderer skin;
        [Min(0)]
        public int index = 0;
        [Range(0,100)]
        public float weight;
        public override ActionEvent Invoke() { if (skin != null && index <= skin.sharedMesh.blendShapeCount) { skin.SetBlendShapeWeight(index, weight); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Renderer/Skinned Mesh/Randomize Blendshape Weights")]
    public class SkinnedMeshRandomizeBlendshapesAction : ActionModule
    {
        public SkinnedMeshRenderer skin;
        public int[] indices;
        [Range(0, 100)]
        public float min, max;
        public override ActionEvent Invoke() { if (skin != null) { RendererActions.RandomizeBlendshapes(ref skin, indices, min, max); return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Renderer/Skinned Mesh/Animate Blendshape PingPong")]
    public class SkinnedMeshAnimateBlendshapePingPongAction : ActionModule
    {
        [Message("Use this action in an update loop.")]
        public SkinnedMeshRenderer skin;
        public string blendshapeName;
        [Range(0, 100)]
        public float min = 0, max = 100;
        public float speed = 1f;
        [Tooltip("If true the next action wont invoke until this action is complete.")]
        public bool holdUntilComplete = false;

        private int index = 0;
        private bool initialized = false;
        private float currentWeight = 0;
        private bool returning = false;
        public override ActionEvent Invoke() 
        { 
            if (skin != null || blendshapeName == string.Empty) 
            {
                if (initialized == false)
                {
                    index = skin.sharedMesh.GetBlendShapeIndex(blendshapeName);
                    skin.SetBlendShapeWeight(index, min);
                    currentWeight = skin.GetBlendShapeWeight(index);
                    initialized = true;
                }
                if(returning == true)
                {
                    currentWeight -= Time.deltaTime * (100 * speed);
                    skin.SetBlendShapeWeight(index, Mathf.Max(0, currentWeight));
                    if(currentWeight <= 0)
                    {
                        returning = false;
                        if(holdUntilComplete == true)
                        {
                            return ActionEvent.Release;
                        }
                    }
                }
                else
                {
                    currentWeight += Time.deltaTime * (100 * speed);
                    skin.SetBlendShapeWeight(index, Mathf.Min(100, currentWeight));
                    if (currentWeight >= 100)
                    {
                        returning = true;
                    }
                }
                if(holdUntilComplete == true)
                {
                    return ActionEvent.Hold;
                }
                else
                {
                    return ActionEvent.Continue;
                } 
            }
            else return ActionEvent.Error; }
    }

    [SRName("Renderer/Set Material Color")]
    public class SetMaterialColorAction : ActionModule
    {
        public Renderer rend;
        public int[] indices;
        public string propertyName;
        public Color color;
        public override ActionEvent Invoke() 
        { 
            if (rend != null) 
            { 
                foreach(int i in indices)
                {
                    if(i < rend.materials.Length)
                    {
                        RendererActions.SetMaterialColor(ref rend.materials[i], propertyName, color);
                    }
                }
                return ActionEvent.Continue; 
            } 
            else return ActionEvent.Error; 
        }
    }

    [SRName("Renderer/Randomize Material Color")]
    public class RendererRandomizeMaterialAction : ActionModule
    {
        public Renderer rend;
        public int[] indices;
        public string propertyName;
        public List<Color> colors;
        public override ActionEvent Invoke()
        {
            if (rend != null)
            {
                foreach (int i in indices)
                {
                    if (i < rend.materials.Length)
                    {
                        RendererActions.SetMaterialColorRandom(ref rend.materials[i], propertyName, colors);
                    }
                }
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }

    [SRName("Renderer/Load Blendshape PlayPref")]
    public class SkinnedMeshLoadBlendshapePlayerPrefAction : ActionModule
    {
        [SerializeReference] private SkinnedMeshRenderer skin;
        [SerializeReference, Min(0)] private int blendshapeIndex = 0;
        [SerializeReference] private string playerPref;
        public override ActionEvent Invoke()
        {
            if (skin != null && !string.IsNullOrEmpty(playerPref))
            {
                skin.SetBlendShapeWeight(blendshapeIndex, PlayerPrefs.GetFloat(playerPref));
                return ActionEvent.Continue;
            }
            else return ActionEvent.Error;
        }
    }
}