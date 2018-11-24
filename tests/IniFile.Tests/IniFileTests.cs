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

namespace IniFile.Tests
{
    public sealed class IniTests
    {
        [Theory]
        [EmbeddedResourceDataByPattern(@".+\.ini")]
        public void Basic_tests(string validIni)
        {
            var ini = Ini.Load(validIni);

            ini.Count.ShouldBe(3);
            ini[0].Name.ShouldBe("Game State");
            ini[1].Name.ShouldBe("Jeevan");
            ini[2].Name.ShouldBe("Merina");

            Ini.Section section = ini[0];
            section.Count.ShouldBe(2);
            section["Player1"].ShouldBe("Ryan");
            section["Player2"].ShouldBe("Emma");
        }

        [Fact]
        public void Save_tests()
        {
            var ini = new Ini();

            var section = new Ini.Section("Test")
            {
                new Ini.Comment("Player definitions"),
                new Ini.Property("Player1", "Jeevan"),
                new Ini.Property("Player2", "Merina"),
                new Ini.BlankLine()
            };
            ini.Add(section);

            ini.Add(new Ini.Section("Jeevan")
            {
                new Ini.Property("Powers", "Superspeed,Super strength"),
                new Ini.Property("Costume", "Scarlet"),
                new Ini.BlankLine()
            });

            ini.Add(new Ini.Section("Merina")
            {
                new Ini.Property("Powers", "Stretchability, Invisibility"),
                new Ini.Property("Costume", "Blue"),
                new Ini.BlankLine()
            });

            ini.SaveTo(@"D:\Temp\Test.ini");
        }

        [Theory]
        [EmbeddedResourceData("IniFile.Tests.Players.ini")]
        public void Add_Inserts_at_correct_position(string iniContent)
        {
            Ini ini = Ini.Load(iniContent);

            Ini.Section jeevanSection = ini["Jeevan"];

            var ryanSection = new Ini.Section("Ryan")
            {
                new Ini.Property("Level", "5"),
                new Ini.Property("Karma", "7.33"),
                new Ini.Property("Weapons", "BFG7000,Fists")
            };
            ini.Add(ryanSection, beforeItem: jeevanSection);

            ini[1].ShouldBeSameAs(ryanSection);
            ini[2].ShouldBeSameAs(jeevanSection);
        }
    }
}