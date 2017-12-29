﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C3DE.Components;
using C3DE.Components.Lighting;
using C3DE.Graphics.PostProcessing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace C3DE.Graphics.Rendering
{
    public class DeferredRenderer : Renderer
    {
        private QuadRenderer m_QuadRenderer;
        private RenderTarget2D m_ColorTarget;
        private RenderTarget2D m_DepthTarget;
        private RenderTarget2D m_NormalTarget;
        private RenderTarget2D m_LightTarget;
        private Effect m_ClearEffect;
        private Effect m_CombineEffect;

        public DeferredRenderer(GraphicsDevice graphics)
            : base(graphics)
        {
            m_QuadRenderer = new QuadRenderer(graphics);
        }

        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);
            m_ClearEffect = content.Load<Effect>("Shaders/Deferred/Clear");
            m_ClearEffect = content.Load<Effect>("Shaders/Deferred/Combine");
        }

        /// <summary>
        /// Rebuilds render targets if Dirty is true.
        /// </summary>
        protected virtual void RebuildRenderTargets()
        {
            if (!Dirty)
                return;

            var width = m_graphicsDevice.PresentationParameters.BackBufferWidth;
            var height = m_graphicsDevice.PresentationParameters.BackBufferHeight;

            m_SceneRenderTarget?.Dispose();
            m_SceneRenderTarget = new RenderTarget2D(m_graphicsDevice, width, height, false, m_HDRSupport ? SurfaceFormat.HdrBlendable : SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);

            m_ColorTarget = new RenderTarget2D(m_graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            m_NormalTarget = new RenderTarget2D(m_graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            m_DepthTarget = new RenderTarget2D(m_graphicsDevice, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            m_LightTarget = new RenderTarget2D(m_graphicsDevice, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            Dirty = false;
        }

        public override void Dispose(bool disposing)
        {
        }

        public override void Render(Scene scene)
        {
            m_graphicsDevice.SetRenderTargets(m_ColorTarget, m_NormalTarget, m_DepthTarget);
            foreach (var pass in m_ClearEffect.Techniques[0].Passes)
            {
                pass.Apply();
                m_QuadRenderer.RenderFullscreenQuad(m_graphicsDevice);
            }

            foreach (var renderer in scene.renderList)
                renderer.Draw(m_graphicsDevice);

            m_graphicsDevice.SetRenderTargets(null);

            m_graphicsDevice.SetRenderTarget(m_LightTarget);
            m_graphicsDevice.Clear(Color.Transparent);

            DrawLights(Camera.Main, scene.lights);

            m_graphicsDevice.SetRenderTargets(null);

            m_graphicsDevice.BlendState = BlendState.Opaque;
            m_graphicsDevice.DepthStencilState = DepthStencilState.None;
            m_graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            m_graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            foreach (var pass in m_CombineEffect.Techniques[0].Passes)
            {
                m_CombineEffect.Parameters["ColorMap"].SetValue(m_ColorTarget);
                m_CombineEffect.Parameters["LightMap"].SetValue(m_LightTarget);
                pass.Apply();
                m_QuadRenderer.RenderFullscreenQuad(m_graphicsDevice);
            }
        }

        private void DrawLights(Camera camera, IEnumerable<Light> lights)
        {

        }

        public override void RenderEditor(Scene scene, Camera camera, RenderTarget2D target)
        {
        }
    }
}
