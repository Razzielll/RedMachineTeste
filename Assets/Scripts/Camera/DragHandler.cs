using Player;
using UnityEngine;
using Player.ActionHandlers;

namespace Camera
{
    public class DragHandler : MonoBehaviour
    {
        private ClickHandler _clickHandler;
        private Vector3 startPosition = Vector3.zero;
        private CameraMovement _cameraMovement;

        private void Start()
        {
            _clickHandler = ClickHandler.Instance;
            _clickHandler.SetDragEventHandlers(OnDragStart, OnDragEnd, OnDragContinue);
            _cameraMovement = GetComponent<CameraMovement>();
        }

        private void OnDragStart(Vector3 startPosition)
        {
            if (PlayerController.PlayerState == PlayerState.Connecting)
                return;
            this.startPosition = startPosition;
            _cameraMovement.SetMoveSpeed(Vector3.zero);
        }

        private void OnDragContinue(Vector3 position)
        {
            if (PlayerController.PlayerState == PlayerState.Connecting)
                return;

            Vector3 deltaPosition = position - startPosition;
            if (deltaPosition != Vector3.zero)
            {
                float moveValue = _cameraMovement.GetMoveSpeed() * Time.deltaTime;
                float moveX = Mathf.Clamp(-deltaPosition.x * moveValue, -moveValue, moveValue);
                float moveY = Mathf.Clamp(-deltaPosition.y * moveValue, -moveValue, moveValue);

                _cameraMovement.SetMoveSpeed(new Vector3(moveX, moveY, 0));
            }
            else
            {
                _cameraMovement.SetMoveSpeed(_cameraMovement.GetCurrentMoveSpeed() * _cameraMovement.GetDampingFactor());
            }
        }

        private void OnDragEnd(Vector3 finishPosition)
        {
            // Handle drag end if needed
        }
    }
}