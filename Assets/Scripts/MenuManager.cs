using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Main Menu UI")]
    public GameObject mainMenuPanel;
    public GameObject aboutGamePanel;

    [Header("Transition Settings")]
    public CanvasGroup transitionFade;
    public float transitionTime = 1f;

    [Header("Audio Settings")]
    public AudioSource sfxSource;
    public AudioClip clickSound;

    // Call this function whenever ANY button is clicked
    public void PlayButtonSound()
    {
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }

    // ==========================================
    //        MAIN MENU SCENE FUNCTIONS
    // ==========================================

    public void OpenNarration()
    {
        // Start the transition sequence instead of loading instantly
        StartCoroutine(FadeAndLoad("Narration"));
    }

    public void OpenLevelSelectionScene()
    {
        StartCoroutine(FadeAndLoad("LevelSelection"));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // 1. Play the click sound
        PlayButtonSound();

        // 2. Turn on the black screen and fade it in smoothly
        if (transitionFade != null)
        {
            transitionFade.gameObject.SetActive(true);
            float timer = 0f;

            while (timer < transitionTime)
            {
                timer += Time.deltaTime;
                // Mathf.Lerp smoothly blends the alpha from 0 (clear) to 1 (solid black)
                transitionFade.alpha = Mathf.Lerp(0f, 1f, timer / transitionTime);
                yield return null; // Wait for the next frame
            }
        }
        else
        {
            // Fallback: If no fade screen is assigned, just wait for the sound to finish
            yield return new WaitForSeconds(clickSound != null ? clickSound.length : 0.5f);
        }

        // 3. Finally, load the new scene!
        SceneManager.LoadScene(sceneName);
    }

    public void OpenAboutPage()
    {
        if (aboutGamePanel != null)
        {
            aboutGamePanel.SetActive(true);
        }

        // Hide the main menu so it doesn't overlap
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
    }

    public void CloseAboutPage()
    {
        if (aboutGamePanel != null)
        {
            aboutGamePanel.SetActive(false);
        }

        // Bring the main menu back when we close the about page
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    // ==========================================
    //              Exiting the Game
    // ==========================================

    public void QuitGame()
    {
        Debug.Log("[SYSTEM] Exiting the game...");

        // This closes the game if you are playing it inside the Unity Editor
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;

                // This closes the game in the final compiled build (.exe / .apk)
        #else
                    Application.Quit();
        #endif
    }

    // ==========================================
    //      LEVEL SELECTION SCENE FUNCTIONS
    // ==========================================

    public void GoBackToMainMenu()
    {
        // Loads the Main Menu scene when Back is clicked
        Time.timeScale = 1f; // Ensure time is normal when going back to menu
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("L1_Objective");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("L2_Objective");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("L3_Objective");
    }
}