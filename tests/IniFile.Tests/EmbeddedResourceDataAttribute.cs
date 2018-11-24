using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace IniFile.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class BaseEmbeddedResourceDataAttribute : DataAttribute
    {
        private readonly IReadOnlyList<string> _nameInputs;

        public BaseEmbeddedResourceDataAttribute(params string[] nameInputs)
        {
            if (nameInputs == null)
                throw new ArgumentNullException(nameof(nameInputs));
            _nameInputs = nameInputs.Where(n => !string.IsNullOrWhiteSpace(n)).ToList();
            if (_nameInputs.Count == 0)
                throw new ArgumentException("Specify at least one valid name.", nameof(nameInputs));
        }

        public Assembly Assembly { get; set; }

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public bool DetectEncoding { get; set; } = false;

        public override sealed IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null)
                throw new ArgumentNullException(nameof(testMethod));

            Assembly assembly = Assembly ?? Assembly.GetExecutingAssembly();
            Encoding encoding = Encoding ?? Encoding.UTF8;

            foreach (string nameInput in _nameInputs)
            {
                IEnumerable<string> resourceNames = GetResourceNames(assembly, nameInput);

                foreach (string resourceName in resourceNames)
                {
                    using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                    using (var reader = new StreamReader(stream, encoding, DetectEncoding))
                    {
                        string content = reader.ReadToEnd();
                        yield return new object[] {content};
                    }
                }
            }
        }

        protected abstract IEnumerable<string> GetResourceNames(Assembly assembly, string nameInput);
    }

    public sealed class EmbeddedResourceDataAttribute : BaseEmbeddedResourceDataAttribute
    {
        public EmbeddedResourceDataAttribute(params string[] resourceNames) : base(resourceNames)
        {
        }

        protected override IEnumerable<string> GetResourceNames(Assembly assembly, string nameInput)
        {
            yield return nameInput;
        }
    }

    public sealed class EmbeddedResourceDataByPatternAttribute : BaseEmbeddedResourceDataAttribute
    {
        public EmbeddedResourceDataByPatternAttribute(params string[] namePatterns) : base(namePatterns)
        {
        }

        protected override IEnumerable<string> GetResourceNames(Assembly assembly, string nameInput)
        {
            var namePattern = new Regex(nameInput);
            return assembly.GetManifestResourceNames()
                .Where(name => namePattern.IsMatch(name));
        }
    }
}