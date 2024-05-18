using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskCompletionHandler : MonoBehaviour
{
    void Start()
    {
        
    }

    public IEnumerator OnTaskCompleted(GameObject uiElement, float displayTime)
    {
        // UI를 활성화
        uiElement.SetActive(true);
        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(displayTime);
        // UI 비활성화
        uiElement.SetActive(false);
    }
}
