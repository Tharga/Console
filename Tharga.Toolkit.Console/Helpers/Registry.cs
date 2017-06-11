using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class Registry
    {
        public enum RegistryHKey
        {
            CurrentUser = 0,
            LocalMachine = 1
        }

        public static T GetSetting<T>(string keyName, RegistryHKey level, T defaultValue = default(T))
        {
            var fullPath = GetFullPath(null);

            var key = GetKey(level, fullPath);
            if (key == null) throw new InvalidOperationException($"Cannot get key for registry path {fullPath}.");

            var value = key.GetValue(keyName);
            if (value == null)
            {
                if (defaultValue == null) throw new InvalidOperationException($"Cannot find setting for registry path {fullPath} and key {keyName} and there is no default value provided.");

                //key.SetValue(keyName, defaultValue);
                return defaultValue;
            }
            return ConvertValue<T>(value.ToString());
        }

        public static void SetSetting<T>(string keyName, T value, RegistryHKey level, string subPath = null)
        {
            var fullPath = GetFullPath(subPath);

            var key = GetKey(level, fullPath);
            if (key == null) throw new InvalidOperationException($"Cannot get key for registry path {fullPath}.");

            key.SetValue(keyName, value.ToString());
        }

        public static void ClearAllSettings()
        {
            try
            {
                var fullPath = GetFullPath(null);
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(fullPath);
            }
            catch (ArgumentException)
            {
            }
        }

        private static string GetFullPath(string subPath)
        {
            if (!string.IsNullOrEmpty(subPath))
                subPath = "\\" + subPath;

            var fullPath = $@"Software\{GetPath(Assembly.GetEntryAssembly())}{subPath}";
            return fullPath;
        }

        private static string GetPath(Assembly assembly)
        {
            var sb = new StringBuilder();

            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            if (!string.IsNullOrEmpty(versionInfo.CompanyName))
            {
                sb.Append(versionInfo.CompanyName + "\\");
            }

            var assemblyName = assembly.GetName().Name;
            sb.Append(assemblyName.Replace(".", "\\"));

            var result = sb.ToString();
            return result;
        }

        private static Microsoft.Win32.RegistryKey GetKey(RegistryHKey environment, string path)
        {
            Microsoft.Win32.RegistryKey key;
            switch (environment)
            {
                case RegistryHKey.CurrentUser:
                    key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(path);
                    break;
                case RegistryHKey.LocalMachine:
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(path);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown environment {environment}.");
            }

            return key;
        }

        private static T ConvertValue<T>(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}