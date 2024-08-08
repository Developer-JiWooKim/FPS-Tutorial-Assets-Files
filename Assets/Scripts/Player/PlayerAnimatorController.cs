using UnityEngine;

/// <summary>
/// �÷��̾� ĳ������ �ִϸ��̼��� �����ϴ� ��Ʈ�ѷ�
/// </summary>
public class PlayerAnimatorController : MonoBehaviour
{
    private Animator    animator;   // �÷��̾��� �ִϸ�����

    /// <summary>
    /// �ִϸ����� �Ķ������ ������Ƽ
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
    /// �ִϸ����� ������Ʈ�� ���� ������ ����
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// ������ �ִϸ��̼��� �����Ű�� �Լ�
    /// </summary>
    public void OnReload()
    {
        animator.SetTrigger("OnReload");
    }

    /// <summary>
    /// �÷��̾� ���¿� ���� �ִϸ��̼��� �����Ű�� �Լ�
    /// </summary>
    public void Play(string stateName, int layer, float normalizedTime)
    {
        animator.Play(stateName, layer, normalizedTime);
    }

    /// <summary>
    /// �ֱ� �ִϸ��̼��� �̸��� ��ȯ�ϴ� �Լ�
    /// </summary>
    public bool CurrentAnimationIs(string name)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    /// <summary>
    /// �ִϸ��̼��� Float���� ��ȯ�ϴ� �Լ�
    /// </summary>
    public void SetFloat(string paramName, float _value)
    {
        animator.SetFloat(paramName, _value);
    }
}
