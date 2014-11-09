﻿using C3DE.Components.Colliders;
using C3DE.Components.Renderers;
using C3DE.Geometries;
using C3DE.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace C3DE.Prefabs
{
    public class LavaPrefab : SceneObject
    {
        protected MeshRenderer renderer;
        protected LavaMaterial material;
        protected BoxCollider collider;

        public MeshRenderer Renderer
        {
            get { return renderer; }
        }

        public LavaMaterial Material
        {
            get { return material; }
        }

        public BoxCollider Collider
        {
            get { return collider; }
        }

        public LavaPrefab(string name)
            : base(name)
        {
            renderer = AddComponent<MeshRenderer>();
            renderer.CastShadow = false;
            renderer.ReceiveShadow = true;
            renderer.Geometry = new PlaneGeometry();
            collider = AddComponent<BoxCollider>();
            collider.IsPickable = false;
        }

        public void Generate(string lavaTexture, string bumpTexture, Vector3 size)
        {
            if (scene == null)
                throw new Exception("You need to attach this prefab to the scene.");

            material = new LavaMaterial(scene);

            if (!string.IsNullOrEmpty(lavaTexture))
                material.MainTexture = Application.Content.Load<Texture2D>(lavaTexture);

            if (!string.IsNullOrEmpty(bumpTexture))
                material.NormalMap = Application.Content.Load<Texture2D>(bumpTexture);

            renderer.Material = material;
            renderer.Geometry.Size = size;
            renderer.Geometry.Generate();
            collider.Box = new BoundingBox(transform.Position, size);
        }
    }
}
