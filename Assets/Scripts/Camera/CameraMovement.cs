 using UnityEngine;

namespace Camera
{
    public class CameraMovement : MonoBehaviour
    {
        [Tooltip("Скорость движения камеры")]
        [SerializeField] private float moveSpeed = 10f;
        [Tooltip("Коэффициент затухания скорости движения камеры")]
        [Range(0f, 1f)]
        [SerializeField] private float dampingFactor = 0.98f;
        
        [Tooltip("Ограничивает движение элементов относительно границы экрана")]
        [SerializeField] private float borderOffset = 0.1f;

        private Vector3 currentMoveSpeed = Vector3.zero;
        private UnityEngine.Camera _camera;
        private BoundsCalculator _boundsCalculator;

        private void Start()
        {
            _camera = CameraHolder.Instance.MainCamera;
            _boundsCalculator = GetComponent<BoundsCalculator>();
        }

        private void Update()
        {
            Vector3 moveDelta = new Vector3(-currentMoveSpeed.x, -currentMoveSpeed.y, 0);
            if (!KeepInSight(moveDelta))
            {
                currentMoveSpeed = Vector3.zero;
                return;
            }

            transform.position = new Vector3(transform.position.x + currentMoveSpeed.x,
                transform.position.y + currentMoveSpeed.y, transform.position.z);
            currentMoveSpeed *= dampingFactor;
        }

        public void SetMoveSpeed(Vector3 speed)
        {
            currentMoveSpeed = speed;
        }

        private bool KeepInSight(Vector3 moveDelta)
        {
            Vector3 viewPosMin = _camera.WorldToViewportPoint(_boundsCalculator.GetMinBounds()+moveDelta);
            Vector3 viewPosMax = _camera.WorldToViewportPoint(_boundsCalculator.GetMaxBounds()+moveDelta);

            if (viewPosMax.x < borderOffset || viewPosMax.y < borderOffset || viewPosMin.x > 1-borderOffset || viewPosMin.y > 1-borderOffset)
            {
                return false;
            }

            return true;
        }
        
        public float GetMoveSpeed()
        {
            return moveSpeed;
        }
        
        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
        }
        
        public float GetDampingFactor()
        {
            return dampingFactor;
        }
        
        public Vector3 GetCurrentMoveSpeed()
        {
            return currentMoveSpeed;
        }
    }
}