using System;
using Camera;
using UnityEngine;
using Utils.Singleton;


namespace Player.ActionHandlers
{
    public class ClickHandler : DontDestroyMonoBehaviourSingleton<ClickHandler>
    {
        [SerializeField] private float clickToDragDuration;

        public event Action<Vector3> PointerDownEvent;
        public event Action<Vector3> ClickEvent;
        public event Action<Vector3> PointerUpEvent;
        public event Action<Vector3> DragStartEvent;
        public event Action<Vector3> AlternativeDragStartEvent;
        public event Action<Vector3> DragContinueEvent;
        public event Action<Vector3> DragEndEvent;
        public event Action<Vector3> AlternativeDragEndEvent;

        private Vector3 _pointerDownPosition;

        private bool _isClick;
        private bool _isDrag;
        private float _clickHoldDuration;


        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isClick = true;
                _clickHoldDuration = .0f;

                _pointerDownPosition = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                
                PointerDownEvent?.Invoke(_pointerDownPosition);
                
                _pointerDownPosition = new Vector3(_pointerDownPosition.x, _pointerDownPosition.y, .0f);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                var pointerUpPosition = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                    
                if (_isDrag)
                {
                    DragEndEvent?.Invoke(pointerUpPosition);
                    AlternativeDragEndEvent?.Invoke(pointerUpPosition);
                    _isDrag = false;
                }
                else
                {
                    ClickEvent?.Invoke(pointerUpPosition);
                }
                
                PointerUpEvent?.Invoke(pointerUpPosition);

                _isClick = false;
            }
            if (Input.GetMouseButton(0))
            {
                var pointerPosition = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                DragContinueEvent?.Invoke(pointerPosition);
            }
        }

        private void LateUpdate()
        {
            if (!_isClick)
                return;

            _clickHoldDuration += Time.deltaTime;
            if (_clickHoldDuration >= clickToDragDuration)
            {
                DragStartEvent?.Invoke(_pointerDownPosition);
                AlternativeDragStartEvent?.Invoke(_pointerDownPosition);
                _isClick = false;
                _isDrag = true;
            }
        }

        public void SetDragEventHandlers(Action<Vector3> dragStartEvent, Action<Vector3> dragEndEvent)
        {
            ClearEvents();

            DragStartEvent = dragStartEvent;
            DragEndEvent = dragEndEvent;
        }
        public void SetDragEventHandlers(Action<Vector3> dragStartEvent, Action<Vector3> dragEndEvent, Action<Vector3> dragContinueEvent)
        {
            ClearNewEvents();

            AlternativeDragStartEvent += dragStartEvent;
            AlternativeDragEndEvent += dragEndEvent;
            DragContinueEvent += dragContinueEvent;
        }

        private void ClearNewEvents()
        {
            AlternativeDragStartEvent = null;
            AlternativeDragEndEvent = null;
            DragContinueEvent = null;
        }

        public void ClearEvents()
        {
            DragStartEvent = null;
            DragEndEvent = null;
        }
    }
}