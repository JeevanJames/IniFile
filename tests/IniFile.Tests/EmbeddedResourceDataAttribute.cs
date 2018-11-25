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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using Xunit.Sdk;

namespace IniFile.Tests
{
    /// <summary>
    ///     Base class for xUnit data attributes that extract data from one or more embedded
    ///     resources in assemblies.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class BaseEmbeddedResourceDataAttribute : DataAttribute
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IReadOnlyList<string> _resourceNames;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Encoding _encoding = Encoding.UTF8;

        public BaseEmbeddedResourceDataAttribute(params string[] resourceNames)
        {
            if (resourceNames == null)
                throw new ArgumentNullException(nameof(resourceNames));
            _resourceNames = resourceNames.Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
            if (_resourceNames.Count == 0)
                throw new ArgumentException("Specify at least one valid name.", nameof(resourceNames));
        }

        /// <summary>
        ///     Indicates whether the specified resource names are regular expressions.
        /// </summary>
        public bool UseAsRegex { get; set; }

        /// <summary>
        ///     The assembly to load the resources from. If not specified, this defaults to the
        ///     currently executing assembly.
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        ///     Gets or sets the character encoding to use when loading the resource data.
        /// </summary>
        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        ///     Gets or sets whether to automatically detect the character encoding when loading
        ///     the resource data.
        /// </summary>
        public bool DetectEncoding { get; set; } = false;

        public sealed override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null)
                throw new ArgumentNullException(nameof(testMethod));

            Assembly assembly = Assembly ?? Assembly.GetExecutingAssembly();
            Encoding encoding = Encoding ?? Encoding.UTF8;

            var matchingResources = new HashSet<string>(StringComparer.Ordinal);

            // Figure out all the resources to load. Avoid duplicate resource names.
            foreach (string resourceName in _resourceNames)
            {
                IEnumerable<string> resolvedResources = GetResourceNames(assembly, resourceName);
                foreach (string resolvedResource in resolvedResources)
                    matchingResources.Add(resolvedResource);
            }

            foreach (string matchingResource in matchingResources)
            {
                using (Stream stream = assembly.GetManifestResourceStream(matchingResource))
                using (var reader = new StreamReader(stream, encoding, DetectEncoding))
                {
                    string content = reader.ReadToEnd();
                    IEnumerable<object[]> data = GetData(content);
                    foreach (object[] dataItem in data)
                        yield return dataItem;
                }
            }
        }

        /// <summary>
        ///     Given the resource content, deriving classes should override this method to extract
        ///     the required data and return as an <c>IEnumerable&lt;object[]&gt;</c>
        /// </summary>
        /// <param name="resourceContent">The content of the resource, as a string.</param>
        /// <returns>A collection of extracted data.</returns>
        protected abstract IEnumerable<object[]> GetData(string resourceContent);

        private IEnumerable<string> GetResourceNames(Assembly assembly, string resourceName)
        {
            if (!UseAsRegex)
                return new[] { resourceName };

            var regex = new Regex(resourceName);
            return assembly.GetManifestResourceNames()
                .Where(name => regex.IsMatch(name));
        }
    }

    /// <summary>
    ///     Provides a data source for a data theory, with the data coming as the content of one or
    ///     more assembly embedded resources.
    /// </summary>
    public sealed class EmbeddedResourceContentAttribute : BaseEmbeddedResourceDataAttribute
    {
        public EmbeddedResourceContentAttribute(params string[] resourceNames) : base(resourceNames)
        {
        }

        protected override IEnumerable<object[]> GetData(string resourceContent)
        {
            yield return new[] { resourceContent };
        }
    }

    /// <summary>
    ///     Provides a data source for a data theory, with the data coming from one or more assembly
    ///     embedded resources, where each data item is a single line in the embedded resource content.
    /// </summary>
    public sealed class EmbeddedResourceLinesAttribute : BaseEmbeddedResourceDataAttribute
    {
        private readonly Func<string, object> _lineConverter;

        public EmbeddedResourceLinesAttribute(Func<string, object> lineConverter, params string[] resourceNames)
            : base(resourceNames)
        {
            if (lineConverter == null)
                throw new ArgumentNullException(nameof(lineConverter));
            _lineConverter = lineConverter;
        }

        public EmbeddedResourceLinesAttribute(params string[] resourceNames) : base(resourceNames)
        {
        }

        protected override IEnumerable<object[]> GetData(string resourceContent)
        {
            string[] lines = Regex.Split(resourceContent, @"\r\n|\r|\n");
            IEnumerable<object> data = _lineConverter != null ? lines.Select(_lineConverter) : lines;
            return data.Select(line => new object[] { line });
        }
    }

    /// <summary>
    ///     Provides a data source for a data theory, with the data coming from one or more assembly
    ///     embedded resources, where each resource is a JSON structure that can be deserialized
    ///     into the specified type.
    /// </summary>
    public sealed class EmbeddedResourceAsJsonAttribute : BaseEmbeddedResourceDataAttribute
    {
        private readonly Type _type;

        public EmbeddedResourceAsJsonAttribute(Type type, params string[] resourceNames) : base(resourceNames)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            _type = type;
        }

        protected override IEnumerable<object[]> GetData(string resourceContent)
        {
            var allData = JToken.Parse(resourceContent);
            if (allData is JArray arr)
            {
                Type listType = typeof(List<>).MakeGenericType(_type);
                var data = (IList)arr.ToObject(listType);
                foreach (object dataItem in data)
                    yield return new object[] {dataItem};
            }
            else if (allData is JObject obj)
            {
                object data = obj.ToObject(_type);
                yield return new object[] {data};
            }
        }
    }
}