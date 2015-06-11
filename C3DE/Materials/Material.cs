﻿using C3DE.Components.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace C3DE.Materials
{
    public enum ShaderQuality
    {
        Low, Normal
    }

    public abstract class Material : IDisposable
    {
        private static int MaterialCounter = 0;

        protected internal Scene scene;
        protected Vector3 diffuseColor;
        protected Texture2D mainTexture;
        protected internal Effect effect;

        public int Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        internal int Index
        {
            get { return scene.Materials.IndexOf(this); }
        }

        public Color DiffuseColor
        {
            get { return new Color(diffuseColor); }
            set { diffuseColor = value.ToVector3(); }
        }

        public Texture2D MainTexture
        {
            get { return mainTexture; }
            set { mainTexture = value; }
        }

        public Vector2 Tiling { get; set; }

        public Vector2 Offset { get; set; }

        public ShaderQuality ShaderQuality { get; set; }

        public Material(Scene mainScene)
        {
            diffuseColor = Color.White.ToVector3();
            Id = MaterialCounter++;
            Name = "Material_" + Id;
            Tiling = Vector2.One;
            Offset = Vector2.Zero;
            ShaderQuality = ShaderQuality.Normal;

#if ANDROID || OPENGL
            ShaderQuality = ShaderQuality.Low;
#endif

            scene = mainScene;
            scene.Add(this);
        }

        public abstract void LoadContent(ContentManager content);

        public abstract void PrePass();

        public abstract void Pass(RenderableComponent renderable);

        public virtual void Dispose()
        {
        }
    }
}
