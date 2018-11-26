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
using System.Collections.Generic;
using System.Diagnostics;

namespace IniFile.Items
{
    public abstract class MajorIniItem : IniItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _name;

        protected MajorIniItem(string name)
        {
            Name = name;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name should contain at least one alpha-numeric character.", nameof(value));
                _name = value.Trim();
            }
        }

        public IList<MinorIniItem> Items { get; } = new List<MinorIniItem>();

        public Comment AddComment(string text)
        {
            var comment = new Comment(text);
            Items.Add(comment);
            return comment;
        }

        public BlankLine AddBlankLine()
        {
            var blankLine = new BlankLine();
            Items.Add(blankLine);
            return blankLine;
        }
    }
}