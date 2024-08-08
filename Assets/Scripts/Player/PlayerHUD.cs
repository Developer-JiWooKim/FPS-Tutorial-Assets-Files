using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    private WeaponBase          weapon;             // 현재 정보가 출력되는 무기

    [Header("Components")]
    [SerializeField]
    private Status              status;             // 플레이어의 상태(이동속도, 체력)

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI     textWeaponName;     // 무기 이름
    [SerializeField]
    private Image               imageWeaponIcon;    // 무기 아이콘
    [SerializeField]
    private Sprite[]            spriteWeaponIcons;  // 무기 아이콘에 사용되는 sprite배열
    [SerializeField]
    private Vector2[]           sizeWeaponIcons;    // 무기 아이콘의 UI 크기 배열


    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI     textAmmo;           // 현재 / 최대 탄 수 출력 Text

    [Header("Magazine")]
    [SerializeField]
    private GameObject          magazineUIPrefab;   // 탄창 UI프리팹
    [SerializeField]
    private Transform           magazineParent;     // 탄창 UI가 배치되는 panel
    [SerializeField]
    private int                 maxMagazineCount;   // 처음 생성하는 최대 탄창 수

    private List<GameObject>    magazineList;       // 탄창 UI 리스트

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI     textHP;             // 플레이어 체력을 나타내는 Text 
    [SerializeField]
    private Image               imageBloodScreen;   // 플레이어가 공격 받았을 때 화면에 표시되는 Image
    
    [SerializeField]
    private AnimationCurve      curveBloodScreen;   // BloodScreen 애니메이션 재생 속도curve

    [Header("UI Objects")]
    [SerializeField]
    private GameObject          PanelWeapon;        // 무기 UI가 배치되는 Panel
    [SerializeField]
    private GameObject          ImageAim;           // Aim 이미지를 가진 오브젝트
    [SerializeField]
    private GameObject          TextHP;             // HP Text를 가진 오브젝트

    /// <summary>
    /// 메소드가 등록되어 있는 이벤트 클래스(weapon.xx)의 Invoke()메소드가 호출될 때 등록된 메소가(매개변수)가 실행
    /// </summary>
    private void Awake()
    {
        // status(Status클래스)의 onHPEvent() 이벤트에 UpdateHPHUD() 함수 등록
        status.onHPEvent.AddListener(UpdateHPHUD); 

        WeaponUIOnOff(); // 처음 UI를 비활성화
    }

    /// <summary>
    /// 무기와 관련된 UI 초기화
    /// </summary>
    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
        imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcons[(int)weapon.WeaponName];
    }

    /// <summary>
    /// 탄창과 관련된 UI 초기화
    /// </summary>
    private void SetupMagazine()
    {
        // weapon에 등록되어 있는 최대 탄창 개수만큼 Image Icon을 생성
        // magazineParent오브젝트의 자식으로 등혹 후 모두 비활성화 / 리스트에 저장
        magazineList = new List<GameObject>();

        // 최대 탄창 개수 만큼 UI 를 생성 후 부모(panel에 붙임)
        for (int i = 0; i < maxMagazineCount; i++)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);

            // 리스트에 추가
            magazineList.Add(clone);
        }
    }

    /// <summary>
    /// 총알 UI를 업데이트하는 함수
    /// </summary>
    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }

    /// <summary>
    /// 탄창 UI를 업데이트하는 함수
    /// </summary>
    private void UpdateMagazineHUD(int currentMagazine)
    {
        // 전부 비활성화
        for (int i = 0; i < magazineList.Count; i++)
        {
            magazineList[i].SetActive(false);
        }

        // 현재 탄창 개수 만큼 UI활성화
        for (int i = 0; i < currentMagazine; i++)
        {
            magazineList[i].SetActive(true);
        }
    }

    /// <summary>
    /// HP UI를 업데이트하는 함수
    /// </summary>
    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP : " + current;

        // 체력이 증가했을 때는 화면에 빨간색 이미지를 출력하지 않도록 return
        if (previous <= current)
        {
            return;
        }

        // 데미지를 입어 체력이 깎이면 OnBloodScreen 코루틴을 실행
        if (previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    /// <summary>
    /// 체력이 깎일 때 실행될 코루틴 함수
    /// </summary>
    private IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));

            imageBloodScreen.color = color;

            yield return null;
        }
    }

    /// <summary>
    /// 모든 무기에 대한 초기화(탄창, 총알 UI)
    /// </summary>
    public void SetupAllWeapons(WeaponBase[] weapons)
    {
        SetupMagazine();

        // 사용 가능한 모든 무기의 이벤트 등록
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }

    /// <summary>
    /// 무기와 관련된 UI를 활성화 / 비활성화 해주는 함수
    /// </summary>
    public void WeaponUIOnOff()
    {
        if (PanelWeapon.activeSelf == true)
        {
            PanelWeapon.SetActive(false);
            ImageAim.SetActive(false);
            TextHP.SetActive(false);
        }
        else
        {
            PanelWeapon.SetActive(true);
            ImageAim.SetActive(true);
            TextHP.SetActive(true);
        }
        
    }

    /// <summary>
    /// 무기 변경 시 변경된 무기와 관련된 UI로 업데이트하는 함수
    /// </summary>
    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;

        SetupWeapon();
    }
}
