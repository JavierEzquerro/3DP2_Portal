using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private float m_OffsetCamera; 
    [SerializeField] private float m_SpeedAnimation; 
    public Transform m_OtherPortalTransform;
    public Portal m_MirrorPortal;
    public Camera m_Camera;
    public float m_PortalSize;
    public bool m_PortalAnimation;
    private float m_AnimationProgress;
    public Vector3 m_StartSizeAnimation;
    private Vector3 m_StartSizePortal;

    public Collider m_WallPortaled;
    public GameObject m_Weapon; 
    public GameObject m_CloneWeapon; 

    private void Start()
    {
        m_StartSizePortal = transform.localScale; 
        m_StartSizeAnimation = m_StartSizePortal / 100;
        m_PortalAnimation = false;
        m_AnimationProgress = 0f;
        m_CloneWeapon.SetActive(true); 
    }

    private void Update()
    {
        Camera l_CameraPlayerController =  GameManager.instance.GetPlayer().m_Camera.GetComponent<Camera>();
        Vector3 l_Position = l_CameraPlayerController.transform.position;
        Vector3 l_LocalPosition = m_OtherPortalTransform.InverseTransformPoint(l_Position);
        Vector3 l_WorldPosition = m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_Forward = l_CameraPlayerController.transform.forward;
        Vector3 l_LocalForward = m_OtherPortalTransform.InverseTransformDirection(l_Forward);
        Vector3 l_WorldForward = m_MirrorPortal.transform.TransformDirection(l_LocalForward);
        m_MirrorPortal.m_Camera.transform.position = l_WorldPosition;
        m_MirrorPortal.m_Camera.transform.forward = l_WorldForward;

        float l_DistanceToPortal = Vector3.Distance(l_WorldPosition, m_MirrorPortal.transform.position);
        float l_DistanceNearClipPlane = m_OffsetCamera + l_DistanceToPortal;
        m_MirrorPortal.m_Camera.nearClipPlane = l_DistanceNearClipPlane;
     
        if (m_PortalAnimation)
            PortalAnimation();
    }

    public void PortalAnimation()
    {
        Vector3 l_Size = m_StartSizePortal * m_PortalSize;
        m_AnimationProgress += m_SpeedAnimation * Time.deltaTime;  

        transform.localScale = Vector3.Lerp(m_StartSizeAnimation, l_Size, m_AnimationProgress);

        if (m_AnimationProgress >= 1f)
        {
            m_PortalAnimation = false;
            m_AnimationProgress = 0f;   
        }
    }
} 
