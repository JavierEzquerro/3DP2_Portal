using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeButton : MonoBehaviour
{
    [SerializeField] private Animator m_CubeAnimator;

    public UnityEvent m_OnButtonClickedEvent;
    public UnityEvent m_OnButtonDeClickedEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CompanionCube"))
        {
            m_CubeAnimator.SetBool("CubeButtonPressed", true);
            m_OnButtonClickedEvent?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_CubeAnimator.SetBool("CubeButtonPressed", false);
        m_OnButtonDeClickedEvent?.Invoke();
    }
}