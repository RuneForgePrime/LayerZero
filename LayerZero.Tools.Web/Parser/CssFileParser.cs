using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Web.Parser
{
    public static class CssFileParser
    {
        public static string Analyse(string CssFilePath)
        {
            var css = File.ReadAllText(CssFilePath);

            var parser = new CssParser();

            var rules = new StringBuilder();

            var styles = parser.ParseStyleSheet(css);

            FormatCssRules(styles.Rules, rules, 1);

            var rl = rules.ToString();

            return rules.ToString();
        }


        public static void FormatCssRules(ICssRuleList rules, StringBuilder sb, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 2);

            foreach (var rule in rules)
            {
                switch (rule)
                {
                    case ICssStyleRule styleRule:
                        if (styleRule.Style.Length == 0)
                            continue;

                        sb.AppendLine($"{indent}{styleRule.SelectorText} {{");
                        foreach (var decl in styleRule.Style)
                        {
                            sb.AppendLine($"{indent}  {decl.Name}: {decl.Value};");
                        }
                        sb.AppendLine($"{indent}}}");
                        break;

                    case ICssMediaRule mediaRule:
                        sb.AppendLine($"{indent}@media {mediaRule.ConditionText} {{");
                        FormatCssRules(mediaRule.Rules, sb, indentLevel + 1);
                        sb.AppendLine($"{indent}}}");
                        break;

                    case ICssSupportsRule supportsRule:
                        sb.AppendLine($"{indent}@supports {supportsRule.ConditionText} {{");
                        FormatCssRules(supportsRule.Rules, sb, indentLevel + 1);
                        sb.AppendLine($"{indent}}}");
                        break;

                    default:
                        sb.AppendLine($"{indent}{rule.CssText}");
                        break;
                }
            }
        }
    }
}
