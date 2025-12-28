using DG.Tweening;
using UnityEngine;

// DOTween kullandığın için animasyonu onunla yapıyoruz

namespace LevelProps
{
    public class DoorController : MonoBehaviour
    {
        [Header("References")]
        public ShootableSwitch[] requiredSwitches;
        public Transform doorModel;

        [Header("Animation Settings")]
        public Vector3 openPositionOffset = new Vector3(0, 3, 0);
        public float openSpeed = 1.0f;
    
        private bool isOpen = false;

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
            
            // Kapı açılma sesi vb. buraya eklenebilir
        }
    }
}