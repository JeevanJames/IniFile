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
using System.Globalization;
using System.Reflection;

namespace IniFile.Items
{
    /// <summary>
    ///     <para>Represents the value of a property</para>
    ///     <para>Property values can be strings, numbers (both floats and integers), date/times and booleans.</para>
    /// </summary>
    public readonly struct PropertyValue : IEquatable<PropertyValue>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Type _type;

        /// <summary>
        ///     The value of the property, internally using the actual type.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _value;

        /// <summary>
        ///     <para>
        ///         The string representation of the value. This is used if the string representation
        ///         is different from the string returned by calling <c>_value.ToString()</c>.
        ///     </para>
        ///     <para>
        ///         Boolean values use this, as there can be multiple string representations of a
        ///         boolean value such as <c>0</c>, <c>on</c>, etc.
        ///     </para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _stringValue;

        /// <summary>
        ///     Initializes an instance of the <see cref="PropertyValue"/> structure.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize the property with. If <c>null</c>, the type is considered a
        ///     <c>string</c>.
        /// </param>
        internal PropertyValue(object value)
        {
            _value = value;
            _type = _value != null ? _value.GetType() : typeof(string);
            _stringValue = null;
        }

        internal PropertyValue(object value, string stringValue)
        {
            _value = value;
            _type = _value != null ? _value.GetType() : typeof(string);
            _stringValue = stringValue;
        }

        public override string ToString() => _stringValue ?? _value?.ToString();

        public DateTime AsDateTime(string format)
        {
            return ConvertTo(this, s => DateTime.TryParseExact(
                s, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime dt)
                    ? new ConversionResult<DateTime>(true, dt) : new ConversionResult<DateTime>(false, default));
        }

        public TEnum AsEnum<TEnum>()
            where TEnum : struct, IComparable
        {
#if NETSTANDARD1_3
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
#else
            if (!typeof(TEnum).IsEnum)
#endif
                throw new InvalidOperationException();
#if !NET35
            if (!Enum.TryParse(ToString(), true, out TEnum value))
                throw new InvalidCastException(string.Format(ErrorMessages.CannotCastPropertyValue, typeof(TEnum).FullName));
            return value;
#else
            try
            {
                TEnum value = (TEnum) Enum.Parse(typeof(TEnum), ToString(), true);
                return value;
            }
            catch (ArgumentException ex)
            {
                throw new InvalidCastException(string.Format(ErrorMessages.CannotCastPropertyValue, typeof(TEnum).FullName), ex);
            }
#endif
        }

        public static implicit operator PropertyValue(string value) => new PropertyValue(value);
        public static implicit operator string(PropertyValue pvalue) => pvalue.ToString();

        public static implicit operator PropertyValue(sbyte value) => new PropertyValue(value);
        public static implicit operator sbyte(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            sbyte.TryParse(s, out sbyte value) ? new ConversionResult<sbyte>(true, value) : new ConversionResult<sbyte>(false, default)
        );

        public static implicit operator PropertyValue(byte value) => new PropertyValue(value);
        public static implicit operator byte(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            byte.TryParse(s, out byte value) ? new ConversionResult<byte>(true, value) : new ConversionResult<byte>(false, default)
        );

        public static implicit operator PropertyValue(short value) => new PropertyValue(value);
        public static implicit operator short(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            short.TryParse(s, out short value) ? new ConversionResult<short>(true, value) : new ConversionResult<short>(false, default)
        );

        public static implicit operator PropertyValue(ushort value) => new PropertyValue(value);
        public static implicit operator ushort(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            ushort.TryParse(s, out ushort value) ? new ConversionResult<ushort>(true, value) : new ConversionResult<ushort>(false, default)
        );

        public static implicit operator PropertyValue(int value) => new PropertyValue(value);
        public static implicit operator int(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            int.TryParse(s, out int value) ? new ConversionResult<int>(true, value) : new ConversionResult<int>(false, default)
        );

        public static implicit operator PropertyValue(uint value) => new PropertyValue(value);
        public static implicit operator uint(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            uint.TryParse(s, out uint value) ? new ConversionResult<uint>(true, value) : new ConversionResult<uint>(false, default)
        );

        public static implicit operator PropertyValue(long value) => new PropertyValue(value);
        public static implicit operator long(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            long.TryParse(s, out long value) ? new ConversionResult<long>(true, value) : new ConversionResult<long>(false, default)
        );

        public static implicit operator PropertyValue(ulong value) => new PropertyValue(value);
        public static implicit operator ulong(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            ulong.TryParse(s, out ulong value) ? new ConversionResult<ulong>(true, value) : new ConversionResult<ulong>(false, default)
        );

        public static implicit operator PropertyValue(float value) => new PropertyValue(value);
        public static implicit operator float(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            float.TryParse(s, out float value) ? new ConversionResult<float>(true, value) : new ConversionResult<float>(false, default)
        );

        public static implicit operator PropertyValue(double value) => new PropertyValue(value);
        public static implicit operator double(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            double.TryParse(s, out double value) ? new ConversionResult<double>(true, value) : new ConversionResult<double>(false, default)
        );

        public static implicit operator PropertyValue(decimal value) => new PropertyValue(value);
        public static implicit operator decimal(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            decimal.TryParse(s, out decimal value) ? new ConversionResult<decimal>(true, value) : new ConversionResult<decimal>(false, default)
        );

        public static implicit operator PropertyValue(DateTime value) =>
            new PropertyValue(value, value.ToString(Ini.Config.Types.DateFormat));
        public static implicit operator DateTime(PropertyValue pvalue) => ConvertTo(pvalue, s => 
            DateTime.TryParseExact(s, Ini.Config.Types.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime value)
                ? new ConversionResult<DateTime>(true, value) : new ConversionResult<DateTime>(false, default)
        );

        private static T ConvertTo<T>(PropertyValue pvalue, Func<string, ConversionResult<T>> converter)
            where T : struct
        {
            if (pvalue._value is T typedValue)
                return typedValue;

            string stringValue = pvalue.ToString();
            if (stringValue == null)
                throw new InvalidCastException(string.Format(ErrorMessages.CannotCastPropertyValue, typeof(T).Name));

            ConversionResult<T> result = converter(stringValue);

            if (!result.CanConvert)
                throw new InvalidCastException(string.Format(ErrorMessages.CannotCastPropertyValue, typeof(T).Name));
            return result.Value;
        }

        public static implicit operator PropertyValue(bool value) =>
            new PropertyValue(value, value ? Ini.Config.Types.TrueString : Ini.Config.Types.FalseString);

        public static implicit operator bool(PropertyValue pvalue)
        {
            if (pvalue._value is bool)
                return (bool)pvalue._value;

            string stringValue = pvalue.ToString()?.ToLowerInvariant();
            if (stringValue == null)
                throw new InvalidCastException();

            switch (stringValue)
            {
                case "0":
                case "f":
                case "n":
                case "off":
                case "no":
                case "disabled":
                case "false":
                    return false;
                case "1":
                case "t":
                case "y":
                case "on":
                case "yes":
                case "enabled":
                case "true":
                    return true;
            }

            if (stringValue == Ini.Config.Types.TrueString.ToLowerInvariant())
                return true;
            if (stringValue == Ini.Config.Types.FalseString.ToLowerInvariant())
                return false;

            throw new Exception($"'value' is not a boolean value.");
        }

        public static bool operator ==(PropertyValue value1, PropertyValue value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(PropertyValue value1, PropertyValue value2)
        {
            return !(value1 == value2);
        }

        public override bool Equals(object obj)
        {
            return obj is PropertyValue && Equals((PropertyValue)obj);
        }

        public bool Equals(PropertyValue other)
        {
            return EqualityComparer<Type>.Default.Equals(_type, other._type);
        }

        public override int GetHashCode()
        {
            return -331038658 + EqualityComparer<Type>.Default.GetHashCode(_type);
        }

        public bool IsEmpty() => _value == null && _stringValue == null;

        public static readonly PropertyValue Empty = new PropertyValue(null);
    }

    internal readonly struct ConversionResult<T>
        where T : struct
    {
        internal ConversionResult(bool canConvert, T value)
        {
            CanConvert = canConvert;
            Value = value;
        }

        internal bool CanConvert { get; }

        internal T Value { get; }
    }
}