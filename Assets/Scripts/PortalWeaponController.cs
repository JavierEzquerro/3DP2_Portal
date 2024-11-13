using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class PortalWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject m_BluePreviewPortal;
    [SerializeField] private GameObject m_OrangePreviewPortal;
    [SerializeField] private Camera m_Camera;
    [SerializeField] private GameObject m_BluePortal;
    [SerializeField] private GameObject m_OrangePortal;
    [SerializeField] private float m_DistanceRay;
    [SerializeField] private float m_ThresholdPortal;
    [SerializeField] private float m_ForceLaunch;
    [SerializeField] private float m_DistanceStopLerpAttract;
    [SerializeField] private GameObject m_CrossHairBlue;
    [SerializeField] private GameObject m_CrossHairOrange;
    [SerializeField] private float m_ScrollWheelIncrement;
    [SerializeField] private float m_AngleValidPortal;
    [SerializeField] private GameObject m_BulletPortalBlue; 
    [SerializeField] private GameObject m_BulletPortalOrange;
    [SerializeField] private Transform m_ShootPoint;

    private Player_Controller m_PlayerController;
    public List<Transform> m_ValidPoints = new List<Transform>();
    public Transform m_AttractPoint;
    private bool m_AttractingObjects;
    private bool m_TrapedObject;
    private GameObject m_ObjectAttract;
    private Rigidbody m_RbObjectAttract;
    private BoxCollider m_ObjectCollider;
    public float m_AttractSpeed;
    private int m_ReSize;
    private Vector3 m_StartScale;
    private float m_CurrentPortalSize;
    private float m_AttractingPorgress;
    private float m_Angle;
    private Transform m_AttachedPreviousParent;
    private float m_PreviewAnimation;
    private bool m_CanShootBlue;
    private bool m_CanShootOrange;

    private void Start()
    {
        m_AttractingObjects = false;
        m_TrapedObject = false;

        m_ReSize = 0;
        m_StartScale = transform.localScale;
        m_CurrentPortalSize = m_ReSize;
        m_PreviewAnimation = 0.0f;

        m_CrossHairBlue.SetActive(false);
        m_CrossHairOrange.SetActive(false);

        m_Angle = Mathf.Cos(m_AngleValidPortal * Mathf.Deg2Rad);
        m_PlayerController = GetComponent<Player_Controller>();

        m_BulletPortalBlue.gameObject.SetActive(false);
        m_BulletPortalBlue.transform.position = m_ShootPoint.position;
        m_CanShootBlue = true;
        m_CanShootOrange = true; 
    }

    private void Update()
    {
        RaycastHit l_hit;
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(l_Ray.origin, l_Ray.direction, out l_hit, m_DistanceRay, ~0, QueryTriggerInteraction.Ignore))
        {
            if (!m_AttractingObjects && !m_TrapedObject)
            {
                //PORTAL
                m_BluePreviewPortal.transform.rotation = Quaternion.LookRotation(l_hit.normal);
                m_BluePreviewPortal.transform.position = l_hit.point;
                m_OrangePreviewPortal.transform.rotation = Quaternion.LookRotation(l_hit.normal);
                m_OrangePreviewPortal.transform.position = l_hit.point;

                if (IsValidPosition())
                {
                    //PREVIEW
                    if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                    {
                        float l_ScrollWheel = Input.GetAxis("Mouse ScrollWheel");

                        if (l_ScrollWheel > 0)
                        {
                            m_ReSize += 1;
                            m_PreviewAnimation = 0; 
                        }
                        else if (l_ScrollWheel < 0)
                        {
                            m_ReSize -= 1;
                            m_PreviewAnimation = 0;
                        }

                        PreviewPortalAnimation(); 

                        m_ReSize = Mathf.Clamp(m_ReSize, -1, 1);

                        if (Input.GetMouseButton(0))
                            m_BluePreviewPortal.SetActive(true);
                        else if (Input.GetMouseButton(1))
                            m_OrangePreviewPortal.SetActive(true);
                    }
                    else
                    {
                        m_BluePreviewPortal.SetActive(false);
                        m_OrangePreviewPortal.SetActive(false);
                    }

                    PortalBullet l_bulletBlue = m_BulletPortalBlue.GetComponent<PortalBullet>();
                    PortalBullet l_bulletOrange = m_BulletPortalOrange.GetComponent<PortalBullet>();

                    if (Input.GetMouseButtonUp(0) && m_CanShootBlue)
                    {
                        m_BulletPortalBlue.SetActive(true);
                        l_bulletBlue.Shoot(m_ShootPoint.position, l_Ray.direction);
                        m_CanShootBlue = false;
                    }

                    if (Input.GetMouseButtonUp(1) && m_CanShootOrange)
                    {
                        m_BulletPortalOrange.SetActive(true);
                        l_bulletOrange.Shoot(m_ShootPoint.position, l_Ray.direction);
                        m_CanShootOrange = false;   
                    }

                    if (l_bulletBlue.m_Colisioned)
                    {
                        ActivePortalBlue(l_hit); 
                        m_CanShootBlue = true;
                        l_bulletBlue.m_Colisioned = false;
                    }

                    if (l_bulletOrange.m_Colisioned)
                    {
                        ActivePortalOrange(l_hit);
                        m_CanShootOrange = true;
                        l_bulletOrange.m_Colisioned = false; 
                    }
                }
                else
                {
                    m_BluePreviewPortal.SetActive(false);
                    m_OrangePreviewPortal.SetActive(false);
                }
            }

            //ATTRACT OBJECTS
            if (l_hit.collider.CompareTag("CompanionCube") || l_hit.collider.CompareTag("Turret"))
            {
                if (Input.GetMouseButtonDown(0) && !m_TrapedObject && !m_AttractingObjects)
                {
                    AttractObject(l_hit);
                }
            }
        }

        if(m_ObjectAttract == null)
        {
            m_TrapedObject = false;
            m_AttractingObjects = false;
        }

        if (Input.GetMouseButtonDown(0) && m_TrapedObject)
        {
            m_RbObjectAttract.isKinematic = false;
            m_ObjectAttract.transform.SetParent(m_AttachedPreviousParent);
            m_TrapedObject = false;
            m_RbObjectAttract.useGravity = true;
            m_ObjectCollider.enabled = true;
            m_RbObjectAttract.AddForce(m_Camera.transform.forward * m_ForceLaunch);
        }
        else if (Input.GetMouseButtonDown(1) && m_TrapedObject)
        {
            m_RbObjectAttract.isKinematic = false;
            m_ObjectAttract.transform.SetParent(m_AttachedPreviousParent);
            m_TrapedObject = false;
            m_RbObjectAttract.useGravity = true;
            m_ObjectCollider.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (m_AttractingObjects)
        {
            m_AttractingPorgress += m_AttractSpeed * Time.deltaTime; 

            m_ObjectAttract.transform.position = Vector3.Lerp(m_ObjectAttract.transform.position, m_AttractPoint.transform.position, m_AttractingPorgress);
            m_ObjectAttract.transform.forward = Vector3.Lerp(m_ObjectAttract.transform.forward, transform.forward, m_AttractingPorgress);

            if(m_AttractingPorgress >= 1)
            {
                m_ObjectAttract.transform.SetParent(m_AttractPoint.transform);
                m_ObjectAttract.transform.localPosition = Vector3.zero;
                m_AttractingObjects = false;
                m_TrapedObject = true;
                m_AttractingPorgress = 0;   
            }
        }

        if (m_TrapedObject && m_RbObjectAttract != null)
        {
            m_RbObjectAttract.isKinematic = true;
        }
    }

    private void AttractObject(RaycastHit l_hit)
    {
        m_ObjectAttract = l_hit.collider.gameObject;
        m_RbObjectAttract = m_ObjectAttract.GetComponent<Rigidbody>();
        m_ObjectCollider = m_RbObjectAttract.GetComponent<BoxCollider>();
        m_AttachedPreviousParent = m_RbObjectAttract.transform.parent;  


        bool l_IsTurret = false;

        if (l_hit.collider.CompareTag("Turret"))
            l_IsTurret = true;

        if (l_IsTurret)
        {
            //m_ObjectAttract.transform.forward = Vector3.Lerp(m_ObjectAttract.transform.forward, transform.forward, m_AttractSpeed * Time.deltaTime); 
        }

        m_AttractingObjects = true;
        m_RbObjectAttract.useGravity = false;
        m_RbObjectAttract.velocity = Vector3.zero;
        m_RbObjectAttract.angularVelocity = Vector3.zero;
        //m_ObjectCollider.enabled = false;
    }

    private void ActivePortalBlue(RaycastHit l_hit)
    {
        m_BluePortal.SetActive(true);
        Portal l_portal = m_BluePortal.GetComponent<Portal>();
        l_portal.m_WallPortaled = l_hit.collider.GetComponent<Collider>();
        l_portal.transform.localScale = l_portal.m_StartSizeAnimation;
        l_portal.m_PortalSize = m_CurrentPortalSize;
        l_portal.m_PortalAnimation = true;
        m_CrossHairBlue.SetActive(true);
        m_BluePortal.transform.rotation = Quaternion.LookRotation(l_hit.normal);
        m_BluePortal.transform.position = l_hit.point;
    }

    private void ActivePortalOrange(RaycastHit l_hit)
    {
        m_OrangePortal.SetActive(true);
        Portal l_portal = m_OrangePortal.GetComponent<Portal>();
        l_portal.m_WallPortaled = l_hit.collider.GetComponent<Collider>(); 
        l_portal.transform.localScale = l_portal.m_StartSizeAnimation;
        l_portal.m_PortalSize = m_CurrentPortalSize;
        l_portal.m_PortalAnimation = true;
        m_CrossHairOrange.SetActive(true);
        m_OrangePortal.transform.rotation = Quaternion.LookRotation(l_hit.normal);
        m_OrangePortal.transform.position = l_hit.point;
    }

    public bool IsValidPosition()
    {
        bool isValid = true;
        RaycastHit l_hit;
        Vector3 l_CameraPosition = m_Camera.transform.position;

        for (int i = 0; i < m_ValidPoints.Count; i++)
        {
            Vector3 l_Diretion = m_ValidPoints[i].transform.position - m_Camera.transform.position;

            if (Physics.Raycast(l_CameraPosition, l_Diretion, out l_hit, m_DistanceRay, ~0, QueryTriggerInteraction.Ignore))
            {
                float l_Dotangle = Vector3.Dot(l_hit.normal, m_ValidPoints[i].forward);

                if (Vector3.Distance(m_ValidPoints[i].transform.position, l_hit.point) >= m_ThresholdPortal ||
                    !l_hit.collider.CompareTag("WhiteWall") ||
                    l_Dotangle >= m_Angle || l_hit.collider.CompareTag("Portal"))
                {
                    return false;
                }
            }
            else
                return false;
        }
        return isValid;
    }

    public void PreviewPortalAnimation()
    {

        float l_speedAnimation = 1;
        m_PreviewAnimation += Time.deltaTime * l_speedAnimation; 

        if (m_ReSize == 1)
        {
            m_BluePreviewPortal.transform.localScale = Vector3.Lerp(m_BluePreviewPortal.transform.localScale, m_StartScale * 2, m_PreviewAnimation);
            m_OrangePreviewPortal.transform.localScale = Vector3.Lerp(m_OrangePreviewPortal.transform.localScale, m_StartScale * 2, m_PreviewAnimation);
            m_CurrentPortalSize = 2;
        }
        else if (m_ReSize == 0)
        {
            m_BluePreviewPortal.transform.localScale = Vector3.Lerp(m_BluePreviewPortal.transform.localScale, m_StartScale, m_PreviewAnimation);
            m_OrangePreviewPortal.transform.localScale = Vector3.Lerp(m_OrangePreviewPortal.transform.localScale, m_StartScale, m_PreviewAnimation);
            m_CurrentPortalSize = 1;
        }
        else if (m_ReSize == -1)
        {
            m_BluePreviewPortal.transform.localScale = Vector3.Lerp(m_BluePreviewPortal.transform.localScale, m_StartScale / 2, m_PreviewAnimation);
            m_OrangePreviewPortal.transform.localScale = Vector3.Lerp(m_OrangePreviewPortal.transform.localScale, m_StartScale / 2, m_PreviewAnimation);
            m_CurrentPortalSize = 0.5f;
        }

        if(m_PreviewAnimation >= 1)
            m_PreviewAnimation = 0;
    }

    public void NewSector()
    {
        m_BluePortal.SetActive(false);
        m_OrangePortal.SetActive(false);
        m_CrossHairBlue.SetActive(false);
        m_CrossHairOrange.SetActive(false);
    }
}
