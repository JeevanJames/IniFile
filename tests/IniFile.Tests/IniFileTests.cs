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
    }
}