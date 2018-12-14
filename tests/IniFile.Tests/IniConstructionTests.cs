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
using System.IO;

using Moq;

using Shouldly;

using Xunit;

namespace IniFile.Tests
{
    public sealed class IniConstructionTests
    {
        [Fact]
        public void Ctor_creates_instance()
        {
            var ini = new Ini();

            ini.ShouldNotBeNull();
        }

        [Fact]
        public void Ctor_with_null_settings()
        {
            var ini = new Ini(null);

            ini.ShouldNotBeNull();
        }

        [Fact]
        public void Ctor_with_non_null_settings()
        {
            var ini = new Ini(IniLoadSettings.Default);

            ini.ShouldNotBeNull();
        }

        [Fact]
        public void Ctor_with_null_stream_will_throw()
        {
            Should.Throw<ArgumentNullException>(() => new Ini((Stream) null));
        }

        [Fact]
        public void Ctor_with_non_readable_stream_will_throw()
        {
            var mock = new Mock<Stream>();
            mock.Setup(s => s.CanRead).Returns(false);

            Should.Throw<ArgumentException>(() => new Ini(mock.Object));
        }

        [Fact]
        public void Ctor_with_null_textreader_should_throw()
        {
            Should.Throw<ArgumentNullException>(() => new Ini((TextReader)null));
        }

        [Fact]
        public void Load_with_null_content_string()
        {
            Should.Throw<ArgumentNullException>(() => Ini.Load(null));
        }
    }
}