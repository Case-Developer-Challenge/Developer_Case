using UnityEngine;

namespace Board
{
    public class BoardVisualController : MonoBehaviour
    {
        [SerializeField] private float sizeOffset;
        public void SetCameraPosition(Vector2 size)
        {
            var cameraMain = Camera.main;
            cameraMain.transform.position = Vector3.back * 5;

            if (size.x > size.y)
            {
                cameraMain.orthographicSize = size.x * 1 / cameraMain.aspect / sizeOffset;
                if (cameraMain.aspect < 1) 
                    cameraMain.orthographicSize *= 1.3f;
            }
            else
            {
                cameraMain.orthographicSize = size.y * 1 / cameraMain.aspect / sizeOffset;
            }
        }
    }
}