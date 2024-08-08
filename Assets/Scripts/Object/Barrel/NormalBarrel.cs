using System.Collections;
using UnityEngine;

public class NormalBarrel : InteractionObject
{
    /// <summary>
    /// 이 오브젝트의 초기 위치, 회전 값
    /// </summary>
    private Vector3         pos;
    private Quaternion      rot;

    private float           delayTime = 5f;     // 지연 시간

    /// <summary>
    /// 초기 위치와 초기 회전 값을 변수에 저장
    /// </summary>
    private void Awake()
    {
        pos = transform.position;
        rot = transform.rotation;
    }

    /// <summary>
    /// 데미지를 입었을 때 실행될 함수
    /// </summary>
    public override void TakeDamage(int damage)
    {
        StartCoroutine("ResetPosition");
    }

    /// <summary>
    /// n초 후 오브젝트의 위치를 초기 위치로 되돌리는 코루틴 
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
