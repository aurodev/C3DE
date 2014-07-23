﻿using Microsoft.Xna.Framework;

namespace C3DE.Components
{
    /// <summary>
    /// A component to add a box collider to an object.
    /// </summary>
    public class BoxCollider : Component
    {
        private BoundingBox _box;
        private Vector3 _center;
        private bool _dirty;

        public BoundingBox Box
        {
            get {return _box;}
        }

        public Vector3 Min
        {
            get { return _box.Min; }
        }

        public Vector3 Max
        {
            get { return _box.Max; }
        }

        public Vector3 Center
        {
            get { return _center; }
        }

        public BoxCollider()
            : base()
        {
            _box = new BoundingBox();
            _center = Vector3.Zero;
            _dirty = true;
        }

        /// <summary>
        /// Compute the bounding box.
        /// </summary>
        public override void Update()
        {
            if (_dirty)
            {
                _dirty = false;

                var modelRenderer = GetComponent<ModelRenderer>();

                if (modelRenderer != null)
                {
                    var sphere = modelRenderer.GetBoundingSphere();
                    _box = BoundingBox.CreateFromSphere(sphere);
                    _center = sphere.Center;
                }
            }
        }
    }
}