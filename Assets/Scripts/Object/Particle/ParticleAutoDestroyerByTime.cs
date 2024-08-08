using UnityEngine;

public class ParticleAutoDestroyerByTime : MonoBehaviour
{
    private ParticleSystem      particle;       // 파티클 시스템 컴포넌트

    /// <summary>
    /// 파티클 시스템 컴포넌트를 얻어와 변수에 저장
    /// </summary>
    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();    
    }

    /// <summary>
    /// 파티클(이펙트)이 재생 중이 아니면 자동으로 삭제
    /// </summary>
    void Update()
    {
        if (particle.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}
