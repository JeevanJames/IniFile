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
    ///     Represents a property object in an INI.
    /// </summary>
    public sealed partial class Property : MajorIniItem, IPaddedItem<PropertyPadding>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Property"/> class.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="items">
        ///     Collection of strings that represent the comments and blank lines of the property.
        ///     If the string is <c>null</c>, an empty string or a whitespace string, then a
        ///     <see cref="BlankLine"/> object is created, otherwise a <see cref="Comment"/> is created.
        /// </param>
        /// <inheritdoc/>
        public Property(string name, string value, params string[] items) : base(name, items)
        {
            Value = value;
        }

        /// <summary>
        ///     The value of the property.
        /// </summary>
        public string Value
        {
            get => _value;
            set => _value = value ?? string.Empty;
        }

        /// <summary>
        ///     Padding details of this <see cref="Property"/>.
        /// </summary>
        public PropertyPadding Padding { get; } = new PropertyPadding();

        /// <inheritdoc/>
        public override string ToString() =>
            $"{Padding.Left.ToString()}{Name}{Padding.InsideLeft.ToString()}={Padding.InsideRight.ToString()}{Value}{Padding.Right.ToString()}";
    }
}