using UnityEngine;
using UnityEngine.Events;

public class LaserReceiver : MonoBehaviour
{
    public UnityEvent m_OnLaserReceived;

    private void OnEnable()
    {
        Turret.OnLaserReceived += LaserReceived;
        LaserEmitter.OnLaserReceived += LaserReceived;
    }

    private void OnDisable()
    {
        Turret.OnLaserReceived -= LaserReceived;
        LaserEmitter.OnLaserReceived -= LaserReceived;
    }

    private void LaserReceived()
    {
        m_OnLaserReceived?.Invoke();
    }
}
