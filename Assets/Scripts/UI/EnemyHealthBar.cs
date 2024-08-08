using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider      slider;         // 슬라이더(체력바)

    private Camera      mainCamera;     // 메인 카메라

    /// <summary>
    /// 변수 초기화
    /// </summary>
    private void Awake()
    {
        mainCamera = Camera.main;
        slider.value = 1;
    }
    private void Update()
    {
        transform.rotation = mainCamera.transform.rotation;
    }

    /// <summary>
    /// 체력바 초기화하는 함수
    /// </summary>
    public void SetupHPBar()
    {
        slider.value = 1;
    }

    /// <summary>
    /// 현재 체력에 따라 체력바 업데이트하는 함수
    /// </summary>
    public void UpdateHPBar(float maxHP, float currentHP)
    {
        slider.value = currentHP / maxHP;
    }
}
