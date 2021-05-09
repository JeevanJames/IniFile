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

namespace IniFile.Items
{
    /// <summary>
    ///     Represents the padding details for an INI section.
    /// </summary>
    public sealed class SectionPadding : Padding
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SectionPadding"/> class.
        /// </summary>
        public SectionPadding()
        {
            SetDefaults();
        }

        /// <summary>
        ///     The amount of space to the right of the section.
        /// </summary>
        public PaddingValue Right { get; set; }

        /// <summary>
        ///     The amount of space between the left brace of the section and the section text.
        /// </summary>
        public PaddingValue InsideLeft { get; set; }

        /// <summary>
        ///     The amount of space between the right brace of the section and the section text.
        /// </summary>
        public PaddingValue InsideRight { get; set; }

        /// <inheritdoc/>
        public override void Reset()
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            Left = Ini.Config.Padding.Section.Left;
            InsideLeft = Ini.Config.Padding.Section.InsideLeft;
            InsideRight = Ini.Config.Padding.Section.InsideRight;
            Right = Ini.Config.Padding.Section.Right;
        }
    }
}