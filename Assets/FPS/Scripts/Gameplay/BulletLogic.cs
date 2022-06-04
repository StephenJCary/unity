using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.FPS.Game;

namespace Unity.FPS.Gameplay
{
    public class BulletLogic : ProjectileBase
    {
        [SerializeField]
        int m_damage = 10;
        public float Radius = 0.01f;

        const float BULLET_SPEED = 20.0f;

        public Transform Root;

        public float Damage = 40f;
        
        public Transform Tip;

        Vector3 m_Velocity;
        Vector3 m_LastRootPosition;

        
        public GameObject ImpactVfx;

        
        public float ImpactVfxLifetime = 5f;

        ProjectileBase m_ProjectileBase;
        public float Speed = 20f;
        public float ImpactVfxSpawnOffset = 0.1f;

        float holdtime = 0.1f;
   
        public AudioClip ImpactSfxClip;

        const QueryTriggerInteraction k_TriggerInteraction = QueryTriggerInteraction.Collide;

        public LayerMask HittableLayers = -1;

        List<Collider> m_IgnoredColliders;
        float m_ShootTime;

        GameObject m_player;

        public float MaxLifeTime = 5f;
        // Start is called before the first frame update
        void Start()
        {
            m_Velocity = transform.forward * BULLET_SPEED;
            m_IgnoredColliders = new List<Collider>();
            m_LastRootPosition = Root.position;
            m_player = GameObject.FindGameObjectWithTag("player");
            Collider[] ownerColliders = m_player.GetComponentsInChildren<Collider>();
            m_IgnoredColliders.AddRange(ownerColliders);
        
            m_ProjectileBase = GetComponent<ProjectileBase>();
            DebugUtility.HandleErrorIfNullGetComponent<ProjectileBase, ProjectileStandard>(m_ProjectileBase, this,
                gameObject);

            m_ProjectileBase.OnShoot += OnShoot;

            Destroy(gameObject, MaxLifeTime);
        }

        new void OnShoot()
        {
            m_ShootTime = Time.time;
            m_LastRootPosition = Root.position;
            m_Velocity = transform.forward * Speed;
            m_IgnoredColliders = new List<Collider>();
            transform.position += m_ProjectileBase.InheritedMuzzleVelocity * Time.deltaTime;

            // Ignore colliders of owner
            Collider[] ownerColliders = m_ProjectileBase.Owner.GetComponentsInChildren<Collider>();
            m_IgnoredColliders.AddRange(ownerColliders);

            
        }

        private void Update()
        {
            transform.position += m_Velocity * Time.deltaTime;

            {
                RaycastHit closestHit = new RaycastHit();
                closestHit.distance = Mathf.Infinity;
                bool foundHit = false;

                // Sphere cast
                Vector3 displacementSinceLastFrame = Tip.position - m_LastRootPosition;
                RaycastHit[] hits = Physics.SphereCastAll(m_LastRootPosition, Radius,
                    displacementSinceLastFrame.normalized, displacementSinceLastFrame.magnitude, HittableLayers,
                    k_TriggerInteraction);
                foreach (var hit in hits)
                {
                    Debug.Log(hit.distance);
                    Debug.Log(closestHit.distance);
                    

                    if (IsHitValid(hit) && hit.distance < closestHit.distance)
                    {
                        foundHit = true;
                        closestHit = hit;
                    }
                }

                if (foundHit)
                {
                    // Handle case of casting while already inside a collider
                    if (closestHit.distance <= 0f)
                    {
                        closestHit.point = Root.position;
                        closestHit.normal = -transform.forward;
                    }

                    OnHit(closestHit.point, closestHit.normal, closestHit.collider);
                }
            }
            m_LastRootPosition = Root.position;
        }
        bool IsHitValid(RaycastHit hit)
        {
            Debug.Log(hit.collider.tag);
            
           

            // ignore hits with an ignore component
            if (hit.collider.GetComponent<IgnoreHitDetection>())
            {
                return false;
            }

            // ignore hits with triggers that don't have a Damageable component
            if (hit.collider.isTrigger && hit.collider.GetComponent<Damageable>() == null)
            {
                return false;
            }

            if (m_IgnoredColliders != null && m_IgnoredColliders.Contains(hit.collider))
            {
                return false;
            }


            return true;
        }

        void OnHit(Vector3 point, Vector3 normal, Collider collider)
        {

            //Debug.Log("o");
            // point damage
            Damageable damageable = collider.GetComponent<Damageable>();
            if (damageable)
            {
                damageable.InflictDamage(Damage, false, m_ProjectileBase.Owner);
            }


            // impact vfx
            if (ImpactVfx)
            {
                GameObject impactVfxInstance = Instantiate(ImpactVfx, point + (normal * ImpactVfxSpawnOffset),
                    Quaternion.LookRotation(normal));
                if (ImpactVfxLifetime > 0)
                {
                    Destroy(impactVfxInstance.gameObject, ImpactVfxLifetime);
                }
            }

            // impact sfx
            if (ImpactSfxClip)
            {
                AudioUtility.CreateSFX(ImpactSfxClip, point, AudioUtility.AudioGroups.Impact, 1f, 3f);
            }

            // Self Destruct
             Destroy(this.gameObject);
        }
        
    }
}