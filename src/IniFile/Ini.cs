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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IniFile.Items;

namespace IniFile
{
    /// <summary>
    ///     <para>In-memory object representation of an INI file.</para>
    ///     <para>This class is a read-only collection of <see cref="Section"/> objects.</para>
    /// </summary>
    [DebuggerDisplay("INI file - {Count} sections")]
    public sealed partial class Ini : IReadOnlyList<Ini.Section>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<ITopLevelIniItem> _items = new List<ITopLevelIniItem>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly StringComparison _comparison;

        /// <summary>
        ///     Initializes a new empty instance of the <see cref="Ini"/> class with the default
        ///     settings.
        /// </summary>
        public Ini() : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new empty instance of the <see cref="Ini"/> class with the specified
        ///     settings.
        /// </summary>
        /// <param name="settings">The Ini file settings.</param>
        public Ini(IniLoadSettings settings)
        {
            settings = settings ?? IniLoadSettings.Default;
            _comparison = settings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ini"/> class and loads the data from
        ///     the specified file.
        /// </summary>
        /// <param name="iniFile">The .ini file to load from.</param>
        /// <param name="settings">Optional Ini file settings.</param>
        public Ini(FileInfo iniFile, IniLoadSettings settings = null)
        {
            if (iniFile == null)
                throw new ArgumentNullException(nameof(iniFile));
            if (!iniFile.Exists)
                throw new FileNotFoundException($"INI file '{iniFile.FullName}' does not exist", iniFile.FullName);

            settings = settings ?? IniLoadSettings.Default;
            _comparison = settings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            using (var reader = new StreamReader(iniFile.FullName, settings.Encoding ?? Encoding.UTF8, settings.DetectEncoding))
                ParseIniFile(reader);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ini"/> class and loads the data from
        ///     the specified file.
        /// </summary>
        /// <param name="iniFilePath">The path to the .ini file.</param>
        /// <param name="settings">Optional Ini file settings.</param>
        public Ini(string iniFilePath, IniLoadSettings settings = null)
        {
            if (iniFilePath == null)
                throw new ArgumentNullException(nameof(iniFilePath));
            if (!File.Exists(iniFilePath))
                throw new FileNotFoundException($"INI file '{iniFilePath}' does not exist", iniFilePath);

            settings = settings ?? IniLoadSettings.Default;
            _comparison = settings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            using (var reader = new StreamReader(iniFilePath, settings.Encoding ?? Encoding.UTF8, settings.DetectEncoding))
                ParseIniFile(reader);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ini"/> class and loads the data from
        ///     the specified stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        /// <param name="settings">Optional Ini file settings.</param>
        public Ini(Stream stream, IniLoadSettings settings = null)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new ArgumentException("Cannot read from specified stream", nameof(stream));

            settings = settings ?? IniLoadSettings.Default;
            _comparison = settings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            using (var reader = new StreamReader(stream, settings.Encoding ?? Encoding.UTF8, settings.DetectEncoding))
                ParseIniFile(reader);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ini"/> class and loads the data from
        ///     the specified <see cref="TextReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> to load from.</param>
        /// <param name="settings">Optional Ini file settings.</param>
        public Ini(TextReader reader, IniLoadSettings settings = null)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            settings = settings ?? IniLoadSettings.Default;
            _comparison = settings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            ParseIniFile(reader);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ini"/> class and loads the data from
        ///     the specified string.
        /// </summary>
        /// <param name="content">The string representing the Ini content.</param>
        /// <param name="settings">Optional Ini file settings.</param>
        /// <returns>An instance of the <see cref="Ini"/> class.</returns>
        public static Ini Load(string content, IniLoadSettings settings = null)
        {
            using (var reader = new StringReader(content))
                return new Ini(reader, settings);
        }

        private void ParseIniFile(TextReader reader)
        {
            Section currentSection = null;
            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                IniItem item1 = IniItemFactory.CreateItem(line);

                IIniItem item = CreateIniItem(line);
                if (item == null)
                    throw new FormatException($"Invalid line in .INI file - '{line}'.");

                if (item is Section section)
                {
                    _items.Add(section);
                    currentSection = section;
                }
                else if (currentSection != null && item is ISectionItem sectionItem)
                    currentSection.Add(sectionItem);
                else if (item is ITopLevelIniItem topLevelItem)
                    _items.Add(topLevelItem);
            }
        }

        private static IIniItem CreateIniItem(string line)
        {
            foreach (IIniItem factory in ItemFactories)
            {
                IIniItem item = factory.TryCreate(line);
                if (item != null)
                    return item;
            }

            return null;
        }

        private static readonly List<IIniItem> ItemFactories = new List<IIniItem>
        {
            new Property(),
            new Section(),
            new Comment(),
            new BlankLine()
        };

        /// <summary>
        ///     Collection of all items in the Ini.
        /// </summary>
        public IList<ITopLevelIniItem> AllItems => _items;

        /// <summary>
        ///     <para>Adds a section, comment or blank line to the Ini object.</para>
        ///     <para>
        ///         Adds at the end of the collection, unless the <c>beforeItem</c> parameter is
        ///         specified, in which case the new item is inserted before the <c>beforeItem</c> item.
        ///     </para>
        /// </summary>
        /// <typeparam name="T">The type of the new item being added.</typeparam>
        /// <param name="item">The new item to insert.</param>
        /// <param name="beforeItem">
        ///     The existing item in the Ini file before which the new item should be inserted.
        /// </param>
        /// <returns>The newly added item.</returns>
        public T Add<T>(T item, ITopLevelIniItem beforeItem = null)
            where T : ITopLevelIniItem
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (beforeItem != null)
            {
                int index = _items.IndexOf(beforeItem);
                if (index < 0)
                    throw new ArgumentException($"Cannot find location to insert the new item.", nameof(beforeItem));
                _items.Insert(index, item);
            }
            else
                _items.Add(item);
            return item;
        }

        public Section AddSection(string name, ITopLevelIniItem beforeItem = null) =>
            Add(new Section(name), beforeItem);

        public BlankLine AddBlankLine(ITopLevelIniItem beforeItem = null) =>
            Add(new BlankLine(), beforeItem);

        public Comment AddComment(string text, ITopLevelIniItem beforeItem = null) =>
            Add(new Comment(text), beforeItem);

        public void SaveTo(string filePath)
        {
            using (StreamWriter writer = File.CreateText(filePath))
                SaveTo(writer);
        }

        public void SaveTo(FileInfo file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            using (StreamWriter writer = File.CreateText(file.FullName))
                SaveTo(writer);
        }

        public void SaveTo(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            using (var writer = new StreamWriter(stream))
                SaveTo(writer);
        }

        public async Task SaveToAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            using (var writer = new StreamWriter(stream))
                await SaveToAsync(writer);
        }

        public void SaveTo(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            InternalSave(writer);
            writer.Flush();
        }

        public async Task SaveToAsync(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            await InternalSaveAsync(writer);
            writer.Flush();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
                InternalSave(writer);
            return sb.ToString();
        }

        private void InternalSave(TextWriter writer)
        {
            foreach (ITopLevelIniItem topLevelIniItem in AllItems)
            {
                topLevelIniItem.Write(writer);
                if (topLevelIniItem is Section section)
                {
                    foreach (ISectionItem sectionItem in section.AllItems)
                        sectionItem.Write(writer);
                }
            }
        }

        private async Task InternalSaveAsync(TextWriter writer)
        {
            foreach (ITopLevelIniItem topLevelIniItem in AllItems)
            {
                await topLevelIniItem.Write(writer);
                if (topLevelIniItem is Section section)
                {
                    foreach (ISectionItem sectionItem in section.AllItems)
                        await sectionItem.Write(writer);
                }
            }
        }

        public IEnumerator<Section> GetEnumerator() =>
            _items.OfType<Section>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public int Count =>
            _items.OfType<Section>().Count();

        public Section this[int index] =>
            _items.OfType<Section>().ElementAt(index);

        public Section this[string name] =>
            _items.OfType<Section>().FirstOrDefault(s => s.Name.Equals(name, _comparison));
    }
}