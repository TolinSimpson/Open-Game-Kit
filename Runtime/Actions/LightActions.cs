using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OGK
{
    [SRName("Light/Set Intensity")]
    public class SetLightIntensity : ActionModule
    {
        [SerializeField] private Light light;
        [SerializeField] private float intensity = 1;

        public override ActionEvent Invoke() { if (light != null) { light.intensity = intensity; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }

    [SRName("Light/Set Color")]
    public class SetLightColor : ActionModule
    {
        [SerializeField] private Light light;
        [SerializeField] private Color color;

        public override ActionEvent Invoke() { if (light != null) { light.color = color; return ActionEvent.Continue; } else return ActionEvent.Error; }
    }
}