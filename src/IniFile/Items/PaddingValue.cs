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

namespace IniFile.Items
{
    /// <summary>
    ///     Represents the size of padding - the number of spaces in the padding.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public readonly struct PaddingValue : IEquatable<PaddingValue>
    {
        internal PaddingValue(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
        }

        /// <summary>
        ///     The number of spaces in the padding.
        /// </summary>
        public int Value { get; }

        public override bool Equals(object obj)
        {
            return obj is PaddingValue && Equals((PaddingValue)obj);
        }

        public bool Equals(PaddingValue other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + Value.GetHashCode();
        }

        /// <summary>
        ///     Converts this <see cref="PaddingValue"/> to a padded string.
        /// </summary>
        /// <returns>The padded string containing the equal number of spaces.</returns>
        public override string ToString() =>
            Value == 0 ? string.Empty : new string(' ', Value);

        public static bool operator ==(PaddingValue value1, PaddingValue value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(PaddingValue value1, PaddingValue value2)
        {
            return !(value1 == value2);
        }

        public static implicit operator PaddingValue(int value) =>
            new PaddingValue(value);

        public static implicit operator int(PaddingValue paddingValue) =>
            paddingValue.Value;
    }
}