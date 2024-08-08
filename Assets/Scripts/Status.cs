using UnityEngine;

[System.Serializable]
public class HPEvent :  UnityEngine.Events.UnityEvent<int, int> { }

/// <summary>
/// HP�� ���� ������Ʈ���� ���Ե� Status Ŭ����
/// </summary>
public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent  onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float   walkSpeed;
    [SerializeField]
    private float   runSpeed;

    [Header("HP")]
    [SerializeField]
    private int     maxHP = 100;
    private int     currentHP;

    /// <summary>
    /// �д� �뵵�� property
    /// </summary>
    public float    WalkSpeed => walkSpeed;
    public float    RunSpeed => runSpeed;
    public float    MaxHP => maxHP;
    public int      CurrentHP => currentHP;

    /// <summary>
    /// ü���� ������ ������Ʈ���� ���۰� ���ÿ� �ִ� ü���� ������ ����
    /// </summary>
    private void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// ü���� ������ �� ȣ���� �Լ�
    /// </summary>
    public bool DecreaseHP(int damage)
    {
        int previousHP = currentHP;

        //(���� ü�� - ���� ������) �� 0���� ũ�� ���� ü�¿��� ���� �������� ���� ��Ŵ, 0���� ������ ���� ü���� 0���� ����
        currentHP = currentHP - damage > 0 ? currentHP - damage : 0;

        // onHPEvent�� ��ϵ� �̺�Ʈ ����, UI�� ������Ʈ
        onHPEvent.Invoke(previousHP, currentHP);

        // ���� ü���� 0�̸� true����
        if (currentHP == 0) 
        {
            return true;
        }

        return false; // �ƴϸ� false����
    }

    /// <summary>
    /// ü�� ȸ�� �������� �Ծ��� �� ȣ���� �Լ�
    /// </summary>
    public void IncreaseHP(int hp)
    {
        int previousHP = currentHP;

        //(���� ü�� + ȸ�� ü��) �� �ִ� ü�� ���� ũ�� ü���� �ִ� ü������ ����
        currentHP = currentHP + hp > maxHP ? maxHP : currentHP + hp;

        // onHPEvent�� ��ϵ� �̺�Ʈ ����, UI�� ������Ʈ
        onHPEvent.Invoke(previousHP, currentHP);
    }

    /// <summary>
    /// ü���� �ʱ�ȭ
    /// </summary>
    public void SetUpHP()
    {
        currentHP = maxHP;
    }
}
