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
    private Image               guideBackGroundUI;      // �濡 �� �� ������ �ؽ�Ʈ���� ���ȭ��
    [SerializeField]
    private TextMeshProUGUI     guideTextUI;            // �濡 �� �� ������ �ؽ�Ʈ

    [SerializeField]
    private GameObject[]        guideObjects;           // ���̵� �ؽ�Ʈ�� ���� ������Ʈ��

    private const float         fadeTime = 1.5f;
    private const float         start = 0f;
    private const float         end = 1;
    private const float         showTime = 5f;

    private List<string>        currentGuideList;

    private bool                isGuideOn;              // �濡 ���� ���� ���̵� �̺�Ʈ�� �߻��ǰ�(�濡�� �������� �ȳ�Ÿ����)

    /// <summary>
    /// ���̵� UI �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        InitGuideUI();
    }

    /// <summary>
    /// ���̵� UI �ʱ�ȭ�ϴ� �Լ�
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
    /// UI����(On/Off)�� �ٲٴ� �Լ� 
    /// </summary>
    public void StateChange() 
    {
        isGuideOn = !isGuideOn;
    }


    /// <summary>
    /// �濡 �����ϰų� ���� �� ȣ��� �Լ�(���̵� UI�� Ű�ų� ��)
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
    /// ���̵� UI�� ��Ȱ��ȭ �ϴ� �Լ�
    /// </summary>
    public void GuideOff()
    {
        // ���� ���� �ڷ�ƾ ����(���̵� ȿ��)
        StopCoroutine("Fade_GuideText");

        // Guide Background UI Off
        Color color = guideBackGroundUI.color;
        color.a = 0;
        guideBackGroundUI.color = color;

        guideTextUI.text = ""; // Text Clear
        
    }

    /// <summary>
    /// ���̵� UI�� Ȱ��ȭ �ϴ� �Լ�
    /// </summary>
    /// <param name="roomName"></param>
    public void GuideOn(E_RoomName roomName)
    {
        // ���̵� ��� ���̰�
        Color color = guideBackGroundUI.color;
        color.a = 1;
        guideBackGroundUI.color = color;

        // ����Ʈ�� ���� �ֱ� ���̵� ����Ʈ�� ����
        currentGuideList = guideObjects[(int)roomName].GetComponent<GuideTextBase>().GetList();

        float delayTime = currentGuideList.Count * showTime + 3f;

        // ���̵� ȿ���� �Բ� �ؽ�Ʈ���� ���
        StartCoroutine(Fade_GuideText(currentGuideList, showTime));
        // delayTime �ڿ� ���̵� UI�� �����Ŵ
        StartCoroutine(EndUI(delayTime));

    }

    /// <summary>
    /// n�� �� UI�� �����Ű�� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator EndUI(float _delayTime)
    {
        yield return new WaitForSeconds(_delayTime);
        GuideOff();
    }

    /// <summary>
    /// �ؽ�Ʈ�� ���̵� ȿ���� �ִ� �ڷ�ƾ �Լ�
    /// </summary>
    private IEnumerator Fade_GuideText(List<string> _guideList, float _showTime)
    {
        // ���̵� ����Ʈ ���� ��ŭ ����
        for (int i = 0; i < _guideList.Count; i++)
        {
            
            guideTextUI.text = _guideList[i]; // UI�� �ؽ�Ʈ�� ����

            float current = 0f;
            float percent = 0f;

            // 1�ʵ��� ���ڰ� õõ�� ���̵� �ƿ�
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



