using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가이드 텍스트를 가지는 클래스들이 상속받는 추상 클래스
/// </summary>
public abstract class GuideTextBase : MonoBehaviour
{
    /// <summary>
    /// 가이드 텍스트를 가지는 클래스들이 오버로드할 추상 메소드들
    /// </summary>
    public abstract void InputText();
    public abstract List<string> GetList();
}
