using Enemy;
using LevelProps;
using UnityEngine;

namespace Player
{
    public class RevolverProjectile : MonoBehaviour
    {
        public float lifeTime = 3f;
        public GameObject impactEffect;

        void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Bullet")) return;

            if (other.gameObject.layer == LayerMask.NameToLayer("Default")) 
            {
                DestroyProjectile();
                return;
            }

            ShootableSwitch shootableSwitch = other.gameObject.GetComponentInParent<ShootableSwitch>();

            if (shootableSwitch != null)
            {
                Debug.Log("Switch Hit");
                shootableSwitch.GetHit();
                DestroyProjectile();
                return;
            }
            
            SchrodingerEnemyAI enemy = other.GetComponentInParent<SchrodingerEnemyAI>();

            if (enemy != null)
            {
                Debug.Log(other.gameObject.name);
                enemy.GetStunned();
                DestroyProjectile();
            }
        }

        void DestroyProjectile()
        {
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
