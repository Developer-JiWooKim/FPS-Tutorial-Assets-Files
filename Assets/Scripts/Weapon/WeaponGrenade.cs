using System.Collections;
using UnityEngine;

public class WeaponGrenade : WeaponBase
{
    private enum E_MessageIndex
    {
        MagazineReload = 0,
    }

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip           audioClipFire; // ���� ����

    [Header("Grenade")]
    [SerializeField]
    private GameObject          grenadePrefab;
    [SerializeField]
    private Transform           grenadeSpawnPoint;

    [SerializeField]
    private MessageController   MessageController;

    [SerializeField]
    private GameObject          grenadeOBJ;

    /// <summary>
    /// ���� �ʱ�ȭ, �޼��� �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        base.Setup();

        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        InputMessage();
    }

    private void OnEnable()
    {
        if (isLimited == false)
        {
            onMagazineEvent.Invoke(0);
            onAmmoEvent.Invoke(999,999);
            weaponSetting.currentAmmo = weaponSetting.maxAmmo;
        }
        else
        {
            onMagazineEvent.Invoke(weaponSetting.currentMagazine);
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        }
    }

    private void Update()
    {
        if (weaponSetting.currentAmmo > 0)
        {
            grenadeOBJ.SetActive(true);
        }
        else
        {
            grenadeOBJ.SetActive(false);
        }
    }

    /// <summary>
    /// Grenade�� �������� �����Ƿ� ���� ó�� X
    /// </summary>
    public override void StartReload()
    {
        
    }

    /// <summary>
    /// ���� ��ư�� ������ �� ȣ��� �Լ�
    /// </summary>
    public override void StartWeaponAction(int type = 0)
    {
        if (weaponSetting.currentAmmo <= 0)
        {
            if (checkMessages[(int)E_MessageIndex.MagazineReload] == false)
            {
                MessageController.MessageSave(messages[(int)E_MessageIndex.MagazineReload]);
                checkMessages[(int)E_MessageIndex.MagazineReload] = true;
            }
            return;
        }
        if (type == 0 && isAttack == false)
        {
            StartCoroutine("OnAttack");
        }
    }

    /// <summary>
    /// �޼��� �迭�� �޽��� �Է�
    /// </summary>
    private void InputMessage()
    {
        messages = new string[1];
        checkMessages = new bool[1];

        messages[(int)E_MessageIndex.MagazineReload] = "We're out of grenade!";
        checkMessages[(int)E_MessageIndex.MagazineReload] = false;
    }

    /// <summary>
    /// �޼��� �ʱ�ȭ �Լ�(ResetMagazine)
    /// </summary>
    public override void ResetMagazineMessage()
    {
        checkMessages[(int)E_MessageIndex.MagazineReload] = false;
    }

    /// <summary>
    /// ���� ������ �����Ƿ� ���� ó�� X
    /// </summary>
    public override void StopWeaponAction(int type = 0)
    {
        
    }
    
    /// <summary>
    /// ���� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator OnAttack()
    {
        isAttack = true;

        animator.Play("Fire", -1, 0);

        PlaySound(audioClipFire);

        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (animator.CurrentAnimationIs("Movement"))
            {
                isAttack = false;

                yield break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// ���� �� Grenade�� �����ϴ� �Լ�(���� �� ���� Ammo ����, UIó��)
    /// </summary>
    public void SpawnGrenadeProjectile()
    {
        GameObject grenadeClone = Instantiate(grenadePrefab, grenadeSpawnPoint.position, Random.rotation);
        grenadeClone.GetComponent<WeaponGrenadeProjectile>().Setup(weaponSetting.damage, transform.parent.forward);

        if (isLimited == true)
        {
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
        }
        
    }

    /// <summary>
    /// Magazine �������� ��� �� ȣ��� �Լ�
    /// </summary>
    public override void IncreaseMagazine(int _ammo)
    {
        // ��ô ���� ����ź�� źâ�� ���� X -> ź(Ammo)�� ����ź�� �����̱� ������ ź ���� ������Ŵ
        weaponSetting.currentAmmo = weaponSetting.currentAmmo + _ammo > weaponSetting.maxAmmo ?
                                    weaponSetting.maxAmmo : weaponSetting.currentAmmo + _ammo;

        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }
}
