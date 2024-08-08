using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum E_RoomName
{
    Start = 0,
    ChangeWeapon,
    Enemy,
    Object,
    MemoryPool,
}

public class GuideController : MonoBehaviour
{
    [SerializeField]
    private Image               guideBackGroundUI;      // 방에 들어갈 때 보여질 텍스트들의 배경화면
    [SerializeField]
    private TextMeshProUGUI     guideTextUI;            // 방에 들어갈 때 보여질 텍스트

    [SerializeField]
    private GameObject[]        guideObjects;           // 가이드 텍스트를 가진 오브젝트들

    private const float         fadeTime = 1.5f;
    private const float         start = 0f;
    private const float         end = 1;
    private const float         showTime = 5f;

    private List<string>        currentGuideList;

    private bool                isGuideOn;              // 방에 들어갔을 때만 가이드 이벤트가 발생되게(방에서 나갈때는 안나타나게)

    /// <summary>
    /// 가이드 UI 초기화
    /// </summary>
    private void Awake()
    {
        InitGuideUI();
    }

    /// <summary>
    /// 가이드 UI 초기화하는 함수
    /// </summary>
    private void InitGuideUI()
    {
        isGuideOn = false;
        Color color = guideBackGroundUI.color;
        color.a = 0;
        guideBackGroundUI.color = color;
        guideTextUI.text = "";
        currentGuideList = new List<string>();
    }

    /// <summary>
    /// UI상태(On/Off)를 바꾸는 함수 
    /// </summary>
    public void StateChange() 
    {
        isGuideOn = !isGuideOn;
    }


    /// <summary>
    /// 방에 입장하거나 나올 때 호출될 함수(가이드 UI를 키거나 끔)
    /// </summary>
    public void GuideEvent(E_RoomName _roomName)
    {
        StateChange();

        if (isGuideOn == true)
        {
            GuideOn(_roomName);
        }
        else
        {
            GuideOff();
            StopAllCoroutines();
        }
    }

    /// <summary>
    /// 가이드 UI를 비활성화 하는 함수
    /// </summary>
    public void GuideOff()
    {
        // 실행 중인 코루틴 종료(페이드 효과)
        StopCoroutine("Fade_GuideText");

        // Guide Background UI Off
        Color color = guideBackGroundUI.color;
        color.a = 0;
        guideBackGroundUI.color = color;

        guideTextUI.text = ""; // Text Clear
        
    }

    /// <summary>
    /// 가이드 UI를 활성화 하는 함수
    /// </summary>
    /// <param name="roomName"></param>
    public void GuideOn(E_RoomName roomName)
    {
        // 가이드 배경 보이게
        Color color = guideBackGroundUI.color;
        color.a = 1;
        guideBackGroundUI.color = color;

        // 리스트를 얻어와 최근 가이드 리스트에 넣음
        currentGuideList = guideObjects[(int)roomName].GetComponent<GuideTextBase>().GetList();

        float delayTime = currentGuideList.Count * showTime + 3f;

        // 페이드 효과와 함께 텍스트들을 출력
        StartCoroutine(Fade_GuideText(currentGuideList, showTime));
        // delayTime 뒤에 가이드 UI를 종료시킴
        StartCoroutine(EndUI(delayTime));

    }

    /// <summary>
    /// n초 후 UI를 종료시키는 코루틴 함수
    /// </summary>
    private IEnumerator EndUI(float _delayTime)
    {
        yield return new WaitForSeconds(_delayTime);
        GuideOff();
    }

    /// <summary>
    /// 텍스트에 페이드 효과를 주는 코루틴 함수
    /// </summary>
    private IEnumerator Fade_GuideText(List<string> _guideList, float _showTime)
    {
        // 가이드 리스트 길이 만큼 실행
        for (int i = 0; i < _guideList.Count; i++)
        {
            
            guideTextUI.text = _guideList[i]; // UI에 텍스트를 넣음

            float current = 0f;
            float percent = 0f;

            // 1초동안 글자가 천천히 페이드 아웃
            while (percent < 1)
            {
                current += Time.deltaTime;
                percent = current / fadeTime;

                Color color = guideTextUI.color;
                color.a = Mathf.Lerp(start, end, percent);
                guideTextUI.color = color;

                yield return null;
            }
            yield return new WaitForSeconds(_showTime);
        }
    }
}



