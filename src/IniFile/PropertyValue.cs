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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

#if NETSTANDARD1_3
using System.Reflection;
#endif

namespace IniFile
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
        internal PropertyValue(object value) : this(value, null)
        {
        }

        /// <summary>
        ///     Initializes an instance of the <see cref="PropertyValue"/> structure where the string
        ///     representation of the underlying object is different from the string representation
        ///     of the value in the INI file.
        /// </summary>
        /// <param name="value">
        ///     The value to initialize the property with. If <c>null</c>, the type is considered a
        ///     <c>string</c>.
        /// </param>
        /// <param name="stringValue">
        ///     The different string representation of the value in the INI file.
        /// </param>
        internal PropertyValue(object value, string stringValue)
        {
            _value = value;
            _type = _value != null ? _value.GetType() : typeof(string);
            _stringValue = stringValue;
        }

        /// <summary>
        ///     Returns the string value, if it exists, otherwise returns the <see cref="ToString"/>
        ///     result of the object value. If the object value is <c>null</c>, then the returned
        ///     value is also <c>null</c>.
        /// </summary>
        /// <returns>String that represents the current property value.</returns>
        public override string ToString()
        {
            return _stringValue ?? _value?.ToString();
        }

        /// <summary>
        ///     Returns the property value as a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="format">
        ///     A format specifier that defines the required format of the property string value.
        /// </param>
        /// <param name="provider">
        ///     An object that specifies the culture-specific formatting information about the
        ///     property string value.
        /// </param>
        /// <returns>The property value as a <see cref="DateTime"/>.</returns>
        public DateTime AsDateTime(string format, IFormatProvider provider = null)
        {
            provider ??= CultureInfo.CurrentCulture;
            return ConvertTo(this, s => DateTime.TryParseExact(
                s, format, provider, DateTimeStyles.None, out DateTime dt)
                    ? new ConversionResult<DateTime>(dt) : default);
        }

        /// <summary>
        ///     Returns the property value as the specified enum.
        /// </summary>
        /// <typeparam name="TEnum">The type of enum to convert the property value to.</typeparam>
        /// <param name="caseSensitive">Indicates whether the property value is case-sensitive.</param>
        /// <returns>The property value as the specified enum.</returns>
        public TEnum AsEnum<TEnum>(bool caseSensitive = false)
            where TEnum : struct, IConvertible
        {
#if NETSTANDARD1_3
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
#else
            if (!typeof(TEnum).IsEnum)
#endif
                throw new InvalidOperationException();
#if !NET35
            if (!Enum.TryParse(ToString(), !caseSensitive, out TEnum value))
                throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, ErrorMessages.CannotCastPropertyValue, typeof(TEnum).FullName));
            return value;
#else
            try
            {
                var value = (TEnum) Enum.Parse(typeof(TEnum), ToString(), !caseSensitive);
                return value;
            }
            catch (ArgumentException ex)
            {
                throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, ErrorMessages.CannotCastPropertyValue, typeof(TEnum).FullName), ex);
            }
#endif
        }

        public static implicit operator PropertyValue(string value) => new(value);
        public static implicit operator string(PropertyValue pvalue) => pvalue.ToString();

        public static implicit operator PropertyValue(sbyte value) => new(value);
        public static implicit operator sbyte(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            sbyte.TryParse(s, out sbyte value) ? new ConversionResult<sbyte>(value) : default
        );

        public static implicit operator PropertyValue(byte value) => new(value);
        public static implicit operator byte(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            byte.TryParse(s, out byte value) ? new ConversionResult<byte>(value) : default
        );

        public static implicit operator PropertyValue(short value) => new(value);
        public static implicit operator short(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            short.TryParse(s, out short value) ? new ConversionResult<short>(value) : default
        );

        public static implicit operator PropertyValue(ushort value) => new(value);
        public static implicit operator ushort(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            ushort.TryParse(s, out ushort value) ? new ConversionResult<ushort>(value) : default
        );

        public static implicit operator PropertyValue(int value) => new(value);
        public static implicit operator int(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            int.TryParse(s, out int value) ? new ConversionResult<int>(value) : default
        );

        public static implicit operator PropertyValue(uint value) => new(value);
        public static implicit operator uint(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            uint.TryParse(s, out uint value) ? new ConversionResult<uint>(value) : default
        );

        public static implicit operator PropertyValue(long value) => new(value);
        public static implicit operator long(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            long.TryParse(s, out long value) ? new ConversionResult<long>(value) : default
        );

        public static implicit operator PropertyValue(ulong value) => new(value);
        public static implicit operator ulong(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            ulong.TryParse(s, out ulong value) ? new ConversionResult<ulong>(value) : default
        );

        public static implicit operator PropertyValue(float value) => new(value);
        public static implicit operator float(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            float.TryParse(s, out float value) ? new ConversionResult<float>(value) : default
        );

        public static implicit operator PropertyValue(double value) => new(value);
        public static implicit operator double(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            double.TryParse(s, out double value) ? new ConversionResult<double>(value) : default
        );

        public static implicit operator PropertyValue(decimal value) => new(value);
        public static implicit operator decimal(PropertyValue pvalue) => ConvertTo(pvalue, s =>
            decimal.TryParse(s, out decimal value) ? new ConversionResult<decimal>(value) : default
        );

        public static implicit operator PropertyValue(DateTime value) =>
            new(value, value.ToString(Ini.Config.Types.DateFormat, CultureInfo.InvariantCulture));
        public static implicit operator DateTime(PropertyValue pvalue) => ConvertTo(pvalue, s => 
            DateTime.TryParseExact(s, Ini.Config.Types.DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime value)
                ? new ConversionResult<DateTime>(value) : default
        );

        private static T ConvertTo<T>(PropertyValue pvalue, Func<string, ConversionResult<T>> converter)
            where T : struct
        {
            if (pvalue._value is T typedValue)
                return typedValue;

            string stringValue = pvalue.ToString();
            if (stringValue == null)
                throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, ErrorMessages.CannotCastPropertyValue, typeof(T).Name));

            ConversionResult<T> result = converter(stringValue);

            if (!result.CanConvert)
                throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, ErrorMessages.CannotCastPropertyValue, typeof(T).Name));
            return result.Value;
        }

        public static implicit operator PropertyValue(bool value) =>
            new(value, value ? Ini.Config.Types.TrueString : Ini.Config.Types.FalseString);

        public static implicit operator bool(PropertyValue pvalue)
        {
            if (pvalue._value is bool boolValue)
                return boolValue;

            string stringValue = pvalue.ToString()?.ToUpperInvariant() ?? string.Empty;

            switch (stringValue)
            {
                case "0":
                case "F":
                case "N":
                case "OFF":
                case "NO":
                case "DISABLED":
                case "FALSE":
                    return false;
                case "1":
                case "T":
                case "Y":
                case "ON":
                case "YES":
                case "ENABLED":
                case "TRUE":
                    return true;
            }

            if (stringValue == Ini.Config.Types.TrueString.ToUpperInvariant())
                return true;
            if (stringValue == Ini.Config.Types.FalseString.ToUpperInvariant())
                return false;

#pragma warning disable S3877 // Exceptions should not be thrown from unexpected methods
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, ErrorMessages.CannotCastPropertyValue, "boolean"), nameof(pvalue));
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
#pragma warning restore S3877 // Exceptions should not be thrown from unexpected methods
        }

        public static bool operator ==(PropertyValue value1, PropertyValue value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(PropertyValue value1, PropertyValue value2)
        {
            return !(value1 == value2);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is PropertyValue value && Equals(value);
        }

        /// <inheritdoc/>
        public bool Equals(PropertyValue other)
        {
            return EqualityComparer<Type>.Default.Equals(_type, other._type);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return -331038658 + EqualityComparer<Type>.Default.GetHashCode(_type);
        }

        /// <summary>
        ///     Returns whether the property has no value.
        /// </summary>
        /// <returns><c>True</c>, if the value is empty; otherwise <c>false</c>.</returns>
        public bool IsEmpty()
        {
            return _value == null && _stringValue == null;
        }

        /// <summary>
        ///     Represents an empty property value.
        /// </summary>
        public static readonly PropertyValue Empty = new(null);
    }

    internal readonly struct ConversionResult<T>
        where T : struct
    {
        internal ConversionResult(T value)
        {
            CanConvert = true;
            Value = value;
        }

        internal bool CanConvert { get; }

        internal T Value { get; }
    }
}