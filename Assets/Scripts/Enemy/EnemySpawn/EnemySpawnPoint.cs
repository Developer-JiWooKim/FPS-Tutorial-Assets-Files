using System.Collections;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]
    private float           fadeSpeed = 4;
    
    private MeshRenderer    meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();   
    }
    /// <summary>
    /// 활성화 될 때 Fade효과를 적용
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine("OnFadeEffect");
    }

    /// <summary>
    /// 비활성화 될 때 Fade효과를 멈춤
    /// </summary>
    private void OnDisable()
    {
        StopCoroutine("OnFadeEffect");
    }

    /// <summary>
    /// 적들이 생성될 때 생성될 위치에 표시될 meshRenderer에 페이드 효과 적용하는 코루틴
    /// </summary>
    private IEnumerator OnFadeEffect()
    {
        while (true)
        {
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSpeed, 1));

            meshRenderer.material.color = color;

            yield return null;
        }
    }
}
