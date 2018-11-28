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

namespace IniFile
{
    /// <summary>
    ///     Options for formatting the contents of the INI.
    /// </summary>
    public sealed class IniFormatOptions
    {
        /// <summary>
        ///     Inserts blank lines between sections, if there isn't any.
        /// </summary>
        public bool EnsureBlankLineBetweenSections { get; set; }

        /// <summary>
        ///     Inserts blank lines between properties, if there isn't any.
        /// </summary>
        public bool EnsureBlankLineBetweenProperties { get; set; }

        /// <summary>
        ///     Removes successive blank lines
        /// </summary>
        public bool RemoveSuccessiveBlankLines { get; set; }

        /// <summary>
        ///     Default formatting options for INI content.
        /// </summary>
        public static readonly IniFormatOptions Default = new IniFormatOptions();
    }
}