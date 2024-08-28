// This file is unlicensed code with or without modifications. Provided 'AS IS' without warranty of any kind.

using UnityEditor;
using UnityEngine;

namespace SerializeReferenceEditor.Editor.MissingTypesValidator.ReportFormats
{
    public interface IAssetMissingTypeReport
    {
        void AttachMissingTypes(Object missingObjectContainer, ManagedReferenceMissingType[] missingTypes);
        void Finished();
    }
}