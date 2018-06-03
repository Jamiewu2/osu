using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Timing;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Osu.Objects.Drawables;

namespace osu.Game.Rulesets.Osu.Mods
{
    class OsuModBeat : ModBeat
    {
        public override string Description => "Shake to the Beat";

        public override void ApplyToClock(IAdjustableClock clock)
        {
            Logger.LogPrint(clock.ToString());
        }

        private static readonly Random getrandom = new Random();

        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }

        public override void ApplyToDrawableHitObjects(IEnumerable<DrawableHitObject> drawables)
        {
            foreach (var d in drawables.OfType<DrawableOsuHitObject>())
            {
                d.ApplyCustomUpdateState += ApplyBeatState;
            }
        }

        //gets called on init and on hit/miss
        private void ApplyBeatState(DrawableHitObject drawable, ArmedState state)
        {
            if (state != ArmedState.Idle)
                return;

            if (!(drawable is DrawableOsuHitObject d)) {
                return;
            }

            var frequencyFactor = 0.5; // 1/2th notes
            //TODO figure out how to hook a bpm timer into here
            var quarterNoteMsDelay = 333.3; //180bpm max love

            int direction = 1;

            var h = d.HitObject;
            var startTime = h.StartTime - h.TimePreempt;
            //TODO figure out why repeat sliders are brokeen. Probably don't like being scaled?

            //HitObject.startTime is the time when you hit the note, that is why this makes sense
            var endTime = ((h as IHasEndTime)?.EndTime ?? h.StartTime);

            foreach (int i in getStarts(startTime, endTime, quarterNoteMsDelay / frequencyFactor))
            {
                using (drawable.BeginAbsoluteSequence(i, true))
                {
                    var originalScale = drawable.Scale;
                    var scaleFactor = 1.35f;

                    var speed = 65 / frequencyFactor;
                    var XoffsetDistance = 6;
                    var YoffsetDistance = XoffsetDistance / 2;
                    var Yoffset = GetRandomNumber(-1 * YoffsetDistance, YoffsetDistance);

                    drawable.ScaleTo(scaleFactor * originalScale, speed, Easing.OutExpo)
                        .MoveToOffset(new OpenTK.Vector2(-1 * direction * XoffsetDistance , Yoffset), speed)
                        .Then()
                        .ScaleTo(originalScale,speed, Easing.OutCubic)
                        .MoveToOffset(new OpenTK.Vector2(direction * XoffsetDistance, -1 * Yoffset), speed);
                }

                direction *= -1;
            }
        }

        private List<double> getStarts(double startTime, double endTime, double bpmAsMsDelay)
        {
            List<double> retVal = new List<double>();

            //first Value
            // 17 % 5 = 2
            // first value = 17 - 2 = 15
            var first = startTime  - (startTime % bpmAsMsDelay);
            while (first < endTime)
            {
                retVal.Add(first);
                first += bpmAsMsDelay;
            }

            return retVal;
        }
    }
}
