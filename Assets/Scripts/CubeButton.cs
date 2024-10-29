using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeButton : MonoBehaviour
{
    [SerializeField] private Animator m_CubeAnimator;

    public UnityEvent m_Event;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CompanionCube"))
        {
            m_CubeAnimator.SetBool("CubeButtonPressed", true);
            m_Event?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_CubeAnimator.SetBool("CubeButtonPressed", false);
    }
}
