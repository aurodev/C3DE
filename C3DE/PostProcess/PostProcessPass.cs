﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace C3DE.PostProcess
{
    public abstract class PostProcessPass : IComparable, IDisposable
    {
        public bool Enabled { get; set; }

        protected int order;

        public PostProcessPass()
        {
            Enabled = true;
        }

        public abstract void Initialize(ContentManager content);
        public abstract void Apply(SpriteBatch spriteBatch, RenderTarget2D renderTarget);

        public virtual void Apply(SpriteBatch spriteBatch, RenderTarget2D source, RenderTarget2D destination)
        {
        }

        public int CompareTo(object obj)
        {
            var pass = obj as PostProcessPass;

            if (pass == null)
                return 1;

            if (order == pass.order)
                return 0;
            else if (order > pass.order)
                return 1;
            else
                return -1;
        }

        public virtual void Dispose()
        {
        }
    }
}
