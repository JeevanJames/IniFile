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
    public sealed partial class Property : MajorIniItem, IPaddedItem<PropertyPadding>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _value;

        public Property(string name, string value) : base(name)
        {
            Value = value;
        }

        public string Value
        {
            get => _value;
            set => _value = value ?? string.Empty;
        }

        public PropertyPadding Padding { get; } = new PropertyPadding();

        public override string ToString() =>
            $"{Padding.Left.ToString()}{Name}{Padding.InsideLeft.ToString()}={Padding.InsideRight.ToString()}{Value}{Padding.Right.ToString()}";
    }
}