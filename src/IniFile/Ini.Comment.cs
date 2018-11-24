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

using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using IniFile.Items;

namespace IniFile
{
    public sealed partial class Ini
    {
        [DebuggerDisplay(";{Text}")]
        public sealed class Comment : ITopLevelIniItem, ISectionItem
        {
            internal Comment()
            {
            }

            public Comment(string text)
            {
                Text = text;
            }

            public string Text { get; set; }

            IIniItem IIniItem.TryCreate(string line)
            {
                Match match = TextPattern.Match(line);
                if (!match.Success)
                    return null;
                return new Comment
                {
                    Text = match.Groups[1].Value
                };
            }

            private static readonly Regex TextPattern = new Regex(@"^\s*;(.*)$");

            async Task IIniItem.Write(TextWriter writer)
            {
                await writer.WriteLineAsync($";{Text ?? ""}");
            }
        }
    }
}
