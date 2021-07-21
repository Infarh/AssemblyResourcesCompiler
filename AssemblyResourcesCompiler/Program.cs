using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using AssemblyResourcesCompiler.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AssemblyResourcesCompiler
{
    internal static class Program
    {
        private static string ResultFileName { get; set; } = "resource.dll";

        static void Main(string[] args)
        {
            IEnumerable<FileInfo> files = null;
            var current_dir = new DirectoryInfo(Environment.CurrentDirectory);
            if (args is not { Length: > 0 })
                files = ProcessDirectory(current_dir);
            else
                foreach (var arg in args)
                    if (arg.Contains('*'))
                        files = files.Append(ProcessDirectory(current_dir, arg));
                    else if (File.Exists(arg))
                        files = EnumExt.Append(files, new FileInfo(arg));
                    else if (Directory.Exists(arg))
                        files = files.Append(ProcessDirectory(new DirectoryInfo(arg), arg));

            var to_processing = files?.Distinct().ToArray();
            if (to_processing is not {Length: > 0})
            {
                Console.WriteLine("Нет файлов для обработки");
                return;
            }

            foreach (var file in to_processing)
                Console.WriteLine("Упаковка файла {0}", file.FullName);
            
            var timer = Stopwatch.StartNew();
            CreateAssembly(to_processing);
            Console.WriteLine("Завершено за {0}", timer.Elapsed);
            Console.WriteLine("Нажмите Enter для выхода...");
        }

        private static IEnumerable<FileInfo> ProcessDirectory(DirectoryInfo dir, string Pattern = "*.*", bool SubDirs = false)
        {
            var current_app_file = Path.GetFullPath(Process.GetCurrentProcess().MainModule.FileName);
            foreach (var file in dir.EnumerateFiles(Pattern, SubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                if (!string.Equals(file.FullName, current_app_file, StringComparison.OrdinalIgnoreCase))
                    yield return file;
        }

        private static void CreateAssembly(FileInfo[] files)
        {
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(ResultFileName, options: options);

            var resources = files.Select(f => new ResourceDescription(f.GetLocalPath(), f.OpenRead, true));

            using var result_file = File.Create(ResultFileName);
            compilation.Emit(result_file, manifestResources: resources);

        }
    }
}
