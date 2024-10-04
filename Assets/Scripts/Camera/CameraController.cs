using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using Player.ActionHandlers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float dampingFactor = 0.98f;
        
        private ClickHandler _clickHandler;
        private Vector3 deltaPosition = Vector3.zero;
        private Vector3 startPosition = Vector3.zero;
        private Vector3 finishPosition = Vector3.zero;
        private Vector3 currentMoveSpeed = Vector3.zero;
        
        private void Start()
        {
            _clickHandler = ClickHandler.Instance;
            _clickHandler.SetDragEventHandlers(OnDragStart, OnDragEnd, OnDragContinue);
        }
        void Update()
        {
            transform.position =new Vector3(transform.position.x + currentMoveSpeed.x,
                transform.position.y +currentMoveSpeed.y, transform.position.z);
            currentMoveSpeed *= dampingFactor;
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

    }
}
