using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PortalWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject m_PreviewPortal; 
    [SerializeField] private Camera m_Camera;
    [SerializeField] private GameObject m_BluePortal;
    [SerializeField] private GameObject m_OrangePortal;
    [SerializeField] private float m_DistanceRay; 
    [SerializeField] private float m_ThresholdPortal;
    [SerializeField] private float m_ForceLaunch;
    [SerializeField] private float m_DistanceStopLerpAttract;
    [SerializeField] private GameObject m_CrossHairBlue;
    [SerializeField] private GameObject m_CrossHairOrange;

    public List<Transform> m_ValidPoints = new List<Transform>();
    public Transform m_AttractPoint; 
    private bool m_AttractingObjects;
    private bool m_TrapedObject;
    private GameObject m_ObjectAttract;
    private Rigidbody m_RbObjectAttract;
    private BoxCollider m_ObjectCollider; 
    public float m_AttractSpeed;
    private float m_ReSize;
    private Vector3 m_StartScale;
    private float m_CurrentPortalSize; 

    private void Start()
    {
        m_BluePortal.SetActive(false); 
        m_OrangePortal.SetActive(false);
        m_AttractingObjects = false;
        m_TrapedObject = false;
        m_ReSize = 1.0f;
        m_StartScale = transform.localScale;    
        m_CurrentPortalSize = m_ReSize; 
        m_CrossHairBlue.SetActive(false);
        m_CrossHairOrange.SetActive(false); 
    }

    private void Update()
    {
        m_ReSize = Mathf.Clamp(m_ReSize, 0.5f, 2.0f);   

        RaycastHit l_hit;
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(l_Ray.origin,l_Ray.direction, out l_hit, m_DistanceRay))
        {
            if (!m_AttractingObjects && !m_TrapedObject)
            {
                //PORTAL
                m_PreviewPortal.transform.rotation = Quaternion.LookRotation(l_hit.normal);
                m_PreviewPortal.transform.position = l_hit.point;

                if (IsValidPosition())
                {
                    //PREVIEW
                    m_PreviewPortal.SetActive(true);

                    if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                    {
                        m_PreviewPortal.transform.rotation = Quaternion.LookRotation(l_hit.normal);
                        m_PreviewPortal.transform.position = l_hit.point;

                        float l_ScrollWheel = Input.GetAxis("Mouse ScrollWheel");

                        if (l_ScrollWheel > 0)
                        {
                            m_ReSize += 0.1f + Time.deltaTime;
                        }
                        else if (l_ScrollWheel < 0)
                        {
                            m_ReSize -= 0.1f + Time.deltaTime;
                        }

                        if(m_ReSize != m_CurrentPortalSize)
                        {
                            m_PreviewPortal.transform.localScale = m_StartScale * m_ReSize;
                            m_CurrentPortalSize = m_ReSize;
                        }
                    }
                    else
                    {
                        m_PreviewPortal.SetActive(false);
                    }

                    if(Input.GetMouseButtonUp(0))
                        ShootPortalBlue(l_hit);

                    if (Input.GetMouseButtonUp(1))
                        ShootPortalOrange(l_hit);
                }
                else
                {
                    m_PreviewPortal.SetActive(false);
                }
            }
            
            //ATTRACT OBJECTS
            if (l_hit.collider.CompareTag("Cube") || l_hit.collider.CompareTag("Turret"))
            {
                if(Input.GetMouseButtonDown(0) && !m_TrapedObject && !m_AttractingObjects)
                {
                    AttractObject(l_hit); 
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && m_TrapedObject)
        {
            m_TrapedObject = false;
            m_RbObjectAttract.useGravity = true;
            m_ObjectCollider.enabled = true;
            m_RbObjectAttract.AddForce(m_Camera.transform.forward * m_ForceLaunch);  

        }
        else if (Input.GetMouseButtonDown(1) && m_TrapedObject)
        {
            m_TrapedObject = false;
            m_RbObjectAttract.useGravity = true;
            m_ObjectCollider.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (m_AttractingObjects)
        {
            m_ObjectAttract.transform.position = Vector3.Lerp(m_ObjectAttract.transform.position, m_AttractPoint.transform.position, m_AttractSpeed * Time.fixedDeltaTime);

            if (Vector3.Distance(m_ObjectAttract.transform.position, m_AttractPoint.transform.position) <= m_DistanceStopLerpAttract)
            {
                m_ObjectAttract.transform.position = m_AttractPoint.transform.position;
                m_AttractingObjects = false;
                m_TrapedObject = true;
            }
        }


        if (m_TrapedObject)
        {
            //m_ObjectAttract.transform.position = m_AttractPoint.transform.position;
            m_ObjectAttract.transform.position = Vector3.Lerp(m_ObjectAttract.transform.position, m_AttractPoint.transform.position, m_AttractSpeed * Time.fixedDeltaTime);
            m_ObjectAttract.transform.forward = Vector3.Lerp(m_ObjectAttract.transform.forward, transform.forward, m_AttractSpeed * Time.fixedDeltaTime);        

        }
    }

    private void AttractObject(RaycastHit l_hit)
    {
        m_ObjectAttract = l_hit.collider.gameObject;
        m_RbObjectAttract = m_ObjectAttract.GetComponent<Rigidbody>();
        m_ObjectCollider = m_RbObjectAttract.GetComponent<BoxCollider>();

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

    private void ShootPortalBlue(RaycastHit l_hit)
    {
        m_BluePortal.SetActive(true);
        m_CrossHairBlue.SetActive(true);    
        m_BluePortal.transform.localScale = m_PreviewPortal.transform.localScale;    
        m_BluePortal.transform.rotation = Quaternion.LookRotation(l_hit.normal);
        m_BluePortal.transform.position = l_hit.point;
    }

    private void ShootPortalOrange(RaycastHit l_hit)
    {
        m_CrossHairOrange.SetActive(true);  
        m_OrangePortal.SetActive(true);
        m_OrangePortal.transform.localScale = m_PreviewPortal.transform.localScale; 
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

            if (Physics.Raycast(l_CameraPosition,l_Diretion, out l_hit))
            {
                float l_Angle = Vector3.Angle(l_hit.normal, m_ValidPoints[i].forward);

                if (Vector3.Distance(m_ValidPoints[i].transform.position, l_hit.point) >= m_ThresholdPortal || !l_hit.collider.CompareTag("WhiteWall") || l_Angle <= 178.0f)
                {
                    return false;
                }
            }
            else
                return false;
        }
        return isValid;
    }

    public void NewSector()
    {
        m_BluePortal.SetActive(false);
        m_OrangePortal.SetActive(false);
        m_CrossHairBlue.SetActive(false);
        m_CrossHairOrange.SetActive(false);
    }
}
