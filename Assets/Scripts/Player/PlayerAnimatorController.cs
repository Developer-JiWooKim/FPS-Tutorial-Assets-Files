using UnityEngine;

/// <summary>
/// 플레이어 캐릭터의 애니메이션을 제어하는 컨트롤러
/// </summary>
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator    animator;   // 플레이어의 애니메이터

    /// <summary>
    /// 애니메이터 파라미터의 프로퍼티
    /// </summary>
    public float MoveSpeed
    {
        set => animator.SetFloat("MovementSpeed", value);
        get => animator.GetFloat("MovementSpeed");
    }

    public bool AimModeIs
    {
        set => animator.SetBool("isAimMode", value);
        get => animator.GetBool("isAimMode");
    }

    /// <summary>
    /// 애니메이터 컴포넌트를 얻어와 변수에 저장
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 재장전 애니메이션을 재생시키는 함수
    /// </summary>
    public void OnReload()
    {
        animator.SetTrigger("OnReload");
    }

    /// <summary>
    /// 플레이어 상태에 따라 애니메이션을 재생시키는 함수
    /// </summary>
    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);
    }

    /// <summary>
    /// 최근 애니메이션의 이름을 반환하는 함수
    /// </summary>
    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    /// <summary>
    /// 애니메이션의 Float값을 반환하는 함수
    /// </summary>
    public void SetFloat(string paramName, float _value)
    {
        animator.SetFloat(paramName, _value);
    }
}
