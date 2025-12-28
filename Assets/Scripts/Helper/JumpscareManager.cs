using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Helper
{
    public class JumpscareManager : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject jumpscarePanel;
        public RectTransform faceImage;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip screamSound;

        [Header("Settings")]
        public float scareDuration = 2.0f;
        public float shakeAmount = 50f;

        public void TriggerJumpscare()
        {
            StartCoroutine(JumpscareRoutine());
        }

        IEnumerator JumpscareRoutine()
        {
            jumpscarePanel.SetActive(true);

            //audioSource.PlayOneShot(screamSound);

            float timer = 0;
            Vector2 originalPos = faceImage.anchoredPosition;

            while (timer < scareDuration)
            {
                Time.timeScale = 0f; 

                float x = Random.Range(-1f, 1f) * shakeAmount;
                float y = Random.Range(-1f, 1f) * shakeAmount;
                faceImage.anchoredPosition = originalPos + new Vector2(x, y);

                timer += Time.unscaledDeltaTime; 
                yield return null;
            }

            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}