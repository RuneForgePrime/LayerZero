using FluentAssertions;
using LayerZero.Tools.Guard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Test.Tools.Guards
{
    public class SpindleTreeGuardTests
    {
        [Fact]
        public void Should_ReturnTrue_When_DirectoryIsEmpty()
        {
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            try
            {
                SpindleTreeGuard.IsDirectoryEmpty(dir.FullName).Should().BeTrue();
            }
            finally
            {
                dir.Delete(true);
            }
        }

        [Fact]
        public void Should_ReturnFalse_When_DirectoryHasFiles()
        {
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            File.WriteAllText(Path.Combine(dir.FullName, "test.txt"), "content");

            try
            {
                SpindleTreeGuard.IsDirectoryEmpty(dir.FullName).Should().BeFalse();
            }
            finally
            {
                dir.Delete(true);
            }
        }

        [Fact]
        public void Should_RespectFileExtensionsFilter()
        {
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            File.WriteAllText(Path.Combine(dir.FullName, "test.log"), "log file");

            try
            {
                SpindleTreeGuard.IsDirectoryEmpty(dir.FullName, SearchOption.AllDirectories, new[] { ".txt" })
                    .Should().BeTrue();

                SpindleTreeGuard.IsDirectoryEmpty(dir.FullName, SearchOption.AllDirectories, new[] { ".log" })
                    .Should().BeFalse();
            }
            finally
            {
                dir.Delete(true);
            }
        }

        [Fact]
        public void Should_Throw_When_DirectoryDoesNotExist()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var act = () => SpindleTreeGuard.IsDirectoryEmpty(path);
            act.Should().Throw<ArgumentException>();
        }
    }
}
