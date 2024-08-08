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
    private AudioClip           audioClipFire; // 공격 사운드

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
    /// 무기 초기화, 메세지 초기화
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
    /// Grenade는 재장전이 없으므로 따로 처리 X
    /// </summary>
    public override void StartReload()
    {
        
    }

    /// <summary>
    /// 공격 버튼을 눌렀을 때 호출될 함수
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
    /// 메세지 배열에 메시지 입력
    /// </summary>
    private void InputMessage()
    {
        messages = new string[1];
        checkMessages = new bool[1];

        messages[(int)E_MessageIndex.MagazineReload] = "We're out of grenade!";
        checkMessages[(int)E_MessageIndex.MagazineReload] = false;
    }

    /// <summary>
    /// 메세지 초기화 함수(ResetMagazine)
    /// </summary>
    public override void ResetMagazineMessage()
    {
        checkMessages[(int)E_MessageIndex.MagazineReload] = false;
    }

    /// <summary>
    /// 연속 공격이 없으므로 따로 처리 X
    /// </summary>
    public override void StopWeaponAction(int type = 0)
    {
        
    }
    
    /// <summary>
    /// 공격 코루틴 함수
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
    /// 공격 시 Grenade를 생성하는 함수(공격 시 남은 Ammo 감소, UI처리)
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
    /// Magazine 아이템을 사용 시 호출될 함수
    /// </summary>
    public override void IncreaseMagazine(int _ammo)
    {
        // 투척 무기 수류탄은 탄창의 개념 X -> 탄(Ammo)가 수류탄의 개수이기 때문에 탄 수를 증가시킴
        weaponSetting.currentAmmo = weaponSetting.currentAmmo + _ammo > weaponSetting.maxAmmo ?
                                    weaponSetting.maxAmmo : weaponSetting.currentAmmo + _ammo;

        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }
}
