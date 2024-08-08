using UnityEngine;

/// <summary>
/// 상호작용 오브젝트가 상속 받을 추상 클래스
/// </summary>
public abstract class InteractionObject : MonoBehaviour
{
    [Header("Interaction Object")]
    [SerializeField]
    protected int maxHP = 100;
    protected int currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 데미지를 받을 시 실행될 추상 메소드
    /// </summary>
    public abstract void TakeDamage(int damage);
}
