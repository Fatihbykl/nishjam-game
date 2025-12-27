using System;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace Helper
{
    public class CameraFadeManager : MonoBehaviour
    {
        public static CameraFadeManager Instance;

        [Header("Ayarlar")]
        [SerializeField] private CanvasGroup fadeOverlay;
        [SerializeField] private float fadeDuration = 1.5f;
        [SerializeField] private float stayBlackDuration = 0.5f;
        [SerializeField] private CinemachineCamera[] allCameras;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        /// <summary>
        /// Hedef kameraya geçiş işlemini başlatır.
        /// </summary>
        /// <param name="targetCamera">Aktif olacak Cinemachine kamerası</param>
        public void TransitionToCamera(CinemachineCamera targetCamera, Action onCompleteFunc = null)
        {
            Sequence seq = DOTween.Sequence();

            fadeOverlay.blocksRaycasts = true;

            seq.Append(fadeOverlay.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad));

            seq.AppendCallback(() => 
            {
                ResetAllCameraPriorities();
            
                targetCamera.Priority = 100;
            });

            seq.AppendInterval(stayBlackDuration);

            seq.Append(fadeOverlay.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad));

            seq.OnComplete(() => 
            {
                fadeOverlay.blocksRaycasts = false;
                onCompleteFunc?.Invoke();
            });
        }

        // Sahnedeki diğer kameraların önceliğini sıfırlayan yardımcı metot
        private void ResetAllCameraPriorities()
        {
            foreach (var cam in allCameras)
            {
                cam.Priority = 0;
            }
        }
    }
}