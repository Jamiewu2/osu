using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Osu.Objects.Drawables;

namespace osu.Game.Rulesets.Osu.Tests
{
    [TestFixture]
    public class TestCaseHitCircleBeat : TestCaseHitCircle
    {
        public override IReadOnlyList<Type> RequiredTypes => base.RequiredTypes.Concat(new[] { typeof(OsuModBeat), typeof(TestCaseHitCircle), typeof(DrawableHitCircle) }).ToList();

        public TestCaseHitCircleBeat()
        {
            Mods.Add(new OsuModBeat());
        }
    }
}
