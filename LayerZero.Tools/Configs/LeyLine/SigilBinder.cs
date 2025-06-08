using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Configs.LeyLine
{
    public class SigilBinder
    {
        public string? AccessToken { get; init; }
        public double ExpiresIn { get; init; }
        public double ExpiresOn { get; init; }
        public string? TokenType { get; init; }

        public DateTime ExpiresAt => DateTimeOffset.FromUnixTimeSeconds((long)ExpiresOn).ToLocalTime().DateTime;

        public bool IsValid => ExpiresAt > DateTime.Now;
    }
}
