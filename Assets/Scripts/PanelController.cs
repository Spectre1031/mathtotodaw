using UnityEngine;
using System.Collections; // Required for IEnumerator
using UnityEngine.SceneManagement; // Required for SceneManager

public class PanelController : MonoBehaviour
{
    public Animator panelAnimator;
    
    [Header("Scene Settings")]
    public string nextSceneName; // Assign the scene name here in the Inspector
    [SerializeField] private float fadeDuration = 1.0f; // Adjust this to match your animation duration

    /// <summary>
    /// This is the main entry function you will link to your OnDialogueEnd event
    /// </summary>
    public void FadeOutPanel()
    {
        Time.timeScale = 1f;
        // Safety check to ensure a scene name has been provided
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(FadeAndLoadRoutine());
        }
        else
        {
            Debug.LogError("Next Scene Name is missing on " + gameObject.name);
        }
    }

    private IEnumerator FadeAndLoadRoutine()
    {
        // 1. Fire your existing visibility parameter
        panelAnimator.SetBool("isVisible", true);

        // 2. Pause code execution here while the screen fades out
        yield return new WaitForSecondsRealtime(fadeDuration);

        // 3. Load the scene once the screen is fully covered/dark
        SceneManager.LoadScene(nextSceneName);
    }
}