using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private GameObject          message;                // 메세지 오브젝트
    [SerializeField]
    private TextMeshProUGUI     messageText;            // 메세지 텍스트

    private bool                isMessageUse = false;   // 메세지가 띄워지고 있는지 검사

    private float               outMessagePos = 450f;   // 메세지가 바깥에 있을 때 위치
    private float               inMessagePos = 0f;      // 메세지가 안쪽에 있을 때 위치


    private Queue<string>       messages;               // 메세지들을 담을 큐

    /// <summary>
    /// 메세지 큐를 생성
    /// </summary>
    private void Start()
    {
        messages = new Queue<string>();
    }

    private void Update()
    {
        UpdateMessage();
    }

    /// <summary>
    /// 메세지를 띄울지 말지 계속 검사하고, 메세지를 띄워야 되면 OnMessageEvent코루틴 함수 실행
    /// </summary>
    private void UpdateMessage()
    {
        // 메세지가 사용중(다른 메세지를 띄우고 있으면) 리턴
        if (isMessageUse == true)
        {
            return;
        }

        // 큐에 메세지가 없으면 리턴
        if (messages.Count == 0)
        {
            return;
        }

        if (messages.Count > 0)
        {
            messageText.text = messages.Dequeue();
        }
        StartCoroutine("OnMessageEvent");
    }

    /// <summary>
    /// 메세지가 나오도록 하는 코루틴 함수
    /// </summary>
    private IEnumerator OnMessageEvent()
    {
        yield return StartCoroutine("MessageMoveLeft");
        yield return new WaitForSeconds(3.5f);
        yield return StartCoroutine("MessageMoveRight");
    }

    /// <summary>
    /// 화면 밖에 있던 메세지 오브젝트를 화면에 나오게(왼쪽으로 이동)하는 코루틴 함수
    /// </summary>
    private IEnumerator MessageMoveLeft()
    {
        float current = 0f;
        float percent = 0f;
        float time = 0.5f;

        isMessageUse = true;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            message.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(outMessagePos, inMessagePos, percent), 0);
            yield return null;
        }
    }

    /// <summary>
    /// 메세지 오브젝트를 다시 화면 밖으로(오른쪽으로 이동) 이동시키는 코루틴 함수
    /// </summary>
    private IEnumerator MessageMoveRight() // 오른쪽으로 이동
    {
        float current = 0f;
        float percent = 0f;
        float time = 0.5f;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / time;

            message.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(inMessagePos, outMessagePos, percent), 0);
            yield return null;
        }
        isMessageUse = false;
    }

    /// <summary>
    /// 큐에 메세지를 저장하는 함수
    /// </summary>
    public void MessageSave(string _message)
    {
        messages.Enqueue(_message);
    }
}