using FluentAssertions;
using LayerZero.Tools.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Test.Tools.IO
{
    public class SpindleTreeTests
    {
        private readonly string _testRoot = Path.Combine(Path.GetTempPath(), "SpindleTreeTest");

        public SpindleTreeTests()
        {
            if (Directory.Exists(_testRoot))
                Directory.Delete(_testRoot, true);
            Directory.CreateDirectory(_testRoot);
        }

        [Fact]
        public void ReturnsNull_WhenDirectoryIsEmpty()
        {
            var result = SpindleTree.GetAllFilesPath(_testRoot);
            result.Should().BeNull();
        }

        [Fact]
        public void ReturnsFiles_WhenFilesExist()
        {
            var filePath = Path.Combine(_testRoot, "test.js");
            File.WriteAllText(filePath, "// js");

            var result = SpindleTree.GetAllFilesPath(_testRoot, FileExtensions: new[] { ".js" });

            result.Should().NotBeNull();
            result.Should().ContainSingle(f => f.EndsWith("test.js"));
        }

        [Fact]
        public void FiltersByExtensionCorrectly()
        {
            File.WriteAllText(Path.Combine(_testRoot, "file1.css"), "body{}");
            File.WriteAllText(Path.Combine(_testRoot, "file2.js"), "// js");

            var result = SpindleTree.GetAllFilesPath(_testRoot, FileExtensions: new[] { ".css" });

            result.Should().NotBeNull();
            result.Should().OnlyContain(f => f.EndsWith(".css"));
        }

        [Fact]
        public void SearchesSubdirectories_WhenRequested()
        {
            var subDir = Path.Combine(_testRoot, "sub");
            Directory.CreateDirectory(subDir);
            File.WriteAllText(Path.Combine(subDir, "subfile.js"), "// js");

            var result = SpindleTree.GetAllFilesPath(_testRoot, SearchOption.AllDirectories, new[] { ".js" });

            result.Should().NotBeNull();
            result.Should().ContainSingle(f => f.Contains("subfile.js"));
        }
    }
}
