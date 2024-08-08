using System.Collections;
using UnityEngine;

public class WeaponKnife : WeaponBase
{
    [SerializeField]
    private WeaponKnifeCollider     weaponKnifeCollider;                    // 공격 시 활성화될 콜라이더

    [SerializeField]
    private GameObject              knifeOBJ;

    /// <summary>
    /// 미션과 관련된 변수
    /// </summary>
    private const int               weaponChangeMissionNumber = 2;          // 에임모드 미션 번호
    private bool                    differentAttackMotionMission = false;

    /// <summary>
    /// 무기 초기화
    /// </summary>
    private void Awake()
    {
        base.Setup();

        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    private void OnEnable()
    {
        isAttack = false;

        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);
    }

    /// <summary>
    /// Knife는 총알, 탄창이 없으므로 따로 처리 X
    /// </summary>
    public override void StartReload()
    {
        
    }

    /// <summary>
    /// 공격 버튼을 누르고 있으면 연속 공격, 아니면 단일 공격을 실행
    /// </summary>
    public override void StartWeaponAction(int type = 0)
    {
        if (isAttack == true)
        {
            return;
        }
        if (differentAttackMotionMission == false)
        {
            if (type == 1)
            {
                Mission_DifferentAttackMotion();
            }
        }
        if (weaponSetting.isAutomaticAttack == true) // 연속 공격
        {
            StartCoroutine("OnAttackLoop", type);
        }
        else // 단일 공격
        {
            StartCoroutine("OnAttack");
        }
    }

    /// <summary>
    /// DifferentAttackMotion 미션이 클리어 시 호출될 함수
    /// </summary>
    private void Mission_DifferentAttackMotion()
    {
        // 활성화된 미션이 있고 그 미션이 WeaponChangeRoom의 미션일 때
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.ChangeWeapon);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.ChangeWeapon, weaponChangeMissionNumber);
            differentAttackMotionMission = true;
        }
    }

    /// <summary>
    /// 공격을 멈출때 호출되는 함수
    /// </summary>
    public override void StopWeaponAction(int type = 0)
    {
        isAttack = false;
        StopCoroutine("OnAttackLoop");
    }

    /// <summary>
    /// 공격을 연속으로 실행할 때 호출되는 코루틴 함수
    /// </summary>
    private IEnumerator OnAttackLoop(int _type)
    {
        while (true)
        {
            yield return StartCoroutine("OnAttack", _type);
        }
    }

    /// <summary>
    /// 공격 코루틴 함수
    /// </summary>
    private IEnumerator OnAttack(int _type)
    {
        isAttack = true;
        knifeOBJ.SetActive(false);

        // 공격 모션 선택 (0, 1)
        animator.SetFloat("attackType", _type);
        if (_type == 1)
        {
            knifeOBJ.SetActive(true);
        }
        // 공격 애니메이션 재생
        animator.Play("Fire", -1, 0);

        yield return new WaitForEndOfFrame();

        while (true)
        {
            if (animator.CurrentAnimationIs("Movement"))
            {
                knifeOBJ.SetActive(true);
                isAttack = false;
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 애니메이션 재생에 맞춰 호출될 함수
    /// </summary>
    public void StartWeaponKnifeCollider()
    {
        weaponKnifeCollider.StartCollider(weaponSetting.damage);
    }
}
