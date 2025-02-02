﻿using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Text.RegularExpressions;

namespace DafnyLanguageServer.ContentManager
{
    public class FileHelper
    {
        public static string GetCurrentWord(string code, int line, int character)
        {
            var selectedLine = SaveLineGetter(code, line, character);
            var match = Regex.Match(selectedLine, @"(\S+)\.$");
            return (match.Success) ? (match.Groups[1].Value) : null;
        }

        public static string GetFollowingWord(string code, int line, int character)
        {
            var selectedLine = SaveLineGetter(code, line, character, false);
            var match = Regex.Match(selectedLine, @"^([a-zA-Z0-9-_]+).*");
            return (match.Success) ? (match.Groups[1].Value) : null;
        }

        private static string SaveLineGetter(string code, int line, int character, bool front = true)
        {
            var codeLines = Regex.Split(code, "\r\n|\r|\n");
            // avoid out of bounce exceptions 
            return (codeLines.Length >= line && codeLines[line].Length >= character)
                ?
                    (front
                    ? codeLines[line].Substring(0, character)
                    : codeLines[line].Substring(character))
                : "";
        }

        // 2Do; this args are pain to documentate... make an object -- ticket #49
        public static bool ChildIsContainedByParent(
            int? childLineStart, int? childLineEnd, int? childPositionStart, int? childPositionEnd,
            int? parentLineStart, int? parentLineEnd, int? parentPositionStart, int? parentPositionEnd
        )

        {
            return (
                (childLineStart >= parentLineStart && childLineEnd <= parentLineEnd && parentLineStart != parentLineEnd) ||
                // if it is an one liner - check position 
                (parentLineStart == parentLineEnd && childLineStart == childLineEnd && parentLineStart == childLineStart
                 && childPositionStart >= parentPositionStart && childPositionEnd <= parentPositionEnd)
            );
        }

        public static string EscapeFilePath(string path)
        {
            string escapeCharacter = "\"";

            if (path.StartsWith(escapeCharacter) && path.EndsWith(escapeCharacter))
            {
                path = path.Substring(1, path.Length - 2);
            }

            if (path.Contains(escapeCharacter))
            {
                throw new NotSupportedException("Filename with Quote is not supported.");
            }

            if (path.Contains(" "))
            {
                return escapeCharacter + path + escapeCharacter;
            }
            else
            {
                return path;
            }
        }

        public static Position CreatePosition(long start, long end)
        {
            if (start < 0 || end < 0)
            {
                throw new ArgumentException("Negative position values are not supported");
            }

            return new Position
            {
                Line = start,
                Character = end
            };
        }

        public static Range CreateRange(long line, long chr, long length)
        {
            if (length < 0)
            {
                length = Math.Abs(length);
                chr -= length;
            }

            Position start = CreatePosition(line, chr);
            Position end = CreatePosition(line, chr + length);
            return new Range(start, end);
        }

        public static int GetLineLength(string source, int line)
        {
            string[] lines = Regex.Split(source, "\r\n|\r|\n");
            if (line < 0)
            {
                throw new ArgumentException("Line-Index must not be negative");
            }
            if (line >= lines.Length)
            {
                throw new ArgumentException($"There are not enogh lines ({line}) in the given source!");
            }
            return lines[line].Length;
        }
    }
}
