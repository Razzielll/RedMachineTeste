using UnityEngine;
using Utils.Scenes;

namespace Camera
{
    public class CameraController : MonoBehaviour
    {
        private UnityEngine.Camera _camera;

        private void OnEnable()
        {
            ScenesChanger.SceneLoadedEvent += ResetCameraPosition;
        }


        private void OnDisable()
        {
            ScenesChanger.SceneLoadedEvent -= ResetCameraPosition;
        }

        private void Start()
        {
            _camera= CameraHolder.Instance.MainCamera;
        }
        
        private void ResetCameraPosition()
        {
            _camera.transform.position = new Vector3(0, 0, _camera.transform.position.z);
        }
    }
}
