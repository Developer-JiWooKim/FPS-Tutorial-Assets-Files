using UnityEngine;

public class EndUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject      endUIOBJ;   // 게임 종료 UI 오브젝트

    /// <summary>
    /// 게임 종료 UI 비활성화
    /// </summary>
    private void Awake()
    {
        endUIOBJ.SetActive(false);
    }

    /// <summary>
    /// 게임 종료 UI를 보여주는 함수
    /// </summary>
    public void ShowEndUI()
    {
        endUIOBJ.SetActive(true);    
    }

    /// <summary>
    /// 게임 종료 UI를 숨기는 함수
    /// </summary>
    public void HideEndUI()
    {
        endUIOBJ.SetActive(false);
    }

    /// <summary>
    /// 게임 종료 함수(게임 종료 버튼 클릭 시 호출)
    /// </summary>
    public void EndGame()
    {
        Application.Quit();
    }
}
