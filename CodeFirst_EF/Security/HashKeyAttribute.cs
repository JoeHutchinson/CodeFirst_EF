using System;

namespace CodeFirst_EF.Security
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class HashKeyAttribute : Attribute
    {
    }
}