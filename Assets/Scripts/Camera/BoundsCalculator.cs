using System.Collections.Generic;
using Connection;
using UnityEngine;
using Utils.Scenes;

namespace Camera
{
    public class BoundsCalculator : MonoBehaviour
    {
        private Vector3 _minBounds = Vector3.zero;
        private Vector3 _maxBounds = Vector3.zero;

        private void OnEnable()
        {
            ScenesChanger.SceneLoadedEvent += CalculateBounds;
        }

        private void OnDisable()
        {
            ScenesChanger.SceneLoadedEvent -= CalculateBounds;
        }

        private void CalculateBounds()
        {
            ColorNode[] nodes = GameObject.FindObjectsOfType<ColorNode>();
            if (nodes == null || nodes.Length == 0) return;

            List<SpriteRenderer> renderers = new List<SpriteRenderer>();
            foreach (var node in nodes)
            {
                SpriteRenderer spriteRenderer = node.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    renderers.Add(spriteRenderer);
                }
            }

            if (renderers.Count == 0) return;

            _minBounds = renderers[0].bounds.min;
            _maxBounds = renderers[0].bounds.max;

            foreach (var boundRenderer in renderers)
            {
                _minBounds = Vector3.Min(_minBounds, boundRenderer.bounds.min);
                _maxBounds = Vector3.Max(_maxBounds, boundRenderer.bounds.max);
            }
        }

        public Vector3 GetMinBounds()
        {
            return _minBounds;
        }

        public Vector3 GetMaxBounds()
        {
            return _maxBounds;
        }
    }
}