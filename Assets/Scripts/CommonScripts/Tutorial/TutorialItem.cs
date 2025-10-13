using System;
using UnityEngine;
using DG.Tweening;

public class TutorialItem : MonoBehaviour
{
    [Header("Tiklanmasi gereken obje")]
    public GameObject targetObject;

    [Header("Ne kadar gecikme ile gorsun?")]
    public float delay = 1f;

    [Header("Fade ayarlari")]
    private float fadeInDuration = 0.4f;
    private float fadeOutDuration = 0.3f;

    private CanvasGroup canvasGroup;
    private bool isActive = false;
    private Action onClickedCallback;

    private bool isSkippedThisSession = false;
    public bool IsSkipped => isSkippedThisSession;
    public bool IsActive => isActive;




    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void SkipThisSession()
    {
        isSkippedThisSession = true;
    }

    public void Show(Action onClicked)
    {
        if (isSkippedThisSession) return;

        onClickedCallback = onClicked;
        isActive = true;
        gameObject.SetActive(true);

        canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad);

        if (targetObject != null)
        {
            TutorialClickReceiver receiver = targetObject.GetComponent<TutorialClickReceiver>();
            if (receiver == null)
                receiver = targetObject.AddComponent<TutorialClickReceiver>();

            receiver.Assign(() =>
            {
                if (!isActive)
                {
                    SkipThisSession(); // erken tiklandi: bu sahnede bir daha acma
                }
                else
                {
                    OnTargetClicked(); // normal tutorial tamamlandi
                }
            });
        }
    }

    private void OnTargetClicked()
    {
        if (!isActive) return;

        isActive = false;
        canvasGroup.DOFade(0f, fadeOutDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onClickedCallback?.Invoke();
            });
    }

    public void ResetItem()
    {
        if (canvasGroup != null)
        {
            DOTween.Kill(canvasGroup);
            canvasGroup.alpha = 0f;
        }

        isActive = false;
        gameObject.SetActive(false);
        isSkippedThisSession = false;
    }
}
