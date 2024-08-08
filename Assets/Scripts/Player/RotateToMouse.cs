using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField]
    private float           rotCamXAxisSpeed = 5;       // X�� ī�޶� ȸ�� �ӵ�

    [SerializeField]
    private float           rotCamYAxisSpeed = 3;       // Y�� ī�޶� ȸ�� �ӵ�

    private float           limitMinX = -80f;           // ī�޶� x�� ȸ�� ����(�ּ�)
    private float           limitMaxX = 50f;            // ī�޶� x�� ȸ�� ����(�ִ�)

    private float           eulerAngleX;                // X�� ȸ�� ����
    private float           eulerAngleY;                // Y�� ȸ�� ����

    /// <summary>
    /// ���콺 �����ӿ� ���� ī�޶� ȸ����Ű�� �Լ�
    /// </summary>
    public void UpdateRotate(float mouseX, float mouseY)
    {
        eulerAngleX -= mouseY * rotCamXAxisSpeed; // ���콺 �� / �� �̵����� ī�޶� x�� ȸ��
        eulerAngleY += mouseX * rotCamYAxisSpeed; // ���콺 �� / �Ʒ� �̵����� ī�޶� y�� ȸ��

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);
        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    /// <summary>
    /// ȸ���� ���ѽ�Ű�� �Լ�
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
