using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneObjectController : MonoBehaviour
{
    [SerializeField] GameObject m_Object; 
    public bool m_Clone;
    private TeleportableObjects m_Teleportable;
    private Portal m_MirrorPortal; 
    private Transform m_PortalTransform;
    private Vector3 m_StartSize; 

    void Start()
    {
        m_Clone = false; 
        m_StartSize = transform.localScale;
    }


    void Update()
    {
        if (m_Clone)
        {
            m_Object.SetActive(true);

            Vector3 l_Offset = m_MirrorPortal.transform.position - transform.position;
            l_Offset.Normalize();

            if(Vector3.Dot(l_Offset, m_MirrorPortal.transform.forward) >0)
            {
               // this.gameObject.SetActive(true);
            }

            Vector3 l_Postion = m_Teleportable.transform.position; 
            Vector3 l_LocalPosition = m_PortalTransform.transform.InverseTransformPoint(l_Postion); 
            Vector3 l_WorldPosition = m_MirrorPortal.transform.TransformPoint(l_LocalPosition); 

            Vector3 l_Forward = m_Teleportable.transform.forward;
            Vector3 l_LocalForward = m_PortalTransform.transform.InverseTransformDirection(l_Forward);
            Vector3 l_WorldFoward = m_MirrorPortal.transform.TransformDirection(l_LocalForward);   
            
            transform.position = l_WorldPosition;
            transform.forward = l_WorldFoward;
        }
        else
        {
           m_Object.SetActive(false);
        }
    }

    public void TeleportObjectClone(TeleportableObjects l_teleportableObject, Portal l_mirrorPortal, Transform l_PortalTransform)
    {
        m_MirrorPortal = l_mirrorPortal;
        m_Teleportable = l_teleportableObject;  
        m_PortalTransform = l_PortalTransform;
        transform.localScale = m_StartSize * l_mirrorPortal.m_PortalSize; 
        m_Clone=true;   
    }
}