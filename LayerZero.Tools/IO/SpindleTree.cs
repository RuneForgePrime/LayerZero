using LayerZero.Tools.Guard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.IO
{
    /// <summary>
    /// Directory management
    /// </summary>

    public static class SpindleTree
    {
        public static List<string>? GetAllFilesPath(string DirectoryPath,
                                            SearchOption SearchOption = SearchOption.AllDirectories,
                                            string[]? FileExtensions = null,
                                            [CallerArgumentExpression("DirectoryPath")] string sourceExpression = "")
        {
            var isDirectoryempty = SpindleTreeGuard.IsDirectoryEmpty(DirectoryPath, SearchOption, FileExtensions);
            if (isDirectoryempty)
                return null;

            var files = Directory.EnumerateFiles(DirectoryPath, "*.*", SearchOption);

            if (FileExtensions == null || FileExtensions.Length == 0)
                return files.ToList();

            return files.Where(file => FileExtensions.Any(ext => file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        public static IEnumerable<(string Path, int Depth)> GetDirectories(string RootPath, int MaxDepth = 2, int CurrentDepth = 1)
        {
            if (RootPath == null)
                yield break;

            if (CurrentDepth > MaxDepth)
                yield break;

            string[] subFolders;

            try
            {
                subFolders = Directory.GetDirectories(RootPath, "*", SearchOption.AllDirectories);
            }
            catch (UnauthorizedAccessException)
            {
                yield break;
            }

            foreach (var folder in subFolders)
            {
                yield return (folder, CurrentDepth);

                foreach (var sub in GetDirectories(folder, MaxDepth, CurrentDepth + 1))
                    yield return sub;
            }
        }
    }
}
