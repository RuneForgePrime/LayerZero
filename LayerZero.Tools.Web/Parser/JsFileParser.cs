using Esprima;

namespace LayerZero.Tools.Web.Parser
{
    public static class JsFileParser
    {
        public static string Analyse(string JsFilePath)
        {
            var js = File.ReadAllText(JsFilePath);

            if(!HasSyntaxError(js, out var error))
                return js;
            else
                return $@"/* File {Path.GetFileName(JsFilePath)} Skipped: {error}  */";

        }

        public static bool HasSyntaxError(string jsCode, out string error)
        {
            try
            {
                var parser = new JavaScriptParser();
                parser.ParseScript(jsCode);
                error = string.Empty;
                return false;
            }
            catch (ParserException ex)
            {
                error = $" Syntax error at line {ex.LineNumber}, column {ex.Column}: {ex.Description}";
                return true;
            }
        }
    }
}
