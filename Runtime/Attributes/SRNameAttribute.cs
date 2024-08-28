// This file is unlicensed code with or without modifications. Provided 'AS IS' without warranty of any kind.

using System;

[AttributeUsage(AttributeTargets.Class)]
public class SRNameAttribute : Attribute
{
    public readonly string FullName;
    public readonly string Name;

    public SRNameAttribute(string fullName)
    {
        FullName = fullName;
        if (!fullName.Contains("/"))
        {
            Name = fullName;
            return;
        }

        var separateName = fullName.Split('/');
        Name = separateName[^1];
    }
}