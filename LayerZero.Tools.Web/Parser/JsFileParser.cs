using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Web.Parser
{
    public static class JsFileParser
    {
        public static string Analyse(string JsFilePath)
        {
            var css = File.ReadAllText(JsFilePath);

            return css;
        }
    }
}
