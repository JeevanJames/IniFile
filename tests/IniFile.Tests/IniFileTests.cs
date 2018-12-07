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
            Ini.Config
                .AllowHashForComments()
                .SetSectionPaddingDefaults(insideLeft: 1, insideRight: 1)
                .SetPropertyPaddingDefaults(left: 4);

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

        [Fact]
        public void Basic_create_test()
        {
            Ini.Config
                .AllowHashForComments(true)
                .SetSectionPaddingDefaults(insideLeft: 1, insideRight: 1)
                .SetPropertyPaddingDefaults(left: 4);

            var ini = new Ini
            {
                new Section("Players", "This section defines the players")
                {
                    new Property("Player1", "The Flash"),
                    new Property("Player2", "Superman")
                },
                new Section("The Flash", string.Empty)
                {
                    ["Level"] = 9,
                    ["Power"] = "Superspeed",
                    ["Masked"] = true
                },
                new Section("Superman", string.Empty)
                {
                    ["Level"] = 9,
                    ["Power"] = "Superstrength,heat vision",
                    ["Masked"] = false,
                    ["MultiLine"] = @"This is line one
This is line 2
This is line 3 and the last line"
                }
            };

            ini.ShouldNotBeNull();
            ini.Count.ShouldBe(3);

            Section playersSection = ini["Players"];
            playersSection.ShouldNotBeNull();
            playersSection.Items.Count.ShouldBe(1);
            playersSection.Items[0].ShouldBeOfType<Comment>();
            playersSection.Count.ShouldBe(2);
            playersSection["Player1"].ShouldBe("The Flash");
            playersSection["Player2"].ShouldBe("Superman");

            Section flashSection = ini["The Flash"];
            flashSection.ShouldNotBeNull();
            flashSection.Items.Count.ShouldBe(1);
            flashSection.Items[0].ShouldBeOfType<BlankLine>();
            flashSection.Count.ShouldBe(3);
            ((int)flashSection["Level"]).ShouldBe(9);
            flashSection["Power"].ShouldBe("Superspeed");

            Section supermanSection = ini["Superman"];
            supermanSection.ShouldNotBeNull();
            supermanSection.Items.Count.ShouldBe(1);
            supermanSection.Items[0].ShouldBeOfType<BlankLine>();
            supermanSection.Count.ShouldBe(4);
            ((int)supermanSection["Level"]).ShouldBe(9);
            supermanSection["Power"].ShouldBe("Superstrength,heat vision");

            flashSection["Level"] = 10;
            int level = flashSection["Level"];
            level.ShouldBe(10);
        }

        //[Theory(Skip = "Need to handle cross-platform new line characters.")]
        //[EmbeddedResourceContent("IniFile.Tests.Players.ini")]
        //public void Ensure_format_is_retained(string validIni)
        //{
        //    var ini = Ini.Load(validIni);
        //    string iniContent = ini.ToString();
        //    iniContent.ShouldBe(validIni);
        //}

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

        [Theory]
        [EmbeddedResourceContent("IniFile.Tests.Data.Unformatted.ini")]
        public void Format_ini(string iniContent)
        {
            Ini ini = Ini.Load(iniContent);

            ini.Format(new IniFormatOptions { RemoveSuccessiveBlankLines = true });
            string formatted = ini.ToString();

            formatted.ShouldNotBeNull();
        }
    }
}