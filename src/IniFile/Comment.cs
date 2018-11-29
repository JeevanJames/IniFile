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

using IniFile.Items;

namespace IniFile
{
    /// <summary>
    ///     Represents a comment object in an INI.
    /// </summary>
    public sealed class Comment : MinorIniItem, IPaddedItem<CommentPadding>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _text;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Comment"/> class with an empty comment.
        /// </summary>
        public Comment()
        {
            Text = string.Empty;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Comment"/> class with the specified text.
        /// </summary>
        /// <param name="text">The comment text.</param>
        public Comment(string text)
        {
            Text = text;
        }

        /// <summary>
        ///     The comment text.
        /// </summary>
        public string Text
        {
            get => _text;
            set => _text = value ?? string.Empty;
        }

        /// <summary>
        ///     Padding details of this <see cref="Comment"/>.
        /// </summary>
        public CommentPadding Padding { get; } = new CommentPadding();

        /// <inheritdoc/>
        public override string ToString() =>
            $"{Padding.Left.ToString()};{Padding.Inside.ToString()}{Text}{Padding.Right.ToString()}";
    }
}