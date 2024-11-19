using UnityEngine;
using UnityEngine.Events;

public class LaserReceiver : MonoBehaviour
{
    public UnityEvent m_OnLaserReceived;

    private void OnEnable()
    {
        Turret.OnLaserReceived += LaserReceived;
        RefractionCube.OnLaserReceived += LaserReceived;
        Portal.OnLaserReceived += LaserReceived;
    }

    private void OnDisable()
    {
        Turret.OnLaserReceived -= LaserReceived;
        RefractionCube.OnLaserReceived -= LaserReceived;
        Portal.OnLaserReceived -= LaserReceived;
    }

    private void Start()
    {
        gameObject.SetActive(true);
    }

    private void LaserReceived()
    {
        m_OnLaserReceived?.Invoke();
    }
}
