using NUglify;

namespace LayerZero.Tools.Web.Parser
{
    public static class CssFileParser
    {
        public static string Analyse(string CssFilePath)
        {
            var css = File.ReadAllText(CssFilePath);
            var result = Uglify.Css(css);
            return string.IsNullOrWhiteSpace(result.Code) ? css : result.Code;
        }
    }
}
