using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Objects.Drawables;

namespace osu.Game.Rulesets.Osu.Mods
{
    class OsuModShifty : ModShifty, IApplicableToDrawableHitObjects
    {
        public override string Description => @"Circles like to move sometimes";
        public override double ScoreMultiplier => 1.06;

        private const double fade_in_duration_multiplier = 0.4;
        private const double fade_out_duration_multiplier = 0.7;

        private static readonly Random getrandom = new Random();

        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }

        public void ApplyToDrawableHitObjects(IEnumerable<DrawableHitObject> drawables)
        {
            foreach (var d in drawables.OfType<DrawableOsuHitObject>())
            {
                d.ApplyCustomUpdateState += ApplyShiftyState;

                d.HitObject.TimeFadein = d.HitObject.TimePreempt * fade_in_duration_multiplier;
                foreach (var h in d.HitObject.NestedHitObjects.OfType<OsuHitObject>())
                    h.TimeFadein = h.TimePreempt * fade_in_duration_multiplier;
            }
        }

        protected void ApplyShiftyState(DrawableHitObject drawable, ArmedState state)
        {
            //TODO What other HitObject could it even be?
            if (!(drawable is DrawableOsuHitObject d))
                return;

            var h = d.HitObject;

            var fadeInStartTime = h.StartTime - h.TimePreempt;
            var fadeOutStartTime = fadeInStartTime + h.TimeFadein;
            var fadeOutDuration = h.TimePreempt * fade_out_duration_multiplier;

            // new duration from completed fade in to end (before fading out)
            var longFadeDuration = ((h as IHasEndTime)?.EndTime ?? h.StartTime) - fadeOutStartTime;

            switch (drawable)
            {
                case DrawableHitCircle circle:
                case DrawableSlider slider:
                case DrawableSliderTick sliderTick:

                    using (drawable.BeginAbsoluteSequence(fadeInStartTime))
                    {
                        var offsetMax = 400;
                        var randomXOffset = GetRandomNumber(-1 * offsetMax, offsetMax);
                        var randomYOffset = GetRandomNumber(-1 * offsetMax, offsetMax);

                        drawable.MoveToOffset(new OpenTK.Vector2(-randomXOffset, -randomYOffset), 1)
                              .Then()
                              .MoveToOffset(new OpenTK.Vector2(randomXOffset, randomYOffset), fadeOutDuration, Easing.OutCubic);
                    }

                    break;
                
                case DrawableSpinner spinner:
                    // hide elements we don't care about.
                    spinner.Disc.Hide();
                    spinner.Ticks.Hide();
                    spinner.Background.Hide();

                    using (spinner.BeginAbsoluteSequence(fadeOutStartTime + longFadeDuration, true))
                        spinner.FadeOut(fadeOutDuration);

                    break;
            }
        }
    }
}
