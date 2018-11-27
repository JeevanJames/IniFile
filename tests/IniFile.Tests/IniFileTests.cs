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

        [Fact]
        public void Save_tests()
        {
            var ini = new Ini();

            var section = new Section("Test")
            {
                new Property("Player1", "Jeevan",
                    new Comment("First player")),
                new Property("Player2", "Merina",
                    new Comment("Second player"))
            };
            ini.Add(section);

            ini.Add(new Section("Jeevan")
            {
                new Property("Powers", "Superspeed,Super strength"),
                new Property("Costume", "Scarlet")
            });

            ini.Add(new Section("Merina")
            {
                new Property("Powers", "Stretchability, Invisibility"),
                new Property("Costume", "Blue")
            });

            ini.Add(new Section("Ryan")
            {
                ["Powers"] = "Healing power",
                ["Costume"] = "Red"
            });

            ini.TrailingItems.Add(new BlankLine());
            ini.TrailingItems.Add(new Comment("This is a trailing comment"));
            ini.TrailingItems.Add(new BlankLine());
            ini.TrailingItems.Add(new BlankLine());

            ini.Format();

            string content = ini.ToString();

            content.ShouldNotBeNullOrWhiteSpace();
        }

        [Theory]
        [EmbeddedResourceContent("IniFile.Tests.Players.ini")]
        public void Add_Inserts_at_correct_position(string iniContent)
        {
            var ini = Ini.Load(iniContent);

            Section jeevanSection = ini["Jeevan"];

            var ryanSection = new Section("Ryan")
            {
                new Property("Level", "5"),
                new Property("Karma", "7.33"),
                new Property("Weapons", "BFG7000,Fists")
            };
            ini.Insert(1, ryanSection);

            ini[1].ShouldBeSameAs(ryanSection);
            ini[2].ShouldBeSameAs(jeevanSection);
        }
    }
}