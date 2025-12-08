using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    void LateUpdate()
    {
        // Force this object to face the same direction as the camera
        // (This is smoother than LookAt for 2D sprites)
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }
    }
}