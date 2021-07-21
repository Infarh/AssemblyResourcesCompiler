using System;
using System.IO;
using System.Reflection;

namespace System.IO
{
    internal static class FileInfoExtensions
    {
        public static string GetLocalPath(this FileInfo file)
        {
            var local = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
            var path = file.FullName;
            if (path.StartsWith(local, StringComparison.OrdinalIgnoreCase))
                return path[(local.Length + 1)..];
            return file.Name;
        }
    }
}
