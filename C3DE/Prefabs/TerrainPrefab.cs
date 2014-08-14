﻿using C3DE.Components;
using C3DE.Components.Renderers;
using C3DE.Geometries;
using C3DE.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace C3DE.Prefabs
{
    public class TerrainPrefab : Prefab
    {
        protected MeshRenderer renderer;
        protected TerrainGeometry geometry;
        // protected TerrainCollider collider;

        public MeshRenderer Renderer
        {
            get { return renderer; }
        }

        public Vector2 TextureRepeat
        {
            get { return geometry.TextureRepeat; }
            set { geometry.TextureRepeat = value; }
        }

        public int Width
        {
            get { return geometry.Width; }
        }

        public int Height
        {
            get { return geometry.Height; }
        }

        public int Depth
        {
            get { return geometry.Depth; }
        }

        public TerrainPrefab(string name, Scene scene)
            : base(name, scene)
        {
            geometry = new TerrainGeometry(100, 100, 1);

            renderer = AddComponent<MeshRenderer>();
            renderer.Geometry = geometry;
            renderer.CastShadow = false;
        }

        /// <summary>
        /// Genearate the terrain with a heightmap texture.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="heightmap"></param>
        public void LoadHeightmap(string heightmapName)
        {
            var heightmap = Application.Content.Load<Texture2D>(heightmapName);

            geometry.Width = heightmap.Width;
            geometry.Depth = heightmap.Height;

            Color[] colors = new Color[geometry.Width * geometry.Depth];
            heightmap.GetData(colors);

            geometry.Data = new float[geometry.Width, geometry.Depth];

            for (int x = 0; x < geometry.Width; x++)
                for (int y = 0; y < geometry.Depth; y++)
                    geometry.Data[x, y] = colors[x + y * geometry.Width].R / 10.0f; // Max height 25.5f

            Build();
        }

        /// <summary>
        /// Randomize the heightdata with the perlin noise algorithm.
        /// </summary>
        /// <param name="octaves"></param>
        /// <param name="amplitude"></param>
        /// <param name="frequency"></param>
        /// <param name="persistence"></param>
        public void Randomize(int octaves = 2, int amplitude = 22, double frequency = 0.085, double persistence = 0.3)
        {
            geometry.Data = new float[geometry.Width, geometry.Depth];

            NoiseGenerator.GenerateNoise(octaves, amplitude, frequency, persistence);

            for (int x = 0; x < geometry.Width; x++)
            {
                for (int z = 0; z < geometry.Depth; z++)
                    geometry.Data[x, z] = (float)NoiseGenerator.Noise(x, z);
            }

            Build();
        }

        public void Flat()
        {
            Build();
        }

        private void Build()
        {
            renderer.Geometry.Generate();
        }

        public void ApplyCollision(Transform tr)
        {
            var y = (GetTerrainHeight(tr.Position.X, 0, tr.Position.Z) + 15 - tr.Position.Y) * 0.2f;
            tr.Translate(0.0f, y, 0.0f);
        }

        public void ApplyCollision(ref Vector3 position)
        {
            var y = (GetTerrainHeight(position) + 2 - position.Y) * 0.2f;
            position.Y += y;
        }

        public virtual float GetTerrainHeight(Vector3 position)
        {
            return GetTerrainHeight(position.X, position.Y, position.Z);
        }

        public virtual float GetTerrainHeight(float x, float y, float z)
        {
            // Terrain space.
            x -= transform.Position.X;
            y -= transform.Position.Y;
            z -= transform.Position.Z;

            float terrainHeigth = 0.0f;

            float sizedPosX = (x / geometry.Size.X) / transform.LocalScale.X;
            float sizedPosY = (y / geometry.Size.Y) / transform.LocalScale.Y;
            float sizedPosZ = (z / geometry.Size.Z) / transform.LocalScale.Z;

            int px = (int)((x / geometry.Size.X) / transform.LocalScale.X);
            int pz = (int)((z / geometry.Size.Z) / transform.LocalScale.Z);

            if (px < 0 || px >= geometry.Data.GetLength(0) - 1 || pz < 0 || pz >= geometry.Data.GetLength(1) - 1)
                terrainHeigth = y;
            else
            {
                float triangleY0 = geometry.Data[px, pz];
                float triangleY1 = geometry.Data[px + 1, pz];
                float triangleY2 = geometry.Data[px, pz + 1];
                float triangleY3 = geometry.Data[px + 1, pz + 1];

                // Determine where are the point
                float segX = sizedPosX - px;
                float segZ = sizedPosZ - pz;

                // We are on the first triangle
                if ((segX + segZ) < 1)
                {
                    terrainHeigth = triangleY0;
                    terrainHeigth += (triangleY1 - triangleY0) * segX;
                    terrainHeigth += (triangleY2 - triangleY0) * segZ;
                }
                else // Second triangle
                {
                    terrainHeigth = triangleY3;
                    terrainHeigth += (triangleY1 - triangleY3) * segX;
                    terrainHeigth += (triangleY2 - triangleY3) * segZ;
                }
            }

            return (terrainHeigth * geometry.Size.Y * transform.LocalScale.Y);
        }

        public static Texture2D t;

        public Texture2D GenerateWeightMap()
        {
            var width = geometry.Width;
            var depth = geometry.Depth;

            var wMap = new Texture2D(Application.GraphicsDevice, width, depth, false, SurfaceFormat.Color);
            var colors = new Color[width * depth];
            float data = 0;

            for (int x = 0; x < geometry.Width; x++)
            {
                for (int z = 0; z < geometry.Depth; z++)
                {
                    data = geometry.Data[x, z];

                    if (data < 3)
                        colors[x + z * width] = Color.Red;

                    else if (data >= 3 && data < 15)
                        colors[x + z * width] = Color.Black;

                    else if (data >= 15 && data < 20)
                        colors[x + z * width] = Color.Green;

                    else
                        colors[x + z * width] = Color.Blue;
                }
            }

            wMap.SetData(colors);

            t = wMap;

            return wMap;
        }
    }
}
