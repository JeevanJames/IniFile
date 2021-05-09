#region --- License & Copyright Notice ---
/*
IniFile Library for .NET
Copyright (c) 2018-2021 Jeevan James
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
    ///     Represents a blank line object in an INI.
    /// </summary>
    [DebuggerDisplay("----------")]
    public sealed class BlankLine : MinorIniItem, IPaddedItem<Padding>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BlankLine"/> class.
        /// </summary>
        public BlankLine()
        {
        }

        /// <summary>
        ///     Padding details of this <see cref="BlankLine"/>.
        /// </summary>
        public Padding Padding { get; } = new();

        /// <inheritdoc/>
        public override string ToString()
        {
            return Padding.Left.ToString();
        }
    }
}