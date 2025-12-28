using System.Collections;
using DG.Tweening;
using Player.States;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace Helper
{
    public class LevelEndSequence : MonoBehaviour
    {
        [Header("Cutscene Objects")]
        public GameObject enemy;
        public CinemachineCamera endCam;
        public Transform standPoint;
        public CanvasGroup blackScreenPanel;
        public Material bodyDissolveMaterial;
        public Animator boxAnimator;

        [Header("Settings")]
        public float dissolveSpeed = 2.0f;
        public float fadeSpeed = 1.5f;

        private void OnTriggerEnter(Collider other)
        {
            // Sadece oyuncu girdiğinde ve sadece bir kere çalışsın
            if (other.CompareTag("Player"))
            {
                // PlayerStateManager'ı bul
                PlayerStateManager player = other.GetComponent<PlayerStateManager>();
                if (player != null)
                {
                    StartCoroutine(PlayEndCutscene(player));
                }
            }
        }

        IEnumerator PlayEndCutscene(PlayerStateManager player)
        {
            enemy.SetActive(false);
            player.enabled = false; 
        
            player._controller.enabled = false; 
            
            player._animator.SetBool("Levitating", true);
            
            player.transform.position = standPoint.position;
            player.transform.rotation = standPoint.rotation;
            
            endCam.Priority = 100;

            yield return new WaitForSeconds(1.0f);
        
            Renderer headRend = player.headMesh.GetComponent<Renderer>();
            Renderer armsRend = player.armsMesh.GetComponent<Renderer>();
            Renderer legsRend = player.legsMesh.GetComponent<Renderer>();
            
            headRend.material = bodyDissolveMaterial;
            armsRend.material = bodyDissolveMaterial;
            legsRend.material = bodyDissolveMaterial;

            player.headMesh.SetActive(true);
            player.armsMesh.SetActive(true);
            player.legsMesh.SetActive(true);
            
            headRend.material.SetFloat("_Cutoff", 1f);
            armsRend.material.SetFloat("_Cutoff", 1f);
            legsRend.material.SetFloat("_Cutoff", 1f);

            headRend.DODissolveIn(dissolveSpeed);
            armsRend.DODissolveIn(dissolveSpeed);
            legsRend.DODissolveIn(dissolveSpeed);

            yield return new WaitForSeconds(dissolveSpeed + 0.5f);
            
            boxAnimator.SetTrigger("Close");
            
            yield return new WaitForSeconds(2f);

            blackScreenPanel.DOFade(1f, 1f).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(1f);

            Debug.Log("Level Bitti!");
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}