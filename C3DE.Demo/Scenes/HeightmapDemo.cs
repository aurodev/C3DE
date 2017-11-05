﻿using C3DE.Components.Lighting;
using C3DE.Components.Rendering;
using C3DE.Demo.Scripts;
using C3DE.Graphics.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace C3DE.Demo.Scenes
{
    public class HeightmapDemo : Scene
    {
        public HeightmapDemo() : base("Heightmap Terrain") { }

        public override void Initialize()
        {
            base.Initialize();

            // Add a camera with a FPS controller
            var cameraGo = GameObjectFactory.CreateCamera(new Vector3(0, 2, -10), new Vector3(0, 0, 0), Vector3.Up);
            cameraGo.AddComponent<ControllerSwitcher>();
            Add(cameraGo);

            // And a light
            var lightGo = GameObjectFactory.CreateLight(LightType.Directional, Color.LightSkyBlue, 1.0f);
            lightGo.Transform.Rotation = new Vector3(-1, 1, 0);
            lightGo.AddComponent<DemoBehaviour>();
            Add(lightGo);

            // Finally a terrain
            var terrainMaterial = new TerrainMaterial(scene);
            terrainMaterial.Texture = Application.Content.Load<Texture2D>("Textures/Terrain/Grass");
            terrainMaterial.SandTexture = Application.Content.Load<Texture2D>("Textures/Terrain/Sand");
            terrainMaterial.SnowTexture = Application.Content.Load<Texture2D>("Textures/Terrain/Snow");
            terrainMaterial.RockTexture = Application.Content.Load<Texture2D>("Textures/Terrain/Rock");

            var terrainGo = GameObjectFactory.CreateTerrain();
            
            scene.Add(terrainGo);

            var terrain = terrainGo.GetComponent<Terrain>();
            terrain.LoadHeightmap("Textures/heightmap");
            terrain.Renderer.Material = terrainMaterial;

            var weightMap = terrain.GenerateWeightMap();           
            terrainMaterial.WeightTexture = weightMap;
            terrainMaterial.Tiling = new Vector2(4);
            terrainGo.AddComponent<WeightMapViewer>();

            terrainGo.Transform.Translate(-terrain.Width >> 1, -10, -terrain.Depth >> 1);

            // With water !
            var waterTexture = Application.Content.Load<Texture2D>("Textures/water");
            var bumpTexture = Application.Content.Load<Texture2D>("Textures/wavesbump");
            var water = GameObjectFactory.CreateWater(waterTexture, bumpTexture, new Vector3(terrain.Width * 0.5f));
            scene.Add(water);

            Screen.ShowCursor = true;

            // Don't miss the Skybox ;)
            RenderSettings.Skybox.Generate(Application.GraphicsDevice, Application.Content, DemoGame.BlueSkybox);

            // And fog
            RenderSettings.FogDensity = 0.0085f;
            RenderSettings.FogMode = FogMode.Exp2;
        }
    }
}
