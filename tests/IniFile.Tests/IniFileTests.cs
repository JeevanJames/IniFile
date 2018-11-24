using IniFile;

using Shouldly;

using Xunit;

namespace CodeBits.Tests
{
    public sealed class IniTests
    {

        private const string ValidIni = @"[Game State]
; These are the players
Player1=Jeevan
Player2=Merina

 ; One section per player

[Jeevan]
  Level = 7
  Karma = 5.23
  Weapons = BFG9000, Star

[Merina]
Level=3
Karma=2.95
Weapons=Star, Fists";

        [Fact]
        public void Basic_tests()
        {
            var ini = Ini.Load(ValidIni, null);

            ini.Count.ShouldBe(3);
            ini[0].Name.ShouldBe("Game State");
            ini[1].Name.ShouldBe("Jeevan");
            ini[2].Name.ShouldBe("Merina");

            Ini.Section section = ini[0];
            section.Count.ShouldBe(2);
            section["Player1"].ShouldBe("Jeevan");
            section["Player2"].ShouldBe("Merina");
        }

        [Fact]
        public void Save_tests()
        {
            var ini = new Ini();

            var section = new Ini.Section("Test")
            {
                new Ini.Property("Player1", "Jeevan"),
                new Ini.Property("Player2", "Merina"),
            };
            ini.Add(section);

            ini.Add(new Ini.Section("Jeevan")
            {
                new Ini.Property("Powers", "Superspeed,Super strength"),
                new Ini.Property("Costume", "Scarlet")
            });

            ini.Add(new Ini.Section("Merina")
            {
                new Ini.Property("Powers", "Stretchability, Invisibility"),
                new Ini.Property("Costume", "Blue")
            });

        }
    }
}