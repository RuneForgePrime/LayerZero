using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Configs.LeyLine
{
    public sealed class EchoShard : IDisposable
    {
        public string FileName { get; init; }
        public string ContentType { get; init; }
        public MemoryStream Content { get; init; }

        public void Dispose()
        {
            Content?.Dispose();
        }
    }
}
