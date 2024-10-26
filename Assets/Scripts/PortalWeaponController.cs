using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject m_PreviewPortal; 
    [SerializeField] private Camera m_Camera;
    [SerializeField] private GameObject m_Portal;
    List<Transform> m_ValidPoints = new List<Transform>();
    [SerializeField] private float m_DistanceRay;

    private void Update()
    {
        RaycastHit l_hit;
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(l_Ray, out l_hit))
        {
            Debug.Log("Collision");
            m_PreviewPortal.transform.rotation = Quaternion.LookRotation(-l_hit.normal);
            m_PreviewPortal.transform.position = l_hit.point + Vector3.forward/1000;

            m_PreviewPortal.SetActive(true);
        }
        else
        {
            m_PreviewPortal.SetActive(false);
        } 
    }

    public bool IsValidPosition()
    {
        bool isValid = false;
        


        return isValid;
    }
}
