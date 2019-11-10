using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DafnyLanguageServer
{
    public class FileHelper
    {
        public static string GetCurrentWord(string code, int line, int character)
        {
            var codeLines = Regex.Split(code, "\r\n|\r|\n");
            var selectedLine = codeLines[line].Substring(0, character);
            var match = Regex.Match(selectedLine, @"(\S+)\.$");
            return (match.Success) ? (match.Groups[1].Value) : null;
        }

        public static bool ChildIsContainedByParent(
            int? childLineStart, int? childLineEnd, int? childPositionStart, int? childPositionEnd,
            int? parentLineStart, int? parentLineEnd, int? parentPositionStart, int? parentPositionEnd
        )

        {
            return (
                (childLineStart >= parentLineStart && childLineEnd <= parentLineEnd && parentLineStart != parentLineEnd) ||
                // if it is an one liner check position 
                (parentLineStart == parentLineEnd && childLineStart == childLineEnd  && parentLineStart == childLineStart
                 && childPositionStart >= parentPositionStart && childPositionEnd <= parentPositionEnd)
            );
        }
    }
}
