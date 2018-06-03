using System;
using System.Collections.Generic;
using System.Text;
using osu.Framework.Timing;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Mods
{
    public abstract class ModBeat : Mod, IApplicableToClock, IApplicableToDrawableHitObjects
    {
        public override string Name => "Beat";

        public override string ShortenedName => "BT";

        public override ModType Type => ModType.DifficultyIncrease;
        public override double ScoreMultiplier => 1.0;

        public abstract void ApplyToClock(IAdjustableClock clock);
        public abstract void ApplyToDrawableHitObjects(IEnumerable<DrawableHitObject> drawables);
    }
}
