using UnityEngine;

namespace LevelProps
{
    public class DoorController : MonoBehaviour
    {
        [Header("References")]
        public ShootableSwitch[] requiredSwitches;
        public PressurePlate[] requiredPlates;
    
        private bool isOpen = false;

        public AudioSource effect;

        public void CheckSwitches()
        {
            if (isOpen) return;

            foreach (var sw in requiredSwitches)
            {
                if (!sw.isActivated) return;
            }
            
            if (requiredPlates != null)
            {
                foreach (var plate in requiredPlates)
                {
                    if (!plate.isCompleted) return;
                }
            }

            OpenDoor();
        }

        void OpenDoor()
        {
            Debug.Log("Tüm şalterler açık! Kapı açılıyor.");
            isOpen = true;

            GetComponent<Animator>().SetTrigger("Open");

            effect.Play();
        }
    }
}