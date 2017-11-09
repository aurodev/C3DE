﻿using C3DE.Components;
using C3DE.Components.Lighting;
using C3DE.Components.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;

namespace C3DE.Graphics.Materials
{
    public enum ShaderQuality
    {
        Low, Normal
    }

    [DataContract]
    public abstract class Material : IDisposable
    {
        protected internal Scene scene;
        protected Vector3 diffuseColor;
        protected internal Effect effect;
        protected internal bool hasAlpha;

        [DataMember]
        public string Id { get; private set; }

        [DataMember]
        public string Name { get; set; }

        internal int Index
        {
            get { return scene.Materials.IndexOf(this); }
        }

        [DataMember]
        public Color DiffuseColor
        {
            get { return new Color(diffuseColor); }
            set { diffuseColor = value.ToVector3(); }
        }

        public Texture2D MainTexture { get;set; }

        [DataMember]
        public Vector2 Tiling { get; set; }

        [DataMember]
        public Vector2 Offset { get; set; }

        [DataMember]
        public ShaderQuality ShaderQuality { get; set; }

		public Material()
		{
			diffuseColor = Color.White.ToVector3();
			Id = "MAT-" + Guid.NewGuid();
			Name = "Material_" + Id;
			Tiling = Vector2.One;
			Offset = Vector2.Zero;
			ShaderQuality = ShaderQuality.Normal;
			hasAlpha = false;

#if ANDROID || OPENGL || DESKTOP
            ShaderQuality = ShaderQuality.Low;
#endif
        }

        public Material(Scene mainScene)
            : this()
        {
            scene = mainScene;

            if (scene == null)
                scene = Scene.current;

            scene.Add(this);
        }

        public abstract void LoadContent(ContentManager content);

        public abstract void PrePass(Camera camera);

        public abstract void Pass(Renderer renderable);

        public virtual void PassLighting(Renderer renderer, Light light) { }

        public virtual void Dispose() { }
    }
}
