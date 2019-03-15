using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Creates a lerped fade in/out, using the ColorGrading-options supplied by the PostProcessing Stack v2.
/// <para></para>
/// Assumes that the Camera has a <see cref="PostProcessLayer"/>-component attached, and that the specified PostProcessing Volume uses a PostProcessing Profile which contains a ColorGrading-effect.
/// </summary>
public class CameraFader : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume cameraFadePostProcessVolume;
    private ColorGrading colorGrading;
    private CancellationToken fadingStateToken;

    private void Start()
    {
        colorGrading = cameraFadePostProcessVolume.GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>();
        if (!colorGrading)
        {
            Debug.LogError("The CameraFadeVolume's 'PostProcessing Profile' doesn't contain a 'Color Grading' effect. \n" +
                "<color=red>The CameraFader will not work.</color>", cameraFadePostProcessVolume.sharedProfile);
        }
    }

    /// <summary>
    /// Makes the screen visible again after being black (after having called on <see cref="FadeOut(float)"/>)
    /// </summary>
    /// <param name="duration">The time for the screen to become completely visible again.</param>
    public void FadeIn(float duration = 1)
    {
        Fade(Color.white, duration);
    }

    /// <summary>
    /// Makes the screen become black (through affecting the ColorFilter of the "Color Grading" effect on a PostProcessing stack.)
    /// </summary>
    /// <param name="duration">The time for the screen to become completely black.</param>
    public void FadeOut(float duration = 1)
    {
        Fade(Color.black, duration);
    }

    private void Fade(Color color, float duration)
    {
        //Error check for required active components of the effect.
        if (!colorGrading.active || !colorGrading.enabled.value || !colorGrading.colorFilter.overrideState)
        {
            Debug.LogError("The 'Color Grading'-effect on the CameraFadeVolume's 'PostProcessing Profile' is either: \n" +
                "<b>'Inactive'</b>, <b>'Disabled'</b> (check the On/Off-button at the top-right of the effect) or the <b>'Color Filter'-setting is not overrideable</b>. \n" +
                "<color=red>The CameraFader will not work.</color>", cameraFadePostProcessVolume);
        }

        if (fadingStateToken != null)
        {
            fadingStateToken.Cancel();
        }

        fadingStateToken = new CancellationToken();
        StartCoroutine(LerpDat(colorGrading.colorFilter, color, duration, fadingStateToken));
    }

    //Source: https://answers.unity.com/questions/192438/coroutines-and-lerp-how-to-make-them-friends.html
    IEnumerator LerpDat(ColorParameter param, Color newColor, float time, CancellationToken cancellationToken)
    {
        cancellationToken?.InitToken();
        float elapsedTime = Time.deltaTime;
        Color startColor = param.value;
        while (elapsedTime < time && !cancellationToken.IsCanceled)
        {
            param.value = Color.Lerp(startColor, newColor, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Set value to exactly "0" or "1".
        if (!cancellationToken.IsCanceled)
        {
            param.value = newColor;
        }
        cancellationToken?.FinishToken();
    }
}

class CancellationToken
{
    private int numInit;
    private int numFinished;
    private bool cancel;

    public bool IsFinished => numFinished >= numInit && numInit > 0;
    public int FinishedTokens => numFinished;
    public bool IsCanceled => cancel;

    public void InitToken()
    {
        numInit++;
    }

    public void FinishToken()
    {
        numFinished++;
    }

    public void Cancel()
    {
        cancel = true;
    }
}
