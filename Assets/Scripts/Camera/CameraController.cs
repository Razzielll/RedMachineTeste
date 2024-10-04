using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Connection;
using Player;
using Player.ActionHandlers;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Scenes;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        [Tooltip("Скорость движения камеры")]
        [SerializeField] private float moveSpeed = 10f;
        [Tooltip("Коэффициент затухания скорости движения камеры")]
        [Range(0f,1f)]
        [SerializeField] private float dampingFactor = 0.98f;
        [Tooltip("Ограничивает движение элементов относительно границы экрана")]
        [SerializeField] float borderOffset = 0.1f;
        
        private ClickHandler _clickHandler;
        private Vector3 deltaPosition = Vector3.zero;
        private Vector3 startPosition = Vector3.zero;
        private Vector3 finishPosition = Vector3.zero;
        private Vector3 currentMoveSpeed = Vector3.zero;
        
        private Vector3 minBounds = Vector3.zero;
        private Vector3 maxBounds = Vector3.zero;
        private UnityEngine.Camera _camera;

        private void OnEnable()
        {
            ScenesChanger.SceneLoadedEvent += CalculateBounds;
            ScenesChanger.SceneLoadedEvent += ResetCameraPosition;
        }


        private void OnDisable()
        {
            ScenesChanger.SceneLoadedEvent -= CalculateBounds;
            ScenesChanger.SceneLoadedEvent -= ResetCameraPosition;
        }

        private void Start()
        {
            _clickHandler = ClickHandler.Instance;
            _clickHandler.SetDragEventHandlers(OnDragStart, OnDragEnd, OnDragContinue);
            _camera= CameraHolder.Instance.MainCamera;
            CalculateBounds();
        }
        void Update()
        {
            Vector3 moveDelta = new Vector3(-currentMoveSpeed.x, -currentMoveSpeed.y, 0);
            if (!KeepInSight(moveDelta))
            {
                currentMoveSpeed = Vector3.zero;
                return;
            }

            transform.position =new Vector3(transform.position.x + currentMoveSpeed.x,
                transform.position.y +currentMoveSpeed.y, transform.position.z);
            currentMoveSpeed *= dampingFactor;
        }
        
        private void ResetCameraPosition()
        {
            _camera.transform.position = new Vector3(0, 0, _camera.transform.position.z);
        }
        private void OnDragContinue(Vector3 position)
        {
            if (PlayerController.PlayerState == PlayerState.Connecting)
                return;
            
            deltaPosition = position - startPosition;
            if (deltaPosition != Vector3.zero)
            {
                float moveValue = moveSpeed  * Time.deltaTime;
                float moveX = Mathf.Clamp(-deltaPosition.x * moveValue, -moveValue, moveValue); 
                float moveY = Mathf.Clamp(-deltaPosition.y * moveValue, -moveValue, moveValue);

                currentMoveSpeed = new Vector3(moveX, moveY, 0);
            }
            else
            {
                currentMoveSpeed *= dampingFactor;
            }
        }


        private void OnDragStart(Vector3 startPosition)
        {
            if (PlayerController.PlayerState == PlayerState.Connecting)
                return;
            this.startPosition = startPosition;
            currentMoveSpeed = Vector3.zero; 
        }
        private void OnDragEnd(Vector3 finishPosition)
        {
        }
        private void CalculateBounds()
        {
            ColorNode[] nodes = GameObject.FindObjectsOfType<ColorNode>();
            if (nodes == null || nodes.Length ==0) return;
            //colorNodesContainer = nodes[0]. transform;
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

            minBounds = renderers[0].bounds.min;
            maxBounds = renderers[0].bounds.max;

            foreach (var boundRenderer in renderers)
            {
                minBounds = Vector3.Min(minBounds, boundRenderer.bounds.min);
                maxBounds = Vector3.Max(maxBounds, boundRenderer.bounds.max);
            }
        }


        private bool KeepInSight(Vector3 moveDelta)
        {
          //  if (colorNodesContainer == null) return false;
            Vector3 viewPosMin = _camera.WorldToViewportPoint(minBounds+moveDelta);
            Vector3 viewPosMax = _camera.WorldToViewportPoint(maxBounds+moveDelta);

            if (viewPosMax.x < borderOffset || viewPosMax.y < borderOffset || viewPosMin.x > 1-borderOffset || viewPosMin.y > 1-borderOffset)
            {
                return false;
               // _camera.transform.position = new Vector3(colorNodesContainer.position.x, colorNodesContainer.position.y, _camera.transform.position.z);
            }

            return true;
        }
    }
}
