using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float sensitivity = 10f; // 마우스 감도
    public float minYAngle = -60f; // 최소 Y축 회전 각도
    public float maxYAngle = 60f; // 최대 Y축 회전 각도
    private float currentRotationX;
    private float currentRotationY;

    void Start()
    {
        // 커서를 잠그고 숨깁니다.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 마우스 입력을 받습니다.
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 현재 회전 값을 갱신합니다.
        currentRotationX -= mouseY;
        currentRotationX = Mathf.Clamp(currentRotationX, minYAngle, maxYAngle); // Y축 회전 범위를 제한합니다.
        currentRotationY += mouseX;

        // 카메라의 회전을 적용합니다.
        transform.eulerAngles = new Vector3(currentRotationX, currentRotationY, 0);

        // 카메라의 위치를 타겟의 위치에 맞추되, 회전된 상태를 유지합니다.
        transform.position = target.position + Quaternion.Euler(currentRotationX, currentRotationY, 0) * offset;
    }
}