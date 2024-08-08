using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider      slider;         // �����̴�(ü�¹�)

    private Camera      mainCamera;     // ���� ī�޶�

    /// <summary>
    /// ���� �ʱ�ȭ
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
    /// ü�¹� �ʱ�ȭ�ϴ� �Լ�
    /// </summary>
    public void SetupHPBar()
    {
        slider.value = 1;
    }

    /// <summary>
    /// ���� ü�¿� ���� ü�¹� ������Ʈ�ϴ� �Լ�
    /// </summary>
    public void UpdateHPBar(float maxHP, float currentHP)
    {
        slider.value = currentHP / maxHP;
    }
}
