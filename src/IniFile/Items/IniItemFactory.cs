#region --- License & Copyright Notice ---
/*
IniFile Library for .NET
Copyright (c) 2018 Jeevan James
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IniFile.Items
{
    internal static class IniItemFactory
    {
        /// <summary>
        ///     Creates the correct <see cref="IniItem"/> object from the given line. If the line
        ///     does not match any INI item object, an exception is thrown.
        /// </summary>
        /// <param name="line">A line from the INI file.</param>
        /// <returns>The <see cref="IniItem"/> object that matches the line format.</returns>
        internal static IniItem CreateItem(string line)
        {
            IniItem item = TryCreateProperty(line)
               ?? TryCreateSection(line)
               ?? TryCreateComment(line)
               ?? TryCreateBlankLine(line);

            if (item != null)
                return item;

            throw new FormatException(string.Format(CultureInfo.CurrentCulture, ErrorMessages.UnrecognizedLine, line));
        }

        private static IniItem TryCreateSection(string line)
        {
            Match match = SectionPattern.Match(line);
            if (!match.Success)
                return null;
            var section = new Section(match.Groups[3].Value);
            section.Padding.Left = match.Groups[1].Length;
            section.Padding.InsideLeft = match.Groups[2].Length;
            section.Padding.InsideRight = match.Groups[4].Length;
            section.Padding.Right = match.Groups[5].Length;
            return section;
        }

        private static readonly Regex SectionPattern = new Regex(@"^(\s*)\[(\s*)([\w\.\$\:][\w_~\-\.\:\s]*)(\s*)\](\s*)$");

        private static IniItem TryCreateProperty(string line)
        {
            Match match = PropertyPattern.Match(line);
            if (!match.Success)
                return null;
            var property = new Property(match.Groups[2].Value, match.Groups[5].Value);
            property.Padding.Left = match.Groups[1].Length;
            property.Padding.InsideLeft = match.Groups[3].Length;
            property.Padding.InsideRight = match.Groups[4].Length;
            property.Padding.Right = match.Groups[6].Length;
            return property;
        }

        private static readonly Regex PropertyPattern = new Regex(@"^(\s*)([\w\.\$\:][\w_~\-\.\:\s]*)(\s*)=(\s*)(.*)(\s*)$");

        private static IniItem TryCreateComment(string line)
        {
            Match match = Ini.Config.HashForComments.Allow
                ? CommentWithHashPattern.Match(line) : CommentPattern.Match(line);
            if (!match.Success)
                return null;
            var comment = new Comment(match.Groups[4].Value)
            {
                CommentChar = match.Groups[2].Value == ";" ? CommentChar.Semicolon : CommentChar.Hash
            };
            comment.Padding.Left = match.Groups[1].Length;
            comment.Padding.Inside = match.Groups[3].Length;
            comment.Padding.Right = match.Groups[5].Length;
            return comment;
        }

        private static readonly Regex CommentPattern = new Regex(@"^(\s*)(;)(\s*)(.+)(\s*)$");

        private static readonly Regex CommentWithHashPattern = new Regex(@"^(\s*)([;|#])(\s*)(.+)(\s*)$");

        private static IniItem TryCreateBlankLine(string line)
        {
            if (line.Trim().Length == 0)
            {
                var blankLine = new BlankLine();
                blankLine.Padding.Left = line.Length;
                return blankLine;
            }
            return null;
        }
    }
}