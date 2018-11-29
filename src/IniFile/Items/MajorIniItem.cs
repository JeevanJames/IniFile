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
    /// <summary>
    ///     <para>
    ///         Base class for the major INI items, namely the <see cref="Section"/> and
    ///         <see cref="Property"/>.
    ///     </para>
    ///     <para>Each major INI item can have zero or more comments and blank lines.</para>
    /// </summary>
    public abstract class MajorIniItem : IniItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MajorIniItem"/> class.
        /// </summary>
        /// <param name="name">The unique name of the INI item.</param>
        /// <param name="items">
        ///     Collection of strings that represent the comments and blank lines of the INI item.
        ///     If the string is <c>null</c>, an empty string or a whitespace string, then a
        ///     <see cref="BlankLine"/> object is created, otherwise a <see cref="Comment"/> is created.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if the specified name is <c>null</c>.</exception>
        protected MajorIniItem(string name, params string[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            Name = name;
            if (items != null)
            {
                foreach (string item in items)
                {
                    if (string.IsNullOrWhiteSpace(item))
                        Items.Add(new BlankLine {Padding = {Left = (item ?? string.Empty).Length}});
                    else
                        Items.Add(new Comment(item));
                }
            }
        }

        /// <summary>
        ///     The unique name of the <see cref="Section"/> or the <see cref="Property"/>.
        /// </summary>
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

        /// <summary>
        ///     The collection of comments and blank lines that are associated with this major INI item.
        /// </summary>
        public IList<MinorIniItem> Items { get; } = new List<MinorIniItem>();

        /// <summary>
        ///     Shortcut method to adding a <see cref="Comment"/> class to the <see cref="Items"/>
        ///     property.
        /// </summary>
        /// <param name="text">The comment text.</param>
        /// <returns>The newly-added comment.</returns>
        public Comment AddComment(string text)
        {
            var comment = new Comment(text);
            Items.Add(comment);
            return comment;
        }

        /// <summary>
        ///     Shortcut method to adding a <see cref="BlankLine"/> class to the <see cref="Items"/>
        ///     property.
        /// </summary>
        /// <returns>The newly-adding blank line.</returns>
        public BlankLine AddBlankLine()
        {
            var blankLine = new BlankLine();
            Items.Add(blankLine);
            return blankLine;
        }
    }
}