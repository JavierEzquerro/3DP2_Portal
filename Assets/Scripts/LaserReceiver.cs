using UnityEngine;
using UnityEngine.Events;

public class LaserReceiver : MonoBehaviour
{
    public UnityEvent m_OnLaserReceived;

    private void OnEnable()
    {
        Torret.OnLaserReceived += LaserReceived;
    }

    private void OnDisable()
    {
        Torret.OnLaserReceived -= LaserReceived;
    }

    private void LaserReceived()
    {
        m_OnLaserReceived?.Invoke();
    }
}
