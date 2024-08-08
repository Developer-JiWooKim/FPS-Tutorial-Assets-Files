using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField]
    private float           rotCamXAxisSpeed = 5;       // X축 카메라 회전 속도

    [SerializeField]
    private float           rotCamYAxisSpeed = 3;       // Y축 카메라 회전 속도

    private float           limitMinX = -80f;           // 카메라 x축 회전 범위(최소)
    private float           limitMaxX = 50f;            // 카메라 x축 회전 범위(최대)

    private float           eulerAngleX;                // X축 회전 각도
    private float           eulerAngleY;                // Y축 회전 각도

    /// <summary>
    /// 마우스 움직임에 따라 카메라를 회전시키는 함수
    /// </summary>
    public void UpdateRotate(float mouseX, float mouseY)
    {
        eulerAngleX -= mouseY * rotCamXAxisSpeed; // 마우스 좌 / 우 이동으로 카메라 x축 회전
        eulerAngleY += mouseX * rotCamYAxisSpeed; // 마우스 위 / 아래 이동으로 카메라 y축 회전

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);
        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    /// <summary>
    /// 회전을 제한시키는 함수
    /// </summary>
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
        {
            angle += 360f;
        }
        if (angle > 360f)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, min, max);
    }
}
