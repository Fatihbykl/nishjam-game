using DG.Tweening;
using UnityEngine;

// DOTween kullandığın için animasyonu onunla yapıyoruz

namespace LevelProps
{
    public class DoorController : MonoBehaviour
    {
        [Header("References")]
        public ShootableSwitch[] requiredSwitches;
    
        private bool isOpen = false;

        public AudioSource effect;

        public void CheckSwitches()
        {
            if (isOpen) return;

            foreach (var sw in requiredSwitches)
            {
                if (!sw.isActivated) return;
            }

            OpenDoor();
        }

        void OpenDoor()
        {
            Debug.Log("Tüm şalterler açık! Kapı açılıyor.");
            isOpen = true;

            GetComponent<Animator>().SetTrigger("Open");

            effect.Play();

            // Kapı açılma sesi vb. buraya eklenebilir
        }
    }
}