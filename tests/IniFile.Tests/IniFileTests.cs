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
using Shouldly;

using Xunit;
using Xunit.DataAttributes;

namespace IniFile.Tests
{
    public sealed class IniTests
    {
        [Theory]
        [EmbeddedResourceContent("IniFile.Tests.Players.ini")]
        public void Basic_tests(string validIni)
        {
            var ini = Ini.Load(validIni);

            ini.Count.ShouldBe(3);
            ini[0].Name.ShouldBe("Game State");
            ini[1].Name.ShouldBe("Jeevan");
            ini[2].Name.ShouldBe("Merina");

            Section section = ini[0];
            section.Count.ShouldBe(2);
            section["Player1"].ShouldBe("Ryan");
            section["Player2"].ShouldBe("Emma");
        }

        [Theory]
        [EmbeddedResourceContent("IniFile.Tests.Players.ini")]
        public void Ensure_format_is_retained(string validIni)
        {
            var ini = Ini.Load(validIni);
            string iniContent = ini.ToString();
            iniContent.ShouldBe(validIni);
        }

        [Theory]
        [EmbeddedResourceContent("IniFile.Tests.Data.UnrecognizedLine.ini")]
        public void Unrecognized_line_throws(string iniContent)
        {
            Should.Throw<FormatException>(() => Ini.Load(iniContent));
        }

        [Theory]
        [EmbeddedResourceContent("IniFile.Tests.Data.PropertyWithoutSection.ini")]
        public void Property_without_section_throws(string iniContent)
        {
            Should.Throw<FormatException>(() => Ini.Load(iniContent));
        }

        [Theory]
        [EmbeddedResourceContent("IniFile.Tests.Data.EmptySections.ini")]
        public void Empty_sections_are_allowed(string iniContent)
        {
            Ini ini = Ini.Load(iniContent);

            ini.ShouldNotBeNull();
            ini.Count.ShouldBe(3);
            ini.ShouldAllBe(section => section.Count == 0);
        }

        [Theory]
        [EmbeddedResourceContent("IniFile.Tests.Data.EmptyProperties.ini")]
        public void Empty_properties_are_allowed(string iniContent)
        {
            Ini ini = Ini.Load(iniContent);

            ini.ShouldNotBeNull();
            ini["Section1"]["Key1"].ShouldBe(string.Empty);
            ini["Section1"]["Key2"].ShouldBe(string.Empty);
            ini["Section2"]["Key4"].ShouldBe(string.Empty);
        }
    }
}