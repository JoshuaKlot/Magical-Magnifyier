using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NextLevel : MonoBehaviour
{
    [SerializeField] private string NextScene;
    [SerializeField] private AudioSource levelFinish;
    [SerializeField] private Image FadeToBlack;
    [SerializeField] private float fadeDuration = 1.0f; // Time it takes to fade to black
    [SerializeField] private float delayBeforeLoading = 1.0f; // Time to wait after the jingle finishes

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(HandleLevelFinish());
        }
    }

    private IEnumerator HandleLevelFinish()
    {
        // Play the jingle
        if (levelFinish != null)
        {
            levelFinish.Play();
            yield return new WaitForSeconds(levelFinish.clip.length);
        }

        // Fade to black
        if (FadeToBlack != null)
        {
            float elapsedTime = 0.0f;
            Color fadeColor = FadeToBlack.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                FadeToBlack.color = fadeColor;
                yield return null;
            }
        }

        // Wait for a moment before loading the next scene
        yield return new WaitForSeconds(delayBeforeLoading);

        // Load the next scene
        SceneManager.LoadScene(NextScene);
    }
}
