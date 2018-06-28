using System;
using System.Linq;
using System.Reflection;

namespace Niles
{
    internal static class Utilities
    {
        public static Type[] GetTypesWithAttribute<T>() where T : Attribute
        {
            return Assembly.GetAssembly(typeof(Bot)).GetTypes().Where(_type => _type.GetCustomAttributes(typeof(T), true).Length > 0).ToArray();
        }
    }
}
