﻿using C3DE.Components;
using C3DE.Components.Physics;
using C3DE.Components.Rendering;
using C3DE.Graphics.Materials;
using C3DE.Graphics.Primitives;
using C3DE.Utils;
using C3DE.VR;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections;

namespace C3DE.Demo.Scripts
{
    public class PhysicsSpawner : Behaviour
    {
        private VRService m_VRService;
        private Transform m_RightHand;

        public override void Start()
        {
            VRManager.VRServiceChanged += OnVRChanged;
        }

        public override void Update()
        {
            base.Update();

            if (Input.Keys.JustPressed(Keys.Space) || Input.Keys.Pressed(Keys.LeftControl))
                SpawnCubeAtPosition(Camera.Main.Transform.Position + new Vector3(0, 0, 5));

            if (m_VRService != null && m_RightHand != null && m_VRService.GetButtonDown(1, XRButton.Trigger))
                SpawnCubeAtPosition(m_RightHand.Position);
        }

        private void SpawnCubeAtPosition(Vector3 position)
        {
            var go = new GameObject("Cube");
            Scene.current.Add(go);
            go.Transform.LocalPosition = position;

            var cube = go.AddComponent<MeshRenderer>();
            cube.Geometry = new CubeMesh();
            cube.Geometry.Size = new Vector3(VRManager.Enabled ? 0.25f : 1.0f);
            cube.Geometry.Build();
            cube.CastShadow = true;
            cube.ReceiveShadow = false;

             var material = new StandardMaterial();
            material.DiffuseColor = RandomHelper.GetColor();
            material.MainTexture = Application.Content.Load<Texture2D>("Textures/Terrain/Rock");
            material.NormalTexture = Application.Content.Load<Texture2D>("Textures/Terrain/Rock_Normal");
            cube.Material = material;

            var collider = cube.AddComponent<BoxCollider>();
            var rb = cube.AddComponent<Rigidbody>();
            rb.AddComponent<RigidbodyRenderer>();
        }

        private void OnVRChanged(VRService service)
        {
            m_VRService = service;

            StartCoroutine(SetupVRPlayer());
        }

        private IEnumerator SetupVRPlayer()
        {
            yield return Coroutine.WaitForSeconds(0.5f);

            var player = Camera.Main.Transform.Parent;
            if (player != null)
            {
                var controllers = player.GetComponentsInChildren<MotionController>();
                foreach (var controller in controllers)
                    if (!controller.LeftHand)
                        m_RightHand = controller.Transform;
            }
        }
    }
}
