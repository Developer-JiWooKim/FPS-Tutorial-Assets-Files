using System.Collections;
using UnityEngine;

public class Casing : MonoBehaviour
{
    [SerializeField]
    private float           deactivateTime = 5.0f;      // 탄피 등장 후 비활성화 되는 시간 

    [SerializeField]
    private float           casingSpin = 1.0f;          // 탄피가 회전하는 속력 계수

    [SerializeField]
    private AudioClip[]     audioClips;                 // 탄피가 부딪혔을 때 재생되는 사운드

    private Rigidbody       rigidbody3D;
    private AudioSource     audioSource;
    private MemoryPool      memoryPool;

    /// <summary>
    /// 탄피 초기화 함수
    /// </summary>
    public void SetUp(MemoryPool pool, Vector3 direction)
    {
        rigidbody3D = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        memoryPool = pool;

        /* 탄피의 이동 속력과 회전 속력 설정*/

        // 이동
        rigidbody3D.velocity = new Vector3(direction.x, 1.0f, direction.z);
        // 회전
        rigidbody3D.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                                Random.Range(-casingSpin, casingSpin),
                                                Random.Range(-casingSpin, casingSpin));

        // 탄피 자동 비활성화를 위한 코루틴 실행
        StartCoroutine("DeactiveAfterTime");

    }

    /// <summary>
    /// deactivateTime 시간 후 탄피를 비활성화하는 코루틴 함수
    /// </summary>
    private IEnumerator DeactiveAfterTime()
    {
        yield return new WaitForSeconds(deactivateTime);

        memoryPool.DeactivatePoolItem(this.gameObject);
    }

    /// <summary>
    /// 탄피가 부딪혔을 때 사운드를 재생시키기 위한 충돌 처리
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

}
