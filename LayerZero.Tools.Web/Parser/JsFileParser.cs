using NUglify;

namespace LayerZero.Tools.Web.Parser
{
    public static class JsFileParser
    {
        public static string Analyse(string JsFilePath)
        {
            var js = File.ReadAllText(JsFilePath);
            var result = Uglify.Js(js);
            if (result.HasErrors)
                return $"/* File {Path.GetFileName(JsFilePath)} Skipped: {result.Errors[0].Message} */";
            return js;
        }
    }
}
