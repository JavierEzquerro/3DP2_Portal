using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator m_DoorAnimator;

    public enum DoorType
    {
        TriggerDoor,
        ButtonDoor
    }

    public DoorType m_DoorType;

    public bool m_OpenDoor { get; set; }


    private void Update()
    {
        HandleButtonDoor();
    }

    private void HandleButtonDoor()
    {
        if (m_OpenDoor == true && m_DoorType == DoorType.ButtonDoor)
        {
            m_DoorAnimator.SetBool("OpenDoor", true);
            m_DoorAnimator.SetBool("CloseDoor", false);
        }
        else
        {
            m_DoorAnimator.SetBool("OpenDoor", false);
            m_DoorAnimator.SetBool("CloseDoor", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && m_DoorType == DoorType.TriggerDoor)
        {
            m_DoorAnimator.SetBool("OpenDoor", true);
            m_DoorAnimator.SetBool("CloseDoor", false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && m_DoorType == DoorType.TriggerDoor)
        {
            m_DoorAnimator.SetBool("OpenDoor", false);
            m_DoorAnimator.SetBool("CloseDoor", true);
        }
    }
}
