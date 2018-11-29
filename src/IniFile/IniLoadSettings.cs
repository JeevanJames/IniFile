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
using System.Diagnostics;
using System.Text;

namespace IniFile
{
    /// <summary>
    ///     The settings to use when loading INI data from files, streams and text readers.
    /// </summary>
    [DebuggerDisplay("Encoding: {Encoding.EncodingName}; Detect: {DetectEncoding}; Case sensitive: {CaseSensitive}")]
    public sealed class IniLoadSettings
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Encoding _encoding;

        /// <summary>
        ///     Initializes a new instance of the <see cref="IniLoadSettings"/> class.
        /// </summary>
        public IniLoadSettings()
        {
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        ///     Gets or sets the character encoding to use when loading or saving INI data.
        /// </summary>
        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        ///     Gets or sets whether to automatically detect the character encoding when loading
        ///     INI data.
        /// </summary>
        public bool DetectEncoding { get; set; }

        /// <summary>
        ///     Gets or sets whether to consider section and property key names as case sensitive,
        ///     when searching for them by name.
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        ///     Default settings to use to load INI data.
        /// </summary>
        public static readonly IniLoadSettings Default = new IniLoadSettings();
    }
}