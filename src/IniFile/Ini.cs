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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniFile.Items;

namespace IniFile
{
    public sealed partial class Ini : IReadOnlyList<Ini.Section>
    {
        private readonly List<ITopLevelIniItem> _items = new List<ITopLevelIniItem>();
        private readonly StringComparison _comparison;

        public Ini() : this(null)
        {
        }

        public Ini(IniLoadSettings settings)
        {
            settings = settings ?? IniLoadSettings.Default;
            _items = new List<ITopLevelIniItem>();
            _comparison = settings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        public Ini(FileInfo iniFile, IniLoadSettings settings)
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

        public Ini(string iniFilePath, IniLoadSettings settings)
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

        public Ini(Stream stream, IniLoadSettings settings)
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

        public static Ini Load(string content, IniLoadSettings settings)
        {
            settings = settings ?? IniLoadSettings.Default;
            Encoding encoding = settings.Encoding ?? Encoding.UTF8;

            byte[] contentBytes = encoding.GetBytes(content);
            var stream = new MemoryStream(contentBytes.Length);
            stream.Write(contentBytes, 0, contentBytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return new Ini(stream, settings);
        }

        private IIniItem CreateIniItem(string line)
        {
            foreach (IIniItem factory in ItemFactories)
            {
                IIniItem item = factory.TryCreate(line);
                if (item != null)
                    return item;
            }

            return null;
        }

        private void ParseIniFile(TextReader reader)
        {
            Section currentSection = null;
            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
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

        private static readonly List<IIniItem> ItemFactories = new List<IIniItem>
        {
            new Property(),
            new Section(),
            new Comment(),
            new BlankLine()
        };

        public void Add(ITopLevelIniItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            _items.Add(item);
        }

        public Section AddSection(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("message", nameof(name));
            var section = new Section(name);
            _items.Add(section);
            return section;
        }

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
            foreach (ITopLevelIniItem topLevelIniItem in _items)
            {
                topLevelIniItem.Write(writer);
                if (topLevelIniItem is Section section)
                {
                    foreach (ISectionItem sectionItem in section)
                        sectionItem.Write(writer);
                }
            }
        }

        private async Task InternalSaveAsync(TextWriter writer)
        {
            foreach (ITopLevelIniItem topLevelIniItem in _items)
            {
                await topLevelIniItem.Write(writer);
                if (topLevelIniItem is Section section)
                {
                    foreach (ISectionItem sectionItem in section)
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