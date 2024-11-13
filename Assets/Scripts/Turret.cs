using UnityEngine;

public class Torret : MonoBehaviour
{
    public LineRenderer m_LaserRenderer;
    public LayerMask m_LayerMask;
    public float m_MaxDistance = 50.0f;
    [SerializeField] private float m_MaxAngleLaserAlive = 10.0f;
    private Portal m_Portal;

    void Update()
    {
        if (IsLaserAlive())
        {
            Ray l_Ray = new Ray(m_LaserRenderer.transform.position, m_LaserRenderer.transform.forward);

            if (Physics.Raycast(l_Ray, out RaycastHit l_HitInfo, m_MaxDistance, m_LayerMask.value))
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

                Destroy(l_HitInfo.collider.gameObject);

            }
            else if (l_HitInfo.collider.CompareTag("Portal"))
            {
                m_Portal = l_HitInfo.collider.GetComponent<Portal>();
                m_LaserRenderer.SetPosition(1, new Vector3(0, 0, l_HitInfo.distance + Vector3.Distance(l_HitInfo.point, m_Portal.transform.position) + 0.5f));
                m_Portal.RayReflection(l_Ray, l_HitInfo);
            }
            else
            {
                m_LaserRenderer.gameObject.SetActive(false);
            }
        }
        else
            m_LaserRenderer.gameObject.SetActive(false);
    }

    private bool IsLaserAlive()
    {
        return Vector3.Dot(transform.up, Vector3.up) > Mathf.Cos(m_MaxAngleLaserAlive * Mathf.Deg2Rad);
    }
}
