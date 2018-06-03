using System;
using System.Collections.Generic;
using System.Text;

namespace osu.Game.Rulesets.Mods
{
    public abstract class ModShifty : Mod
    {
        public override string Name => "Shifty";

        public override string ShortenedName => "SH";
        public override ModType Type => ModType.DifficultyIncrease;

        public override double ScoreMultiplier => 1.06;
    }
}
