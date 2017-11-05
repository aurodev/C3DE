﻿using C3DE.Components;
using C3DE.Components.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace C3DE.Graphics.Materials
{
    [DataContract]
    public class TransparentMaterial : Material
    {
        public TransparentMaterial(Scene scene, string name = "Transparent Material")
            : base(scene)
        {
            diffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            hasAlpha = true;
            Name = name;
        }

        public override void LoadContent(ContentManager content)
        {
            effect = content.Load<Effect>("Shaders/TransparentEffect");
        }

        public override void PrePass(Camera camera)
        {
            effect.Parameters["View"].SetValue(camera.view);
            effect.Parameters["Projection"].SetValue(camera.projection);
        }

        public override void Pass(Renderer renderable)
        {
            // Material
            effect.Parameters["TextureTiling"].SetValue(Tiling);
            effect.Parameters["TextureOffset"].SetValue(Offset);
            effect.Parameters["AmbientColor"].SetValue(scene.RenderSettings.ambientColor);
            effect.Parameters["DiffuseColor"].SetValue(diffuseColor);
            effect.Parameters["MainTexture"].SetValue(diffuseTexture);
            effect.Parameters["World"].SetValue(renderable.GameObject.Transform.world);
            effect.CurrentTechnique.Passes[0].Apply();
        }
    }
}
