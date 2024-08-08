using UnityEngine;

public class EndUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject      endUIOBJ;   // ���� ���� UI ������Ʈ

    /// <summary>
    /// ���� ���� UI ��Ȱ��ȭ
    /// </summary>
    private void Awake()
    {
        endUIOBJ.SetActive(false);
    }

    /// <summary>
    /// ���� ���� UI�� �����ִ� �Լ�
    /// </summary>
    public void ShowEndUI()
    {
        endUIOBJ.SetActive(true);    
    }

    /// <summary>
    /// ���� ���� UI�� ����� �Լ�
    /// </summary>
    public void HideEndUI()
    {
        endUIOBJ.SetActive(false);
    }

    /// <summary>
    /// ���� ���� �Լ�(���� ���� ��ư Ŭ�� �� ȣ��)
    /// </summary>
    public void EndGame()
    {
        Application.Quit();
    }
}
