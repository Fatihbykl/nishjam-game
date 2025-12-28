using Helper;
using UnityEngine;
using UnityEngine.AI;
using Player.States; // Namespace'inin doğru olduğundan emin ol

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
        public float predictionFactor = 1.5f;
        public float zigzagFrequency = 2.0f;
        public float zigzagAmplitude = 2.0f;
        public float hesitationTime = 1.5f;
        public float stunDuration = 4.0f;

        [Header("References")]
        public Transform playerTransform;
        public PlayerStateManager playerState;
        public SkinnedMeshRenderer[] meshRenderers;
        public JumpscareManager jumpscareManager;

        private NavMeshAgent agent;
        private Animator animator;
        private PlayerBaseState lastKnownState;

        // --- TIMER DEĞİŞKENLERİ ---
        private float currentStunTimer = 0f;      // Stun süresini tutar
        private float currentHesitationTimer = 0f; // Tereddüt süresini tutar
        
        // State kontrolü için basit bool yerine timer kontrolü yapacağız
        public bool IsStunned => currentStunTimer > 0f;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            agent.speed = normalSpeed;
            
            // Başlangıçta state'i al
            if(playerState != null) lastKnownState = playerState._currentState;
        }

        void Update()
        {
            // 1. STUN MANTIĞI (En yüksek öncelik)
            if (currentStunTimer > 0f)
            {
                HandleStunState();
                return; // Stun yemişse başka hiçbir şey yapma
            }

            // 2. MESAFE VE ÖLÜM KONTROLÜ
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= killDistance)
            {
                GameOver();
                return;
            }

            // 3. STATE DEĞİŞİMİ KONTROLÜ
            CheckStateChange();

            // 4. HAREKET MANTIĞI
            HandleMovementLogic();

            // 5. GÖRÜNÜRLÜK
            HandleVisibility();
        }

        // --- Stun Yönetimi ---
        void HandleStunState()
        {
            // Süreyi azalt
            currentStunTimer -= Time.deltaTime;

            // Stun halindeyken hareket etmemeli
            if (!agent.isStopped) 
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                animator.SetBool("Walking", false);
                animator.SetBool("Stunned", true);
            }

            // Süre bittiğinde
            if (currentStunTimer <= 0f)
            {
                RecoverFromStun();
            }
        }

        public void GetStunned()
        {
            Debug.Log("Düşman Vuruldu! Timer Resetlendi.");
            
            // SADECE SÜREYİ FULLÜYORUZ. Coroutine durdur/başlat derdi yok.
            currentStunTimer = stunDuration;
            
            // Hesitation (Tereddüt) süresini sıfırla ki stun bitince hemen saldırsın
            currentHesitationTimer = 0f; 

            // Animasyonu anlık olarak tekrar tetiklemek istersen:
            animator.Play("Stunned", 0, 0f); // Stun animasyonunu baştan oynat
        }

        void RecoverFromStun()
        {
            currentStunTimer = 0f;
            agent.isStopped = false;
            animator.SetBool("Stunned", false);
        }

        // --- State ve Hareket Yönetimi ---
        void CheckStateChange()
        {
            if (playerState._currentState != lastKnownState)
            {
                // Eğer yeni mod ARMS ise, tereddüt zamanlayıcısını kur
                if (playerState._currentState is ArmsState)
                {
                    currentHesitationTimer = hesitationTime;
                }
                else
                {
                    // Başka moda geçtiysek tereddütü iptal et
                    currentHesitationTimer = 0f;
                }

                lastKnownState = playerState._currentState;
            }
        }

        void HandleMovementLogic()
        {
            // NavMesh aktif değilse işlem yapma (Hata önleyici)
            if (!agent.isOnNavMesh) return;

            // A. BACAK MODU
            if (playerState._currentState is LegsState)
            {
                agent.isStopped = false;
                agent.speed = normalSpeed;
                
                Vector3 playerVelocity = playerState._controller.velocity;
                Vector3 predictedPos = playerTransform.position + (playerVelocity * predictionFactor);
                agent.SetDestination(predictedPos);
                
                animator.SetBool("Walking", true);
            }
            // B. KAFA MODU
            else if (playerState._currentState is HeadState)
            {
                agent.isStopped = false;
                agent.speed = slowSpeed;

                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                Vector3 sideVector = Vector3.Cross(directionToPlayer, Vector3.up);
                float zigzagOffset = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
                Vector3 zigzagTarget = playerTransform.position + (sideVector * zigzagOffset);
                
                agent.SetDestination(zigzagTarget);
                animator.SetBool("Walking", true);
            }
            // C. KOL MODU (Hesitation Timer burada işliyor)
            else if (playerState._currentState is ArmsState)
            {
                if (currentHesitationTimer > 0f)
                {
                    // Tereddüt süresi azalıyor
                    currentHesitationTimer -= Time.deltaTime;
                    
                    // Duruyor
                    agent.isStopped = true;
                    agent.velocity = Vector3.zero;
                    animator.SetBool("Walking", false);
                }
                else
                {
                    // Süre bitti, saldır
                    agent.isStopped = false;
                    agent.speed = normalSpeed;
                    agent.SetDestination(playerTransform.position);
                    animator.SetBool("Walking", true);
                }
            }
        }

        void HandleVisibility()
        {
            // Stun yemişse VEYA Kafa modundaysa görünür
            bool shouldBeVisible = (playerState._currentState is HeadState) || IsStunned;
            // İstersen testi kolaylaştırmak için bunu 'true' yapabilirsin
            // bool shouldBeVisible = true; 

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if(meshRenderers[i].enabled != shouldBeVisible)
                    meshRenderers[i].enabled = shouldBeVisible;
            }
        }

        void GameOver()
        {
            Debug.Log("JUMPSCARE!");
    
            if (jumpscareManager != null)
            {
                jumpscareManager.TriggerJumpscare();
        
                this.enabled = false; 
                agent.isStopped = true;
            }
        }
    }
}