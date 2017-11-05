﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace C3DE.Graphics.PostProcessing
{
    public abstract class PostProcessPass : IComparable, IDisposable
    {
        protected GraphicsDevice m_GraphicsDevice;
        protected int m_Order;

        public bool Enabled { get; set; } = true;

        public PostProcessPass(GraphicsDevice graphics)
        {
            m_GraphicsDevice = graphics;
        }

        public abstract void Initialize(ContentManager content);
        public abstract void Draw(SpriteBatch spriteBatch, RenderTarget2D renderTarget);

        public virtual void Apply(SpriteBatch spriteBatch, RenderTarget2D source, RenderTarget2D destination)
        {
        }

        protected RenderTarget2D GetRenderTarget(RenderTargetUsage targetUsage = RenderTargetUsage.DiscardContents)
        {
            var pp = Application.GraphicsDevice.PresentationParameters;
            var width = pp.BackBufferWidth;
            var height = pp.BackBufferHeight;
            var format = pp.BackBufferFormat;

            return new RenderTarget2D(Application.GraphicsDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, targetUsage);
        }

        protected void DrawFullscreenQuad(SpriteBatch spriteBatch, Texture2D texture, RenderTarget2D renderTarget, Effect effect)
        {
            m_GraphicsDevice.SetRenderTarget(renderTarget);
            DrawFullscreenQuad(spriteBatch, texture, renderTarget.Width, renderTarget.Height, effect);
        }

        protected void DrawFullscreenQuad(SpriteBatch spriteBatch, Texture2D texture, int width, int height, Effect effect)
        {
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();
        }

        public int CompareTo(object obj)
        {
            var pass = obj as PostProcessPass;

            if (pass == null)
                return 1;

            if (m_Order == pass.m_Order)
                return 0;
            else if (m_Order > pass.m_Order)
                return 1;
            else
                return -1;
        }

        #region IDisposable

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
        }

        protected void DisposeObject(IDisposable obj)
        {
            if (obj != null)
            {
                obj.Dispose();
                obj = null;
            }
        }

        #endregion
    }
}