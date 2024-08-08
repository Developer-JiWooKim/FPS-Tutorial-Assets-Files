using System.Collections;
using UnityEngine;

public class WeaponKnife : WeaponBase
{
    [SerializeField]
    private WeaponKnifeCollider     weaponKnifeCollider;                    // ���� �� Ȱ��ȭ�� �ݶ��̴�

    [SerializeField]
    private GameObject              knifeOBJ;

    /// <summary>
    /// �̼ǰ� ���õ� ����
    /// </summary>
    private const int               weaponChangeMissionNumber = 2;          // ���Ӹ�� �̼� ��ȣ
    private bool                    differentAttackMotionMission = false;

    /// <summary>
    /// ���� �ʱ�ȭ
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
    /// Knife�� �Ѿ�, źâ�� �����Ƿ� ���� ó�� X
    /// </summary>
    public override void StartReload()
    {
        
    }

    /// <summary>
    /// ���� ��ư�� ������ ������ ���� ����, �ƴϸ� ���� ������ ����
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
        if (weaponSetting.isAutomaticAttack == true) // ���� ����
        {
            StartCoroutine("OnAttackLoop", type);
        }
        else // ���� ����
        {
            StartCoroutine("OnAttack");
        }
    }

    /// <summary>
    /// DifferentAttackMotion �̼��� Ŭ���� �� ȣ��� �Լ�
    /// </summary>
    private void Mission_DifferentAttackMotion()
    {
        // Ȱ��ȭ�� �̼��� �ְ� �� �̼��� WeaponChangeRoom�� �̼��� ��
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) &&
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.ChangeWeapon);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.ChangeWeapon, weaponChangeMissionNumber);
            differentAttackMotionMission = true;
        }
    }

    /// <summary>
    /// ������ ���⶧ ȣ��Ǵ� �Լ�
    /// </summary>
    public override void StopWeaponAction(int type = 0)
    {
        isAttack = false;
        StopCoroutine("OnAttackLoop");
    }

    /// <summary>
    /// ������ �������� ������ �� ȣ��Ǵ� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator OnAttackLoop(int _type)
    {
        while (true)
        {
            yield return StartCoroutine("OnAttack", _type);
        }
    }

    /// <summary>
    /// ���� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator OnAttack(int _type)
    {
        isAttack = true;
        knifeOBJ.SetActive(false);

        // ���� ��� ���� (0, 1)
        animator.SetFloat("attackType", _type);
        if (_type == 1)
        {
            knifeOBJ.SetActive(true);
        }
        // ���� �ִϸ��̼� ���
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
    /// �ִϸ��̼� ����� ���� ȣ��� �Լ�
    /// </summary>
    public void StartWeaponKnifeCollider()
    {
        weaponKnifeCollider.StartCollider(weaponSetting.damage);
    }
}
