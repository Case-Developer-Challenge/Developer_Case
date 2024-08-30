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
            float biggerSize;
            if (size.x > size.y)
                biggerSize = size.x;
            else
                biggerSize = size.y;
            cameraMain.orthographicSize = biggerSize * 1 / cameraMain.aspect / sizeOffset;
        }
    }
}