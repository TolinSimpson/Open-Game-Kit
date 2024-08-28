// This file is unlicensed code with or without modifications. Provided 'AS IS' without warranty of any kind.

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace SerializeReferenceEditor.Editor.MissingTypesValidator.Loaders
{
    public interface IAssetsLoader
    {
        bool TryLoadAssetsForCheck([NotNull] List<Object> assets);
    }
}