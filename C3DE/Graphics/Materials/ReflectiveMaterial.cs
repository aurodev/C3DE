﻿using C3DE.Components;
using C3DE.Components.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace C3DE.Graphics.Materials
{
    [DataContract]
    public class ReflectiveMaterial : Material
    {
        private Vector3 _reflectionColor;

        public TextureCube ReflectionMap { get; set; }

        [DataMember]
        public Color ReflectionColor
        {
            get { return new Color(_reflectionColor); }
            set { _reflectionColor = value.ToVector3(); }
        }

        public ReflectiveMaterial(Scene scene, string name = "Reflective Material")
            : base(scene)
        {
            Name = name;
        }

        public override void LoadContent(ContentManager content)
        {
            effect = content.Load<Effect>("Shaders/ReflectionEffect");
        }

        public override void PrePass(Camera camera)
        {
            effect.Parameters["View"].SetValue(camera.view);
            effect.Parameters["Projection"].SetValue(camera.projection);

            // Material
            effect.Parameters["EyePosition"].SetValue(camera.GameObject.Transform.Position);
        }

        public override void Pass(Renderer renderable)
        {
            effect.Parameters["MainTextureEnabled"].SetValue(diffuseTexture != null);

            if (diffuseTexture != null)
                effect.Parameters["MainTexture"].SetValue(diffuseTexture);

            effect.Parameters["ReflectionColor"].SetValue(diffuseColor);
            effect.Parameters["ReflectiveTexture"].SetValue(ReflectionMap);
            effect.Parameters["TextureTiling"].SetValue(Tiling);
            effect.Parameters["TextureOffset"].SetValue(Offset);
            effect.Parameters["World"].SetValue(renderable.GameObject.Transform.world);
            effect.CurrentTechnique.Passes[0].Apply();
        }
    }
}
