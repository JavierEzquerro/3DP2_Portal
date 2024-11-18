using System;
using System.Collections;
using UnityEngine;

public class LaserEmitter : MonoBehaviour
{
    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] private AudioClip m_LaserSound;
    [SerializeField] private float m_LaserDistance = 8f;
    [SerializeField] private LayerMask m_LayerMask;
    [SerializeField] private float m_LaserOnTime = 2f;
    [SerializeField] private float m_LaserOffTime = 1f;

    private bool m_LaserActive = true;
    private bool m_SoundPlayed = false;

    public static Action OnLaserReceived;

    private void Awake()
    {
        m_LineRenderer.positionCount = 2;
        //StartCoroutine(ToggleLaser());
    }

    private void Update()
    {
        if (!m_LaserActive)
        {
            m_LineRenderer.enabled = false;
            return;
        }

        if (!m_SoundPlayed)
        {
            //SoundsManager.instance.PlayLongSound3D(m_LaserSound, transform, 0.2f, laserOnTime);
            m_SoundPlayed = true;
        }

        m_LineRenderer.enabled = true;

        Ray l_Ray;
        RaycastHit l_RayHit;
        l_Ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(l_Ray, out l_RayHit))
        {
            m_LineRenderer.SetPosition(0, transform.position);
            m_LineRenderer.SetPosition(1, l_RayHit.point);

            if (l_RayHit.collider.CompareTag("Player") && l_RayHit.collider.TryGetComponent(out PlayerLifeController l_PlayerLifeController))
            {
                float l_LaserDuration = l_PlayerLifeController.m_TimeToKillPlayer;
                float l_PlayerHealth = l_PlayerLifeController.m_MaxPlayerHealth;
                float l_DamagePerSecond = l_PlayerHealth / l_LaserDuration;
                GameManager.instance.ReportPlayerDamaged(l_DamagePerSecond);
            }

            if (l_RayHit.collider.CompareTag("LaserReceiver"))
            {

            }
        }
        else
        {
            m_LineRenderer.SetPosition(0, transform.position);
            m_LineRenderer.SetPosition(1, transform.position + transform.forward * m_LaserDistance);
        }
    }

    /*private IEnumerator ToggleLaser()
    {
        while (true)
        {
            m_LaserActive = true;
            m_SoundPlayed = false;
            yield return new WaitForSeconds(m_LaserOnTime);
            m_LaserActive = false;
            yield return new WaitForSeconds(m_LaserOffTime);
        }
    }*/
}
