// Erdroy's Game of Life © 2016-2017 Damian 'Erdroy' Korczowski

using UnityEngine;

namespace Life
{
    public class CameraController : MonoBehaviour
    {
        public float MaxZoom = 1.0f;
        public float MinZoom = 0.01f;

        public AnimationCurve SpeedOverZoom;

        private Camera _camera;
        private Vector3 _dragStart;
        private Vector3 _startPos;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                _dragStart = Input.mousePosition;
                _startPos = transform.position;
            }

            if (Input.GetKey(KeyCode.Mouse2))
            {
                var current = (Input.mousePosition - _dragStart) * 0.005f;
                current *= _camera.orthographicSize;

                transform.position = _startPos - current;
            }
            else
            {
                var percent = 1.0f - MaxZoom / (MinZoom + _camera.orthographicSize);

                _camera.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * SpeedOverZoom.Evaluate(percent);
            }

            var size = _camera.orthographicSize;
            _camera.orthographicSize = Mathf.Clamp(size, MinZoom, MaxZoom);
        }
    }
}