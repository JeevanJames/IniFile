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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IniFile.Items
{
    public sealed partial class Section : MajorIniItem, IPaddedItem<SectionPadding>
    {
        public Section(string name) : base(name)
        {
        }

        public SectionPadding Padding { get; } = new SectionPadding();

        public override string ToString() =>
            $"{Padding.Left.ToString()}[{Padding.InsideLeft.ToString()}{Name}{Padding.InsideRight.ToString()}]{Padding.Right.ToString()}";
    }

    public sealed partial class Section : IList<Property>
    {
        private readonly List<Property> _properties = new List<Property>();

        public Property this[int index]
        {
            get => _properties[index];
            set => _properties[index] = value;
        }

        public string this[string name]
        {
            get => _properties.FirstOrDefault()?.Value;
            set => _properties.FirstOrDefault().Value = value;
        }

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
}