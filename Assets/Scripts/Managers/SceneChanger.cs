using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    FadeInOut fade;

    void Start()
    {
        fade = FindAnyObjectByType<FadeInOut>();
        // StartCoroutine(fade.FadeIn());
    }

    public void GoStartScene()
    {
        StartCoroutine(ChangeScene("Scenes/VR Starter Scene"));
    }

    public void GoAuthScene()
    {
        StartCoroutine(ChangeScene("Scenes/AuthScene"));
    }

    public void GoProfileScene()
    {
        StartCoroutine(ChangeScene("Scenes/ProfileScene"));
    }

    public void GoMainScene()
    {
        Debug.Log("메인 씬으로 이동");
        StartCoroutine(ChangeScene("Scenes/MainScene"));
    }

    public void GoARScene()
    {
        Debug.Log("AR 씬으로 이동");
        StartCoroutine(ChangeScene("Scenes/ARScene"));
    }

    // 씬 전환 함수
    public IEnumerator ChangeScene(string sceneName)
    {
        fade.FadeIn();
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);

    }

}
