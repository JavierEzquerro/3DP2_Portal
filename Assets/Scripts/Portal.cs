using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform m_OtherPortalTransform;
    public Portal m_MirrorPortal;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private float m_OffsetCamera;


    private void Update()
    {
        Camera l_CameraPlayerController =  GameManager.instance.GetPlayer().m_Camera.GetComponent<Camera>();
        Vector3 l_Position = l_CameraPlayerController.transform.position;
        Vector3 l_Forward = l_CameraPlayerController.transform.forward;
        Vector3 l_LocalPosition = m_OtherPortalTransform.InverseTransformPoint(l_Position);
        Vector3 l_LocalForward = m_OtherPortalTransform.InverseTransformDirection(l_Forward);

        Vector3 l_WorldPosition = m_MirrorPortal.transform.TransformPoint(l_LocalPosition);
        Vector3 l_WorldForward = m_MirrorPortal.transform.TransformDirection(l_LocalForward);
        m_MirrorPortal.m_Camera.transform.position = l_WorldPosition;
        m_MirrorPortal.m_Camera.transform.forward = l_WorldForward;

        float l_DistanceToPortal = Vector3.Distance(l_WorldPosition, m_MirrorPortal.transform.position);
        float l_DistanceNearClipPlane = m_OffsetCamera + l_DistanceToPortal;
        m_MirrorPortal.m_Camera.nearClipPlane = l_DistanceNearClipPlane;
    }
} 
