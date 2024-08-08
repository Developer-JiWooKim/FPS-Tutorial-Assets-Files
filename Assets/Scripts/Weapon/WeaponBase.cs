using UnityEngine;

public enum WeaponType { Main=0, Sub, Melee, Throw, }

[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { }
[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }

/// <summary>
/// ������� ��� ���� �߻� Ŭ����
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    // �ܺο��� �̺�Ʈ �Լ� ����� �� �� �ֵ��� public ����
    [HideInInspector]
    public AmmoEvent                    onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent                onMagazineEvent = new MagazineEvent();

    [Header("WeaponBase")]
    [SerializeField]
    public WeaponType                   weaponType;         // ���� ����
    [SerializeField]
    protected WeaponSetting             weaponSetting;      // ���� ����

    [SerializeField]
    protected bool                      isLimited = true;   // �Ѿ� ���� �������� �Ǻ��ϴ� ����

    protected float                     lastAttackTime = 0; // ������ �߻�ð� üũ��
    protected bool                      isReload = false;   // ������ ������ üũ
    protected bool                      isAttack = false;   // ���� ���� üũ

    protected AudioSource               audioSource;
    protected PlayerAnimatorController  animator;

    // �޼��� â�� ����� �޼�����
    protected string[]                  messages;
    protected bool[]                    checkMessages;      // �޼����� �� ���� ������ ���� bool �迭 ����

    /// <summary>
    /// �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ ������Ƽ
    /// </summary>
    public PlayerAnimatorController     Animator => animator;
    public WeaponName                   WeaponName => weaponSetting.weaponName;
    public int                          CurrentMagazine => weaponSetting.currentMagazine;
    public int                          MaxMagazine => weaponSetting.maxMagazine;

    public abstract void StartWeaponAction(int type = 0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void StartReload();

    /// <summary>
    /// �ʿ��� ���带 ����ϴ� �Լ�
    /// </summary>
    protected void PlaySound(AudioClip clip)
    {
        audioSource.Stop(); // ������ ������� ���� ����
        audioSource.clip = clip; // ���ο� ���� clip���� ��ü
        audioSource.Play(); // ���� ���
    }

    protected void Setup()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<PlayerAnimatorController>();
    }

    /// <summary>
    /// ��� ���� ������ ������ �� ���� virtual �Լ�
    /// </summary>
    public virtual void ResetMagazineMessage()
    {

    }

    /// <summary>
    /// źâ�� ���� ��Ű�� �Լ�(������ ��� �� �� virtual�Լ��� ������ Ȱ��ȭ�� ������ źâ�� ����)
    /// </summary>
    public virtual void IncreaseMagazine(int _magazine)
    {
        weaponSetting.currentMagazine = CurrentMagazine + _magazine > MaxMagazine ? 
                                        MaxMagazine : CurrentMagazine + _magazine;

        onMagazineEvent.Invoke(CurrentMagazine);
    }

    /// <summary>
    /// MemoryPoolRoom�� �� �� �Ѿ� ������ ����(���ö� �ٽ� ����)
    /// </summary>
    public void MemoryPoolRoomEvent()
    {
        isLimited = !isLimited;
    }
}
