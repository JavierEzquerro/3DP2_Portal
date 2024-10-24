using UnityEngine;

public class Torret : MonoBehaviour
{
    public LineRenderer m_LaserRenderer;
    public LayerMask m_LayerMask;
    public float m_MaxDistance = 50.0f;

    void Update()
    {
        Ray l_Ray = new Ray(m_LaserRenderer.transform.position, m_LaserRenderer.transform.forward);

        if (Physics.Raycast(l_Ray, out RaycastHit l_HitInfo, m_MaxDistance, m_LayerMask.value))
        {
            float l_Distance = l_HitInfo.distance;
            m_LaserRenderer.SetPosition(1, new Vector3(0, 0, l_HitInfo.distance));
            m_LaserRenderer.gameObject.SetActive(true);
            if (l_HitInfo.collider.CompareTag("RefractionCube"))
            {
                //Reflect ray
                l_HitInfo.collider.GetComponent<RefractionCube>().CreateRefraction();
            }
        }
        else
            m_LaserRenderer.gameObject.SetActive(false);

    }
}
