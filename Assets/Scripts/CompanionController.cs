using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    [SerializeField] private float m_TeleportOffset;
    private Vector3 m_MovementDirection;
    private Rigidbody m_Rigidbody;
    private Vector3 m_StartSize;
    private bool m_EnterPortal; 
    public static Action OnCubeDestroyed;
    private Portal m_Portal; 
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_StartSize = transform.localScale; 
    }

    private void FixedUpdate()
    {
        if (m_Rigidbody.velocity.magnitude > 0.01f)
        {
            m_MovementDirection = m_Rigidbody.velocity.normalized;
        }
        else
        {
            m_MovementDirection = Vector3.zero;
        }
    }

    private void Update()
    {
        if (m_EnterPortal)
        {
            Teleport(m_Portal);
        }
    }

    private void Teleport(Portal l_portal)
    {
        m_MovementDirection.Normalize();

        Vector3 l_Position = transform.position + m_MovementDirection * m_TeleportOffset;
        Vector3 l_LocalPosition = l_portal.m_OtherPortalTransform.InverseTransformPoint(l_Position);
        Vector3 l_WorldPosition = l_portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_LocalForward = l_portal.m_OtherPortalTransform.InverseTransformDirection(m_MovementDirection);
        Vector3 l_WorldForward = l_portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);

        transform.position = l_WorldPosition;
        transform.forward = l_WorldForward;
        m_Rigidbody.velocity = l_WorldForward * m_Rigidbody.velocity.magnitude;
        Debug.Log(l_portal.m_MirrorPortal.m_PortalSize); 
        transform.localScale = transform.localScale * l_portal.m_MirrorPortal.m_PortalSize; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            m_Portal = other.GetComponent<Portal>();
        }
    }

    private void OnDestroy()
    {
        OnCubeDestroyed?.Invoke();
    }
}
