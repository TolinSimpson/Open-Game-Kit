// This file is unlicensed code with or without modifications. Provided 'AS IS' without warranty of any kind.

using SerializeReferenceEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OGK
{
	public class SRPickerAttribute : SRAttribute
	{
		public SRPickerAttribute() : base()
		{
		}

		public SRPickerAttribute(Type baseType) : base(baseType)
		{
		}

		public SRPickerAttribute(params Type[] types) : base(types)
		{
		}

		/*public override void OnCreate(object instance)
		{
			if (instance is AbstractData)
			{
				((AbstractData)instance).DataName = instance.GetType().Name;
			}
		}*/
	}
}