using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader instance;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.5f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    async Task Fade(float targetTransparency)
    {
        float start = canvasGroup.alpha, t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetTransparency, t / fadeDuration);
            await Task.Yield();
        }
        canvasGroup.alpha = targetTransparency;
    }

    public async Task FadeOut()
    {
        canvasGroup.blocksRaycasts = true; // 터치/클릭 방지
        await Fade(1); // Fade to black
    }

    public async Task FadeIn()
    {
        await Fade(0); // Fade to transparent
        canvasGroup.blocksRaycasts = false; // 터치 해제
    }

    // --- 기존 커맨드 (잠잘 때: 어두워졌다가 다시 밝아짐) ---
    [YarnCommand("fade_screen_sleep")]
    public static IEnumerator FadeScreenSleepCommand()
    {
        if (instance == null) yield break;

        Task fadeOutTask = instance.FadeOut();
        while (!fadeOutTask.IsCompleted) yield return null;

        yield return new WaitForSeconds(1.0f);

        Task fadeInTask = instance.FadeIn();
        while (!fadeInTask.IsCompleted) yield return null;
    }

    // --- 신규 커맨드 (엔딩용: 계속 검은 화면 유지!) ---
    [YarnCommand("fade_out_permanent")]
    public static IEnumerator FadeOutPermanentCommand()
    {
        if (instance == null) yield break;

        Task fadeOutTask = instance.FadeOut();
        while (!fadeOutTask.IsCompleted) yield return null;
    }
}