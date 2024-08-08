using UnityEngine;

public class WeaponSwitchSystem : MonoBehaviour
{
    /// <summary>
    /// 플레이어 관련
    /// </summary>
    [SerializeField]
    private PlayerController    playerController;
    [SerializeField]
    private PlayerHUD           playerHUD;         

    [SerializeField]
    private bool                isSwitchWeapon;     // 무기를 바꿀 수 있는 상태인지 검사용 변수

    [SerializeField]
    private WeaponBase[]        weapons;            // 소지중인 무기 종류들 

    private WeaponBase          currentWeapon;      // 현재 무기  
    private WeaponBase          previousWeapon;     // 이전 무기

    /// <summary>
    /// 미션 관련 변수
    /// </summary>
    private const int           weaponChangeMissionNumber = 0;
    private bool                switchMission = false; 

    /// <summary>
    /// 초기화 작업
    /// </summary>
    private void Awake()
    {
        // 무기 정보 출력을 위해 현재 소지중인 모든 무기 이벤트 등록
        playerHUD.SetupAllWeapons(weapons);

        // 현재 소지중인 모든 무기를 보이지 않게 설정
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

        // Main 무기를 현재 무기로 사용
        SwitchingWeapon(WeaponType.Main);
    }

    private void Update()
    {
        UpdateSwitch();
    }

    /// <summary>
    /// 방에 들어갈 때만 무기 활성화 가능하게하는 함수
    /// </summary>
    private void CheckWeaponEnable()
    {
        // 방에 들어갈 때만 무기 활성화 가능하게

        // 무기를 바꿀 수 없으면
        if (isSwitchWeapon == false) 
        {
            // 현재 무기가 활성화 되어있으면 무기를 비활성화, UI를 비활성화
            if (currentWeapon.gameObject.activeSelf == true)
            {
                currentWeapon.gameObject.SetActive(false);
                playerHUD.WeaponUIOnOff(); // UI를 끔
            }
            return;
        }
        // 무기를 바꿀 수 있으면
        else
        { 
          // 현재 무기가 비활성화 되어있으면 무기를 활성화, UI를 활성화
            if (currentWeapon.gameObject.activeSelf == false)
            {
                currentWeapon.gameObject.SetActive(true); 
                playerHUD.WeaponUIOnOff(); // UI를 킴

            }
        }
    }

    /// <summary>
    /// 현재 무기를 지속적으로 체크(1 ~ 4키를 누르면 무기를 교체)
    /// </summary>
    private void UpdateSwitch()
    {
        CheckWeaponEnable();

        // 무기를 바꿀 수 없으면
        if (isSwitchWeapon == false) 
        { 
            return;
        }

        if ( !Input.anyKeyDown )
        {
            return;
        }

        // 1 ~ 4숫자키를 누르면 무기 교체
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
    /// WeaponSwitch 미션 클리어 시 호출될 함수
    /// </summary>
    private void Mission_WeaponSwitch()
    {
        // 활성화된 미션이 있고 그 미션이 WeaponChange의 미션일 때
        bool isComplete = (MissionSystem.missionSystem.isActiveMission == true) && 
                           MissionSystem.missionSystem.activeMission.GetComponent<MissionFunc>().GetRoomNum() == ((int)E_RoomName.ChangeWeapon);
        if (isComplete)
        {
            MissionSystem.missionSystem.MissionComplete((int)E_RoomName.ChangeWeapon, weaponChangeMissionNumber); 
            switchMission = true;
        }
    }

    /// <summary>
    /// 무기 변경 함수
    /// </summary>
    private void SwitchingWeapon(WeaponType weaponType)
    {
        // 교체 가능한 무기 없으면 종료
        if (weapons[(int)weaponType] == null)
        {
            Debug.Log("[WeaponSwitchSystem.cs] 교체 가능한 무기가 없음!");
            return;
        }

        // 현재 사용중인 무기가 있으면 이전 무기 정보에 저장
        if (currentWeapon != null)
        {
            previousWeapon = currentWeapon;
        }

        // 현재 에임모드 중이면 무기 교체를 못하게
        if (currentWeapon != null)
        {
            if (currentWeapon.Animator.AimModeIs == true)
            {
                return;
            }
        }

        // 무기 교체
        currentWeapon = weapons[(int)weaponType];

        // 현재 사용중인 무기로 교체하려고 할 때 종료
        if (currentWeapon == previousWeapon)
        {
            return;
        }

        // 무기를 사용하는 PlayerController, PlayerHUD에 현재 무기 정보 전달
        playerController.SwitchingWeapon(currentWeapon);
        playerHUD.SwitchingWeapon(currentWeapon);

        // 이전에 사용하던 무기 비활성화
        if (previousWeapon != null)
        {
            previousWeapon.gameObject.SetActive(false);
        }

        // 현재 사용하는 무기 활성화
        currentWeapon.gameObject.SetActive(true);

        // 현재 무기 바꿀 수 있는 상태가 아니면 현재 무기 비활성화()
        if (isSwitchWeapon == false)
        {
            currentWeapon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 탄창을 증가시키는 함수(무기 타입을 매개변수로 받아 onMagazineEvent를 실행)
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
    /// 탄창을 증가시키는 함수(Grenade)
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
    /// 최근 무기의 타입을 반환하는 함수
    /// </summary>
    public WeaponType GetCurrentWeaponType()
    {
        return currentWeapon.weaponType;
    }

    /// <summary>
    /// 방에 들어갈 때랑 나올 때 외부에서 호출될 함수 
    /// </summary>
    public void SetIsSwitchWeapon()
    {
        isSwitchWeapon = !isSwitchWeapon;
    }
}
