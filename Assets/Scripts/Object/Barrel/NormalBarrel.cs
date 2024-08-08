using System.Collections;
using UnityEngine;

public class NormalBarrel : InteractionObject
{
    /// <summary>
    /// �� ������Ʈ�� �ʱ� ��ġ, ȸ�� ��
    /// </summary>
    private Vector3         pos;
    private Quaternion      rot;

    private float           delayTime = 5f;     // ���� �ð�

    /// <summary>
    /// �ʱ� ��ġ�� �ʱ� ȸ�� ���� ������ ����
    /// </summary>
    private void Awake()
    {
        pos = transform.position;
        rot = transform.rotation;
    }

    /// <summary>
    /// �������� �Ծ��� �� ����� �Լ�
    /// </summary>
    public override void TakeDamage(int damage)
    {
        StartCoroutine("ResetPosition");
    }

    /// <summary>
    /// n�� �� ������Ʈ�� ��ġ�� �ʱ� ��ġ�� �ǵ����� �ڷ�ƾ 
    /// </summary>
    private IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(delayTime);

        transform.position = pos;
        transform.rotation = rot;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
