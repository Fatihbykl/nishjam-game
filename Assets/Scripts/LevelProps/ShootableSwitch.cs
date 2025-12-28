using UnityEngine;

namespace LevelProps
{
    public class ShootableSwitch : MonoBehaviour
    {
        [Header("Settings")]
        public DoorController linkedDoor;
        public MeshRenderer switchRenderer;
        public Color activeColor = Color.green;
        public bool isActivated = false;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip activationSound;

        public void GetHit()
        {
            if (isActivated) return;

            Debug.Log("Şalter Vuruldu!");
            isActivated = true;

            if (switchRenderer != null)
            {
                switchRenderer.material.color = activeColor;
                switchRenderer.material.EnableKeyword("_EMISSION");
                switchRenderer.material.SetColor("_EmissionColor", activeColor);
            }

            if (audioSource && activationSound)
            {
                audioSource.PlayOneShot(activationSound);
            }

            if (linkedDoor != null)
            {
                linkedDoor.CheckSwitches();
            }
        }
    }
}