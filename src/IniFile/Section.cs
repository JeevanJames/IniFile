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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using IniFile.Items;

namespace IniFile
{
    /// <summary>
    ///     <para>Represents a section object in an INI.</para>
    ///     <para>A section is also a collection of <see cref="Property"/> instances.</para>
    /// </summary>
    public sealed partial class Section : MajorIniItem, IPaddedItem<SectionPadding>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Section"/> class.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <param name="items">
        ///     Collection of strings that represent the comments and blank lines of the section.
        ///     If the string is <c>null</c>, an empty string or a whitespace string, then a
        ///     <see cref="BlankLine"/> object is created, otherwise a <see cref="Comment"/> is created.
        /// </param>
        /// <inheritdoc/>
        public Section(string name, params string[] items) : base(name, items)
        {
        }

        /// <summary>
        ///     Padding details of this <see cref="Section"/>.
        /// </summary>
        public SectionPadding Padding { get; } = new SectionPadding();

        /// <inheritdoc/>
        public override string ToString() =>
            $"{Padding.Left.ToString()}[{Padding.InsideLeft.ToString()}{Name}{Padding.InsideRight.ToString()}]{Padding.Right.ToString()}";
    }

    public sealed partial class Section : IList<Property>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<Property> _properties = new List<Property>();

        public Property this[int index]
        {
            get => _properties[index];
            set => _properties[index] = value;
        }

        /// <summary>
        ///     Gets or sets the value of the property given its name.
        /// </summary>
        /// <param name="name">The name of the property to get or set.</param>
        /// <returns>The value of the property.</returns>
        public string this[string name]
        {
            get => _properties.FirstOrDefault(p => p.Name == name)?.Value;
            set
            {
                Property property = _properties.FirstOrDefault(p => p.Name == name);
                if (property == null)
                {
                    property = new Property(name, value);
                    _properties.Add(property);
                }
                else
                    property.Value = value;
            }
        }

        public BooleanConverter AsBool => new BooleanConverter(this);

        public EnumConverter<TEnum> AsEnum<TEnum>()
            where TEnum : struct, IComparable =>
            new EnumConverter<TEnum>(this);

        public IntegerConverter AsInteger => new IntegerConverter(this);

        public NumberConverter AsNumber => new NumberConverter(this);

        /// <summary>
        ///     The number of properties in this section.
        /// </summary>
        public int Count => _properties.Count;

        public bool IsReadOnly => false;

        public void Add(Property item)
        {
            _properties.Add(item);
        }

        public void Clear()
        {
            _properties.Clear();
        }

        public bool Contains(Property item) =>
            _properties.Contains(item);

        public void CopyTo(Property[] array, int arrayIndex)
        {
            _properties.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Property> GetEnumerator() =>
            _properties.GetEnumerator();

        public int IndexOf(Property item) =>
            _properties.IndexOf(item);

        public void Insert(int index, Property item)
        {
            _properties.Insert(index, item);
        }

        public bool Remove(Property item) =>
            _properties.Remove(item);

        public void RemoveAt(int index)
        {
            _properties.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            _properties.GetEnumerator();
    }

    public sealed class BooleanConverter
    {
        private readonly Section _section;

        internal BooleanConverter(Section section)
        {
            _section = section;
        }

        public bool this[string name]
        {
            get
            {
                string value = _section[name];
                switch (value)
                {
                    case "0":
                    case "f":
                    case "n":
                    case "off":
                    case "no":
                    case "disabled":
                        return false;
                    case "1":
                    case "t":
                    case "y":
                    case "on":
                    case "yes":
                    case "enabled":
                        return true;
                    default:
                        throw new Exception($"'value' is not a boolean value.");
                }
            }
            set => _section[name] = value ? "1" : "0";
        }
    }

    public sealed class EnumConverter<TEnum>
        where TEnum : struct, IComparable
    {
        private readonly Section _section;

        internal EnumConverter(Section section)
        {
#if NETSTANDARD1_3
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
#else
            if (!typeof(TEnum).IsEnum)
#endif
                throw new Exception($"{typeof(TEnum).FullName} is not an enum type.");
            _section = section;
        }

        public TEnum Get(string name, bool caseSensitive = false) =>
            (TEnum)Enum.Parse(typeof(TEnum), _section[name], !caseSensitive);

        public void Set(string name, TEnum value)
        {
            _section[name] = value.ToString();
        }
    }

    public sealed class IntegerConverter
    {
        private readonly Section _section;

        internal IntegerConverter(Section section)
        {
            _section = section;
        }

        public long this[string name]
        {
            get => long.Parse(_section[name]);
            set => _section[name] = value.ToString();
        }
    }

    public sealed class NumberConverter
    {
        private readonly Section _section;

        internal NumberConverter(Section section)
        {
            _section = section;
        }

        public double this[string name]
        {
            get => double.Parse(_section[name]);
            set => _section[name] = value.ToString();
        }
    }
}