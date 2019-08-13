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
            section["Player1"].ToString().ShouldBe("Ryan");
            section["Player2"].ToString().ShouldBe("Emma");
        }

        [Fact(DisplayName = "Basic INI creation test")]
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
            playersSection["Player1"].ToString().ShouldBe("The Flash");
            playersSection["Player2"].ToString().ShouldBe("Superman");

            Section flashSection = ini["The Flash"];
            flashSection.ShouldNotBeNull();
            flashSection.Items.Count.ShouldBe(1);
            flashSection.Items[0].ShouldBeOfType<BlankLine>();
            flashSection.Count.ShouldBe(3);
            ((int)flashSection["Level"]).ShouldBe(9);
            flashSection["Power"].ToString().ShouldBe("Superspeed");

            Section supermanSection = ini["Superman"];
            supermanSection.ShouldNotBeNull();
            supermanSection.Items.Count.ShouldBe(1);
            supermanSection.Items[0].ShouldBeOfType<BlankLine>();
            supermanSection.Count.ShouldBe(4);
            ((int)supermanSection["Level"]).ShouldBe(9);
            supermanSection["Power"].ToString().ShouldBe("Superstrength,heat vision");

            flashSection["Level"] = 10;
            int level = flashSection["Level"];
            level.ShouldBe(10);

            flashSection["Masked"] = "yes";
            bool masked = flashSection["Masked"];
            masked.ShouldBeTrue();
        }

        //[Theory(Skip = "Need to handle cross-platform new line characters.")]
        //[EmbeddedResourceContent("IniFile.Tests.Players.ini")]
        //public void Ensure_format_is_retained(string validIni)
        //{
        //    var ini = Ini.Load(validIni);
        //    string iniContent = ini.ToString();
        //    iniContent.ShouldBe(validIni);
        //}

        [Theory(DisplayName = "Throws on unrecognized line")]
        [EmbeddedResourceContent("IniFile.Tests.Data.UnrecognizedLine.ini")]
        public void Unrecognized_line_throws(string iniContent)
        {
            Should.Throw<FormatException>(() => Ini.Load(iniContent));
        }

        [Theory(DisplayName = "Throwns on a property without a section")]
        [EmbeddedResourceContent("IniFile.Tests.Data.PropertyWithoutSection.ini")]
        public void Property_without_section_throws(string iniContent)
        {
            Should.Throw<FormatException>(() => Ini.Load(iniContent));
        }

        [Theory(DisplayName = "Allows empty sections")]
        [EmbeddedResourceContent("IniFile.Tests.Data.EmptySections.ini")]
        public void Empty_sections_are_allowed(string iniContent)
        {
            Ini ini = Ini.Load(iniContent);

            ini.ShouldNotBeNull();
            ini.Count.ShouldBe(3);
            ini.ShouldAllBe(section => section.Count == 0);
        }

        [Theory(DisplayName = "Properties without values are allowed")]
        [EmbeddedResourceContent("IniFile.Tests.Data.EmptyProperties.ini")]
        public void Empty_properties_are_allowed(string iniContent)
        {
            Ini ini = Ini.Load(iniContent);

            ini.ShouldNotBeNull();
            ini["Section1"]["Key1"].ToString().ShouldBe(string.Empty);
            ini["Section1"]["Key2"].ToString().ShouldBe(string.Empty);
            ini["Section2"]["Key4"].ToString().ShouldBe(string.Empty);
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

        [Theory(DisplayName = "Can read typed properties")]
        [EmbeddedResourceContent("IniFile.Tests.Data.TypedProperties.ini")]
        public void Can_read_typed_properties(string iniContent)
        {
            Ini.Config.Types.DateFormat = "yyyy-MM-dd";

            Ini ini = Ini.Load(iniContent);

            Section dataSection = ini["TypedData"];
            string stringValue = dataSection["String"];
            sbyte sbyteValue = dataSection["SByte"];
            byte byteValue = dataSection["Byte"];
            short shortValue = dataSection["Short"];
            ushort ushortValue = dataSection["UShort"];
            int intValue = dataSection["Int"];
            uint uintValue = dataSection["UInt"];
            long longValue = dataSection["Long"];
            ulong ulongValue = dataSection["ULong"];
            float floatValue = dataSection["Float"];
            double doubleValue = dataSection["Double"];
            decimal decimalValue = dataSection["Decimal"];
            DateTime dateTimeValue = dataSection["DateTime"];

            Section falseBoolSection = ini["False Booleans"];
            bool falseBool1 = falseBoolSection["Bool1"];
            bool falseBool2 = falseBoolSection["Bool2"];
            bool falseBool3 = falseBoolSection["Bool3"];
            bool falseBool4 = falseBoolSection["Bool4"];
            bool falseBool5 = falseBoolSection["Bool5"];
            bool falseBool6 = falseBoolSection["Bool6"];
            bool falseBool7 = falseBoolSection["Bool7"];

            Section trueBoolSection = ini["True Booleans"];
            bool trueBool1 = trueBoolSection["Bool1"];
            bool trueBool2 = trueBoolSection["Bool2"];
            bool trueBool3 = trueBoolSection["Bool3"];
            bool trueBool4 = trueBoolSection["Bool4"];
            bool trueBool5 = trueBoolSection["Bool5"];
            bool trueBool6 = trueBoolSection["Bool6"];
            bool trueBool7 = trueBoolSection["Bool7"];

            Section enumSection = ini["Enum"];
            DayOfWeek dow = enumSection["Enum1"].AsEnum<DayOfWeek>();

            dateTimeValue.ShouldBe(new DateTime(2018, 12, 08));

            falseBool1.ShouldBeFalse();
            falseBool2.ShouldBeFalse();
            falseBool3.ShouldBeFalse();
            falseBool4.ShouldBeFalse();
            falseBool5.ShouldBeFalse();
            falseBool6.ShouldBeFalse();
            falseBool7.ShouldBeFalse();

            trueBool1.ShouldBeTrue();
            trueBool2.ShouldBeTrue();
            trueBool3.ShouldBeTrue();
            trueBool4.ShouldBeTrue();
            trueBool5.ShouldBeTrue();
            trueBool6.ShouldBeTrue();
            trueBool7.ShouldBeTrue();

            dow.ShouldBe(DayOfWeek.Saturday);
        }

        [Fact(DisplayName = "Can write typed properties")]
        public void Can_write_typed_properties()
        {
            var ini = new Ini
            {
                new Section("TypedData")
                {
                    ["String"] = "This is a string",
                    ["SByte"] = (sbyte) -10,
                    ["Byte"] = (byte) 25,
                    ["Short"] = (short) -400,
                    ["UShort"] = (ushort) 700,
                    ["Int"] = -30000,
                    ["UInt"] = 50000U,
                    ["Long"] = -1000000L,
                    ["ULong"] = 20000000UL,
                    ["Float"] = 12.54443F,
                    ["Double"] = 6566123.22D,
                    ["Decimal"] = 1000.50m,
                    ["Boolean"] = true,
                    ["DateTime"] = DateTime.Now
                }
            };

            ini.ShouldNotBeNull();
        }

        [Theory(DisplayName = "Can read a multiline property value")]
        [EmbeddedResourceContent("IniFile.Tests.Data.MultilinePropertyValue.ini")]
        public void Can_read_multiline_property_value(string iniContent)
        {
            Ini ini = Ini.Load(iniContent);

            string expectedValue = "This is a " + Environment.NewLine +
                                   "multiline" + Environment.NewLine +
                                   " value.";
            ini["Section"]["Multiline"].ToString().ShouldBe(expectedValue);
        }

        [Fact]
        public void Can_load_from_file()
        {
            var ini = new Ini(@"D:\Temp\Data.ini");
            string ip = ini["tcp"]["ip2"];

            ip.ShouldNotBeNull();
        }
    }
}
