using UnityEngine;

public class WeaponSwitchSystem : MonoBehaviour
{
    /// <summary>
    /// �÷��̾� ����
    /// </summary>
    [SerializeField]
    private PlayerController    playerController;
    [SerializeField]
    private PlayerHUD           playerHUD;         

    [SerializeField]
    private bool                isSwitchWeapon;     // ���⸦ �ٲ� �� �ִ� �������� �˻�� ����

    [SerializeField]
    private WeaponBase[]        weapons;            // �������� ���� ������ 

    private WeaponBase          currentWeapon;      // ���� ����  
    private WeaponBase          previousWeapon;     // ���� ����

    /// <summary>
    /// �̼� ���� ����
    /// </summary>
    private const int           weaponChangeMissionNumber = 0;
    private bool                switchMission = false; 

    /// <summary>
    /// �ʱ�ȭ �۾�
    /// </summary>
    private void Awake()
    {
        // ���� ���� ����� ���� ���� �������� ��� ���� �̺�Ʈ ���
        playerHUD.SetupAllWeapons(weapons);

        // ���� �������� ��� ���⸦ ������ �ʰ� ����
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].gameObject != null)
            {
                weapons[i].gameObject.SetActive(false);
            }
        }

        {
            isSwitchWeapon = false;
        }

        // Main ���⸦ ���� ����� ���
        SwitchingWeapon(WeaponType.Main);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    /// <summary>
    /// �濡 �� ���� ���� Ȱ��ȭ �����ϰ��ϴ� �Լ�
    /// </summary>
    private void CheckWeaponEnable()
    {
        // �濡 �� ���� ���� Ȱ��ȭ �����ϰ�

        // ���⸦ �ٲ� �� ������
        if (isSwitchWeapon == false) 
        {
            // ���� ���Ⱑ Ȱ��ȭ �Ǿ������� ���⸦ ��Ȱ��ȭ, UI�� ��Ȱ��ȭ
            if (currentWeapon.gameObject.activeSelf == true)
            {
                currentWeapon.gameObject.SetActive(false);
                playerHUD.WeaponUIOnOff(); // UI�� ��
            }
            return;
        }
        // ���⸦ �ٲ� �� ������
        else
        { 
          // ���� ���Ⱑ ��Ȱ��ȭ �Ǿ������� ���⸦ Ȱ��ȭ, UI�� Ȱ��ȭ
            if (currentWeapon.gameObject.activeSelf == false)
            {
                currentWeapon.gameObject.SetActive(true); 
                playerHUD.WeaponUIOnOff(); // UI�� Ŵ

            }
        }
    }

    /// <summary>
    /// ���� ���⸦ ���������� üũ(1 ~ 4Ű�� ������ ���⸦ ��ü)
    /// </summary>
    private void UpdateSwitch()
    {
        CheckWeaponEnable();

        // ���⸦ �ٲ� �� ������
        if (isSwitchWeapon == false) 
        { 
            return;
        }

        if ( !Input.anyKeyDown )
        {
            return;
        }

        // 1 ~ 4����Ű�� ������ ���� ��ü
        int inputIndex = 0;
        if ( int.TryParse(Input.inputString, out inputIndex) && (inputIndex > 0 && inputIndex < 5) )
        {
            if (switchMission == false)
            {
                Mission_WeaponSwitch();
            }
            SwitchingWeapon( (WeaponType)(inputIndex - 1) );
        }
    }

    /// <summary>
    /// WeaponSwitch �̼� Ŭ���� �� ȣ��� �Լ�
    /// </summary>
    private void Mission_WeaponSwitch()
    {
        // Ȱ��ȭ�� �̼��� �ְ� �� �̼��� WeaponChange�� �̼��� ��
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) && 
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.ChangeWeapon);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.ChangeWeapon, weaponChangeMissionNumber); 
            switchMission = true;
        }
    }

    /// <summary>
    /// ���� ���� �Լ�
    /// </summary>
    private void SwitchingWeapon(WeaponType weaponType)
    {
        // ��ü ������ ���� ������ ����
        if (weapons[(int)weaponType] == null)
        {
            Debug.Log("[WeaponSwitchSystem.cs] ��ü ������ ���Ⱑ ����!");
            return;
        }

        // ���� ������� ���Ⱑ ������ ���� ���� ������ ����
        if (currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }

        // ���� ���Ӹ�� ���̸� ���� ��ü�� ���ϰ�
        if (currentWeapon != null)
        {
            if (currentWeapon.Animator.AimModeIs == true)
            {
                return;
            }
        }

        // ���� ��ü
        currentWeapon = weapons[(int)weaponType];

        // ���� ������� ����� ��ü�Ϸ��� �� �� ����
        if (currentWeapon == previousWeapon)
        {
            return;
        }

        // ���⸦ ����ϴ� PlayerController, PlayerHUD�� ���� ���� ���� ����
        playerController.SwitchingWeapon(currentWeapon);
        playerHUD.SwitchingWeapon(currentWeapon);

        // ������ ����ϴ� ���� ��Ȱ��ȭ
        if (previousWeapon != null)
        {
            previousWeapon.gameObject.SetActive(false);
        }

        // ���� ����ϴ� ���� Ȱ��ȭ
        currentWeapon.gameObject.SetActive(true);

        // ���� ���� �ٲ� �� �ִ� ���°� �ƴϸ� ���� ���� ��Ȱ��ȭ()
        if (isSwitchWeapon == false)
        {
            currentWeapon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// źâ�� ������Ű�� �Լ�(���� Ÿ���� �Ű������� �޾� onMagazineEvent�� ����)
    /// </summary>
    public void IncreaseMagazine(WeaponType _weaponType, int _magazine)
    {
        if (weapons[(int)_weaponType] != null)
        {
            weapons[(int)_weaponType].IncreaseMagazine(_magazine);
            weapons[(int)_weaponType].onMagazineEvent.Invoke(currentWeapon.CurrentMagazine);
        }
    }

    /// <summary>
    /// źâ�� ������Ű�� �Լ�(Grenade)
    /// </summary>
    public void IncreaseMagazine(int _magazine)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].IncreaseMagazine(_magazine);
            }
        }
    }

    /// <summary>
    /// �ֱ� ������ Ÿ���� ��ȯ�ϴ� �Լ�
    /// </summary>
    public WeaponType GetCurrentWeaponType()
    {
        return currentWeapon.weaponType;
    }

    /// <summary>
    /// �濡 �� ���� ���� �� �ܺο��� ȣ��� �Լ� 
    /// </summary>
    public void SetIsSwitchWeapon()
    {
        isSwitchWeapon = !isSwitchWeapon;
    }
}
