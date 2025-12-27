using System.Collections;
using Player.States;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SchrodingerEnemyAI : MonoBehaviour
    {
        [Header("Core Settings")]
        public float normalSpeed = 5f;
        public float slowSpeed = 2.5f;
        public float killDistance = 1.2f;

        [Header("Behavior Settings")]
        public float predictionFactor = 1.5f; // Bacak modunda ne kadar ileriye koşsun?
        public float zigzagFrequency = 2.0f;  // Zigzag ne kadar hızlı olsun?
        public float zigzagAmplitude = 2.0f;  // Zigzag ne kadar geniş olsun?
        public float hesitationTime = 1.5f;   // Kol moduna geçince kaç sn dursun?
        public float stunDuration = 4.0f;     // Vurulunca kaç sn beklesin?

        [Header("References")]
        public Transform playerTransform;
        public PlayerStateManager playerState; // Senin yazdığın State Manager
        public MeshRenderer meshRenderer;      // Görünürlük kontrolü için

        private NavMeshAgent agent;
        private bool isStunned = false;
        private bool hasHesitated = false; // Kol modunda duraksama hakkını kullandı mı?
        private PlayerBaseState lastKnownState;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = normalSpeed;
            lastKnownState = playerState._currentState;
        }

        void Update()
        {
            // 1. Önce Stun ve Mesafe Kontrolü
            if (isStunned) return;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= killDistance)
            {
                GameOver();
                return;
            }

            // 2. Player State Değişimi Kontrolü
            CheckStateChange();

            // 3. State'e Göre Hareket Mantığı
            HandleMovementLogic();
        
            // 4. Görünürlük Mantığı (Opsiyonel: Sadece Kafa modunda görünsün istersen)
            HandleVisibility();
        }

        void CheckStateChange()
        {
            // Eğer oyuncu mod değiştirdiyse
            if (playerState._currentState != lastKnownState)
            {
                // Eğer yeni mod ARMS ise, tereddüt (hesitation) sıfırlanır
                if (playerState._currentState is ArmsState)
                {
                    hasHesitated = false;
                    StartCoroutine(HesitationRoutine());
                }

                lastKnownState = playerState._currentState;
            }
        }

        void HandleMovementLogic()
        {
            // --- DURUM 1: BACAK MODU (Tahminci Koşu) ---
            if (playerState._currentState is LegsState)
            {
                agent.speed = normalSpeed;
                agent.isStopped = false;

                // Oyuncunun hız vektörünü al ve gelecekteki konumunu tahmin et
                Vector3 playerVelocity = playerState._controller.velocity;
                Vector3 predictedPos = playerTransform.position + (playerVelocity * predictionFactor);
            
                agent.SetDestination(predictedPos);
            }
        
            // --- DURUM 2: KAFA MODU (Yavaş Zigzag) ---
            else if (playerState._currentState is HeadState)
            {
                agent.speed = slowSpeed;
                agent.isStopped = false;

                // Oyuncuya doğru giden vektör
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            
                // Bu vektöre dik bir vektör bul (Sağ/Sol)
                Vector3 sideVector = Vector3.Cross(directionToPlayer, Vector3.up);

                // Sinüs dalgası ile sağa sola öteleme hesapla
                float zigzagOffset = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
            
                // Hedef noktayı sapmayla beraber belirle
                Vector3 zigzagTarget = playerTransform.position + (sideVector * zigzagOffset);
            
                agent.SetDestination(zigzagTarget);
            }

            // --- DURUM 3: KOL MODU (Duraksama ve Saldırı) ---
            else if (playerState._currentState is ArmsState)
            {
                // Eğer hala tereddüt ediyorsa dur
                if (!hasHesitated)
                {
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                }
                else
                {
                    // Süre bitti, atış kaçtı! Şimdi normal hızda üstüne yürü
                    agent.isStopped = false;
                    agent.speed = normalSpeed;
                    agent.SetDestination(playerTransform.position);
                }
            }
        }

        // Arms moduna geçince çalışan kısa bekleme süresi
        IEnumerator HesitationRoutine()
        {
            // Duraksama başladı
            yield return new WaitForSeconds(hesitationTime);
        
            // Süre bitti, artık saldırabilir
            hasHesitated = true;
        }

        // Silahın tarafından çağırılacak fonksiyon
        public void GetStunned()
        {
            if (isStunned) return;

            Debug.Log("Düşman Vuruldu! Sersemledi.");
            StopAllCoroutines(); // Hesitation varsa iptal et
            StartCoroutine(StunRoutine());
        }

        IEnumerator StunRoutine()
        {
            isStunned = true;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            // Görsel efekt: Titreme veya donma animasyonu buraya
            if(meshRenderer) meshRenderer.material.color = Color.red; // Örnek feedback

            yield return new WaitForSeconds(stunDuration);

            isStunned = false;
            agent.isStopped = false;
            if(meshRenderer) meshRenderer.material.color = Color.white; // Eski haline dön
        }
    
        void HandleVisibility()
        {
            // Sadece Kafa modunda veya Stun yemişken görünür
            bool shouldBeVisible = (playerState._currentState is HeadState) || isStunned;
        
            if(meshRenderer) meshRenderer.enabled = shouldBeVisible;
        }

        void GameOver()
        {
            Debug.Log("ÖLDÜN! Düşman yakaladı.");
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}