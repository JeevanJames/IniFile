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

namespace IniFile.Config
{
    /// <summary>
    ///     Configuration for padding defaults for sections, properties and comments.
    /// </summary>
    public sealed class PaddingConfig
    {
        /// <summary>
        ///     Configuration for padding defaults of sections.
        /// </summary>
        public SectionPaddingConfig Section { get; } = new SectionPaddingConfig();

        /// <summary>
        ///     Configuration for padding defaults of properties.
        /// </summary>
        public PropertyPaddingConfig Property { get; } = new PropertyPaddingConfig();

        /// <summary>
        ///     Configuration for padding defaults of comments.
        /// </summary>
        public CommentPaddingConfig Comment { get; } = new CommentPaddingConfig();
    }
}