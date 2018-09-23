using System;

namespace CodeFirst_EF.Security
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HashAttribute : Attribute
    {
    }
}