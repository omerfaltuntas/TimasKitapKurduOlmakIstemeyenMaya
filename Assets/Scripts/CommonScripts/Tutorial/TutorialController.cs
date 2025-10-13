using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    private List<TutorialItem> tutorialItems = new List<TutorialItem>();
    private int currentIndex = 0;
    private Coroutine tutorialRoutine;

    private void OnEnable()
    {
        StartCoroutine(DelayedInit());
    }

    private void OnDisable()
    {
        if (tutorialRoutine != null)
            StopCoroutine(tutorialRoutine);

        foreach (var item in tutorialItems)
        {
            item.ResetItem();
        }

        currentIndex = 0;
    }

    IEnumerator DelayedInit()
    {
        yield return null; // Sayfa objeleri aktif olsun

        tutorialItems.Clear();
        GetComponentsInChildren(true, tutorialItems);

        foreach (var item in tutorialItems)
        {
            item.ResetItem();

            // Hedef objeye global tıklama dinleyicisi
            if (item.targetObject != null)
            {
                var receiver = item.targetObject.GetComponent<TutorialClickReceiver>();
                if (receiver == null)
                    receiver = item.targetObject.AddComponent<TutorialClickReceiver>();

                var captured = item;
                receiver.Assign(() =>
                {
                    // Eğer tutorial şu anda aktif değilse = erken tıklanmıştır
                    if (!captured.IsActive)
                    {
                        captured.SkipThisSession();
                        // Debug.Log("Erken tiklandi, skip: " + captured.name);
                    }
                });
            }
        }

        currentIndex = 0;
        tutorialRoutine = StartCoroutine(PlayTutorialsSequentially());
    }

    IEnumerator PlayTutorialsSequentially()
    {
        while (currentIndex < tutorialItems.Count)
        {
            var current = tutorialItems[currentIndex];


            yield return new WaitForSeconds(current.delay);

            if (current.IsSkipped)
            {
                currentIndex++;
                continue;
            }
            current.Show(OnTutorialItemClicked);
            yield break;
        }
    }

    private void OnTutorialItemClicked()
    {
        currentIndex++;
        if (currentIndex < tutorialItems.Count)
        {
            tutorialRoutine = StartCoroutine(PlayTutorialsSequentially());
        }
    }
}
