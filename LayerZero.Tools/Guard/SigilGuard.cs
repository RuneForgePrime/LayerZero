using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Guard
{
    /// <summary>
    /// Strings, patterns, literals
    /// </summary>
    public static class SigilGuard
    {
        public static void IsNotNullNorEmptyOrWhiteSpace(this string Text,
                                        [CallerArgumentExpression("Text")] string sourceExpression = "")
        {
            if (Text == null || Text.Trim().Length == 0)
                throw new ArgumentNullException(sourceExpression);
        }
    }
}
