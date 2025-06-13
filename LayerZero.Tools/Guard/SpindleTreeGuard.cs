using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Guard
{
    public static class SpindleTreeGuard
    {

        public static bool Exists(string DirectoryPath)
        {
            DirectoryPath.IsNotNullNorEmptyOrWhiteSpace();
            return Directory.Exists(DirectoryPath);
        }

        public static bool IsDirectoryEmpty(string DirectoryPath,
                                            SearchOption SearchOption = SearchOption.AllDirectories,
                                            string[]? FileExtensions = null, 
                                            [CallerArgumentExpression("DirectoryPath")] string sourceExpression = "")
        {


            DirectoryPath.IsNotNullNorEmptyOrWhiteSpace();
            if (!Exists(DirectoryPath))
                throw new ArgumentException($"Provided path {sourceExpression} does not exists");

            if (FileExtensions == null || FileExtensions.Length == 0)
                return !Directory.EnumerateFiles(DirectoryPath, "*.*", SearchOption).Any();


            return !Directory.EnumerateFiles(DirectoryPath, "*.*", SearchOption)
            .Any(file => FileExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase));

        }
    }
}
