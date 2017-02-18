﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Input;
using OpenTK;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Transformations;
using osu.Game.Configuration;

namespace osu.Game.Graphics.Containers
{
    class ParallaxContainer : Container
    {
        public float ParallaxAmount = 0.02f;

        private bool parallaxEnabled = true;
        public bool ParallaxEnabled
        {
            get
            {
                return parallaxEnabled;
            }
            set
            {
                parallaxEnabled = value;
                if (!parallaxEnabled)
                {
                    content.MoveTo(Vector2.Zero, 1000, EasingTypes.OutQuint);
                    content.Scale = Vector2.One;
                }
            }
        }

        public override bool Contains(Vector2 screenSpacePos) => true;

        public ParallaxContainer()
        {
            RelativeSizeAxes = Axes.Both;
            AddInternal(content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            });
        }

        private Container content;
        private InputManager input;

        protected override Container<Drawable> Content => content;

        [BackgroundDependencyLoader]
        private void load(UserInputManager input, OsuConfigManager config)
        {
            this.input = input;

            config.GetBindable<bool>(OsuConfig.MenuParallax).ValueChanged += delegate
            {
                ParallaxEnabled = config.GetBindable<bool>(OsuConfig.MenuParallax);
            };
        }

        bool firstUpdate = true;

        protected override void Update()
        {
            base.Update();

            if (ParallaxEnabled) { 
                Vector2 offset = input.CurrentState.Mouse == null ? Vector2.Zero : ToLocalSpace(input.CurrentState.Mouse.NativeState.Position) - DrawSize / 2;
                content.MoveTo(offset * ParallaxAmount, firstUpdate ? 0 : 1000, EasingTypes.OutQuint);
                content.Scale = new Vector2(1 + ParallaxAmount);
            }

            firstUpdate = false;
        }
    }
}
