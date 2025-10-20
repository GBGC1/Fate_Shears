using Script.Manager.Events;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 5f;

    private Rect cameraRect;

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 targetPos = new Vector3(player.position.x, player.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        }

        UpdateCameraRect();
    }

    private void UpdateCameraRect()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        cameraRect = new Rect(
            bottomLeft.x,
            bottomLeft.y,
            topRight.x - bottomLeft.x,
            topRight.y - bottomLeft.y
        );

        EventBus.Instance().Publish(new CameraRectEventData(cameraRect));
    }

    public Rect GetCameraRect()
    {
        return cameraRect;
    }
}