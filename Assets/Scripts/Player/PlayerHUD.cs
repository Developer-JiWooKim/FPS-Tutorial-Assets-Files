using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    private WeaponBase          weapon;             // ���� ������ ��µǴ� ����

    [Header("Components")]
    [SerializeField]
    private Status              status;             // �÷��̾��� ����(�̵��ӵ�, ü��)

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI     textWeaponName;     // ���� �̸�
    [SerializeField]
    private Image               imageWeaponIcon;    // ���� ������
    [SerializeField]
    private Sprite[]            spriteWeaponIcons;  // ���� �����ܿ� ���Ǵ� sprite�迭
    [SerializeField]
    private Vector2[]           sizeWeaponIcons;    // ���� �������� UI ũ�� �迭


    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI     textAmmo;           // ���� / �ִ� ź �� ��� Text

    [Header("Magazine")]
    [SerializeField]
    private GameObject          magazineUIPrefab;   // źâ UI������
    [SerializeField]
    private Transform           magazineParent;     // źâ UI�� ��ġ�Ǵ� panel
    [SerializeField]
    private int                 maxMagazineCount;   // ó�� �����ϴ� �ִ� źâ ��

    private List<GameObject>    magazineList;       // źâ UI ����Ʈ

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI     textHP;             // �÷��̾� ü���� ��Ÿ���� Text 
    [SerializeField]
    private Image               imageBloodScreen;   // �÷��̾ ���� �޾��� �� ȭ�鿡 ǥ�õǴ� Image
    
    [SerializeField]
    private AnimationCurve      curveBloodScreen;   // BloodScreen �ִϸ��̼� ��� �ӵ�curve

    [Header("UI Objects")]
    [SerializeField]
    private GameObject          PanelWeapon;        // ���� UI�� ��ġ�Ǵ� Panel
    [SerializeField]
    private GameObject          ImageAim;           // Aim �̹����� ���� ������Ʈ
    [SerializeField]
    private GameObject          TextHP;             // HP Text�� ���� ������Ʈ

    /// <summary>
    /// �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ����(weapon.xx)�� Invoke()�޼ҵ尡 ȣ��� �� ��ϵ� �޼Ұ�(�Ű�����)�� ����
    /// </summary>
    private void Awake()
    {
        // status(StatusŬ����)�� onHPEvent() �̺�Ʈ�� UpdateHPHUD() �Լ� ���
        status.onHPEvent.AddListener(UpdateHPHUD); 

        WeaponUIOnOff(); // ó�� UI�� ��Ȱ��ȭ
    }

    /// <summary>
    /// ����� ���õ� UI �ʱ�ȭ
    /// </summary>
    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
        imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcons[(int)weapon.WeaponName];
    }

    /// <summary>
    /// źâ�� ���õ� UI �ʱ�ȭ
    /// </summary>
    private void SetupMagazine()
    {
        // weapon�� ��ϵǾ� �ִ� �ִ� źâ ������ŭ Image Icon�� ����
        // magazineParent������Ʈ�� �ڽ����� ��Ȥ �� ��� ��Ȱ��ȭ / ����Ʈ�� ����
        magazineList = new List<GameObject>();

        // �ִ� źâ ���� ��ŭ UI �� ���� �� �θ�(panel�� ����)
        for (int i = 0; i < maxMagazineCount; i++)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);

            // ����Ʈ�� �߰�
            magazineList.Add(clone);
        }
    }

    /// <summary>
    /// �Ѿ� UI�� ������Ʈ�ϴ� �Լ�
    /// </summary>
    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }

    /// <summary>
    /// źâ UI�� ������Ʈ�ϴ� �Լ�
    /// </summary>
    private void UpdateMagazineHUD(int currentMagazine)
    {
        // ���� ��Ȱ��ȭ
        for (int i = 0; i < magazineList.Count; i++)
        {
            magazineList[i].SetActive(false);
        }

        // ���� źâ ���� ��ŭ UIȰ��ȭ
        for (int i = 0; i < currentMagazine; i++)
        {
            magazineList[i].SetActive(true);
        }
    }

    /// <summary>
    /// HP UI�� ������Ʈ�ϴ� �Լ�
    /// </summary>
    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP : " + current;

        // ü���� �������� ���� ȭ�鿡 ������ �̹����� ������� �ʵ��� return
        if (previous <= current)
        {
            return;
        }

        // �������� �Ծ� ü���� ���̸� OnBloodScreen �ڷ�ƾ�� ����
        if (previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    /// <summary>
    /// ü���� ���� �� ����� �ڷ�ƾ �Լ�
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
    /// ��� ���⿡ ���� �ʱ�ȭ(źâ, �Ѿ� UI)
    /// </summary>
    public void SetupAllWeapons(WeaponBase[] weapons)
    {
        SetupMagazine();

        // ��� ������ ��� ������ �̺�Ʈ ���
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }

    /// <summary>
    /// ����� ���õ� UI�� Ȱ��ȭ / ��Ȱ��ȭ ���ִ� �Լ�
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
    /// ���� ���� �� ����� ����� ���õ� UI�� ������Ʈ�ϴ� �Լ�
    /// </summary>
    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;

        SetupWeapon();
    }
}
