using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeButton : MonoBehaviour, IRestartGame
{
    [SerializeField] private Animator m_CubeAnimator;

    public UnityEvent m_OnButtonClickedEvent;
    public UnityEvent m_OnButtonDeClickedEvent;

    [Header("Sounds")]
    [SerializeField] private AudioClip m_ButtonClickedSound;
    [SerializeField] private AudioClip m_ButtonDeClickedSound;

    private void Start()
    {
        GameManager.instance.AddRestartGame(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CompanionCube"))
        {
            m_CubeAnimator.SetBool("CubeButtonPressed", true);
            SoundsManager.instance.PlaySoundClip(m_ButtonClickedSound, transform, 0.2f);
            m_OnButtonClickedEvent?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CompanionCube"))
        {
            m_CubeAnimator.SetBool("CubeButtonPressed", false);
            SoundsManager.instance.PlaySoundClip(m_ButtonDeClickedSound, transform, 0.2f);
            m_OnButtonDeClickedEvent?.Invoke();
        }
    }

    public void RestartGame()
    {
        m_CubeAnimator.SetBool("CubeButtonPressed", false);
        m_OnButtonDeClickedEvent?.Invoke();
    }
}