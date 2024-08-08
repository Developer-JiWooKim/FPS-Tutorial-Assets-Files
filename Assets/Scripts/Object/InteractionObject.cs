using UnityEngine;

/// <summary>
/// ��ȣ�ۿ� ������Ʈ�� ��� ���� �߻� Ŭ����
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
    /// �������� ���� �� ����� �߻� �޼ҵ�
    /// </summary>
    public abstract void TakeDamage(int damage);
}
