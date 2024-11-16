using System;
using UnityEngine;

public class Turret : TeleportableObjects, IRestartGame
{
    [SerializeField] private float m_MaxAngleLaserAlive = 10.0f;

    private Vector3 m_StartPosition; 
    private Quaternion m_StartRotation; 

    public LineRenderer m_LaserRenderer;
    public LayerMask m_LayerMask;
    public float m_MaxDistance = 50.0f;

    [Header("Sounds")]
    [SerializeField] private AudioClip m_TurretDeathSound;

    public static Action OnLaserReceived;
    public static Action<float> OnPlayerDamagedByLaser;

    
    public override void Start()
    {
        base.Start(); 
        GameManager.instance.AddTurretToRestart(this);
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
    }

    public override void Update()
    {
        base.Update();
        if (IsLaserAlive())
        {
            Ray l_Ray = new Ray(m_LaserRenderer.transform.position, m_LaserRenderer.transform.forward);

            if (Physics.Raycast(l_Ray, out RaycastHit l_HitInfo, m_MaxDistance, m_LayerMask.value))
            {
                m_LaserRenderer.SetPosition(1, new Vector3(0, 0, l_HitInfo.distance));
                m_LaserRenderer.gameObject.SetActive(true);

                if (l_HitInfo.collider.CompareTag("RefractionCube"))
                {
                    m_LaserRenderer.SetPosition(1, new Vector3(0, 0, l_HitInfo.distance));
                    m_LaserRenderer.gameObject.SetActive(true);
                }
                else if (l_HitInfo.collider.CompareTag("Turret"))
                {
                    //Animacion
                    if (m_Portal != null)
                    {
                        m_Portal.m_LaserEnabled = false;
                        Debug.Log("No laser");
                    }
                    SoundsManager.instance.PlaySoundClip(m_TurretDeathSound, transform, 0.2f);
                    Destroy(l_HitInfo.collider.gameObject);
                }
                else if (l_HitInfo.collider.CompareTag("Portal"))
                {
                    m_Portal = l_HitInfo.collider.GetComponent<Portal>();
                    m_LaserRenderer.SetPosition(1, new Vector3(0, 0, l_HitInfo.distance + Vector3.Distance(l_HitInfo.point, m_Portal.transform.position) + 0.5f));
                    m_Portal.RayReflection(l_Ray, l_HitInfo);
                }
                else if (l_HitInfo.collider.CompareTag("LaserReceiver"))
                {
                    OnLaserReceived?.Invoke();
                }
                else if (l_HitInfo.collider.TryGetComponent(out PlayerLifeController l_PlayerLifeController))
                {
                    float l_LaserDuration = l_PlayerLifeController.m_TimeToKillPlayer;
                    float l_MaxHealthPlayerHealth = l_PlayerLifeController.m_MaxPlayerHealth;
                    float l_DamagePerSecond = l_MaxHealthPlayerHealth / l_LaserDuration;
                    OnPlayerDamagedByLaser?.Invoke(Time.deltaTime * l_DamagePerSecond);
                }
            }
        }
        else
            m_LaserRenderer.gameObject.SetActive(false);
    }

    private bool IsLaserAlive()
    {
        return Vector3.Dot(transform.up, Vector3.up) > Mathf.Cos(m_MaxAngleLaserAlive * Mathf.Deg2Rad);
    }

    public void RestartGame()
    {
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
    }
}
