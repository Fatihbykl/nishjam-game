using DG.Tweening;
using Player.States;
using UnityEngine;

namespace LevelProps
{
    public class PressurePlate : MonoBehaviour
    {
        [Header("Settings")]
        public float requiredTime = 2.0f;
        public float pressDistance = 0.2f;
        public bool isCompleted = false;

        [Header("Visuals")]
        public Transform movingPart;
        public MeshRenderer plateRenderer;
        public Color emptyColor = Color.red;
        public Color fullColor = Color.green;

        [Header("Connections")]
        public DoorController linkedDoor;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip chargingSound;
        public AudioClip completeSound;

        private float currentProgress = 0f;
        private Vector3 initialPos;
        private Vector3 targetPos;
        private bool isPlayerOnPlate = false;
        private PlayerStateManager currentPlayer;

        void Start()
        {
            if (movingPart == null) movingPart = transform;
        
            initialPos = movingPart.localPosition;
            targetPos = initialPos - new Vector3(0, pressDistance, 0);

            if(plateRenderer) plateRenderer.materials[1].color = emptyColor;
        }

        void Update()
        {
            if (isCompleted) return;

            bool conditionsMet = isPlayerOnPlate && 
                                 currentPlayer != null && 
                                 currentPlayer._currentState is LegsState;

            if (conditionsMet)
            {
                ProcessFilling();
            }
            else
            {
                ProcessDecay();
            }

            UpdateVisuals();
        }

        void ProcessFilling()
        {
            currentProgress += Time.deltaTime;

            if (audioSource && chargingSound && !audioSource.isPlaying)
            {
                audioSource.clip = chargingSound;
                audioSource.Play();
            }
            if(audioSource) audioSource.pitch = 1f + (currentProgress / requiredTime);

            if (currentProgress >= requiredTime)
            {
                CompletePlate();
            }
        }

        void ProcessDecay()
        {
            if (currentProgress > 0)
            {
                currentProgress -= Time.deltaTime * 2f;
                if (audioSource) audioSource.Stop();
            }
        }

        void UpdateVisuals()
        {
            float t = Mathf.Clamp01(currentProgress / requiredTime);

            movingPart.localPosition = Vector3.Lerp(initialPos, targetPos, t);

            if (plateRenderer)
            {
                plateRenderer.materials[1].color = Color.Lerp(emptyColor, fullColor, t);
            
                plateRenderer.materials[1].SetColor("_EmissionColor", Color.Lerp(emptyColor, fullColor, t));
            }
        }

        void CompletePlate()
        {
            isCompleted = true;
            currentProgress = requiredTime;
        
            movingPart.DOShakePosition(0.2f, 0.05f);
        
            if(audioSource)
            {
                audioSource.Stop();
                audioSource.pitch = 1f;
                audioSource.PlayOneShot(completeSound);
            }

            Debug.Log("Plaka Kilitlendi!");
        
            if (linkedDoor != null)
            {
                linkedDoor.CheckSwitches(); 
            }
        }

        // --- TRIGGER EVENTS ---

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerOnPlate = true;
                currentPlayer = other.GetComponent<PlayerStateManager>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerOnPlate = false;
                currentPlayer = null;
            }
        }
    }
}