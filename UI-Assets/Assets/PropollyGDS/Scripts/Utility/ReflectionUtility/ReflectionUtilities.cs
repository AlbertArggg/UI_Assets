using System;
using System.Linq;

public static class ReflectionUtilities
{
    /// <summary>
    /// Checks if a type with the specified name exists in the currently loaded assemblies.
    /// </summary>
    /// <param name="typeName">The name of the type to search for.</param>
    /// <returns>true if the type exists; otherwise, false.</returns>
    public static bool TypeExists(string typeName)
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Any(type => type.Name.Equals(typeName, StringComparison.Ordinal));
    }
}