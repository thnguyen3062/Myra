using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_FadeCanvasGroup : MonoBehaviour
{
    public SC_Button_Upgrade UpgradeButton;
    public SC_Bar_Nonstop BonusAnim;
    private int fakeInt = 0;
    [SerializeField] float fadeDuration;
    private CanvasGroup thisCanvasGroup;
    // Start is called before the first frame update
    public bool visible = false;
    void Start()
    {
        thisCanvasGroup = GetComponent<CanvasGroup>();

        if (thisCanvasGroup == null)
        {
            // If CanvasGroup doesn't exist, add it.
            thisCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }        

        thisCanvasGroup.alpha = 0;

        UpgradeButton.OnUpgradeButtonClick += () => FadeInCanvas(fakeInt);


        if (BonusAnim != null)
        {
            BonusAnim.BonusFlyDone += (fakeInt) => FadeOutCanvas(fakeInt);
        }
    }

    void FadeInCanvas(int aFakeInt)
    {
        visible = false;
        StartCoroutine(FadeCanvasGroup(visible, fadeDuration));
    }

    void FadeOutCanvas(int aFakeInt)
    {
        visible = true;
        StartCoroutine(FadeCanvasGroup(visible, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(bool fadeIn, float duration)
    {
        float targetAlpha = fadeIn ? 0f : 1f;
        float startAlpha = thisCanvasGroup.alpha;
        float elapsedTime = 0f;

        yield return new WaitForSeconds(1f);

        while (elapsedTime < duration)
        {
            thisCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeIn = !fadeIn;
        visible = !visible;

        thisCanvasGroup.alpha = targetAlpha; // Ensure the final alpha value is set
    }    


    void OnDestroy()
    {
        UpgradeButton.OnUpgradeButtonClick -= () => FadeInCanvas(fakeInt);
        BonusAnim.BonusFlyDone -= (fakeInt) => FadeOutCanvas(fakeInt);
    }
}
