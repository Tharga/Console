using System;
using System.Reflection;

namespace Tharga.Console.Helpers
{
    internal class AssemblyHelper
    {
        public static string GetAssemblyInfo()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                if (assembly == null) return null;
                return $"{assembly.GetName().Name} (Version {assembly.GetName().Version})";
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}