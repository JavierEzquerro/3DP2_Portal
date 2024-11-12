using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindowCollider : MonoBehaviour
{
    public Portal m_MirrorPortal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            Debug.Log("Clone");
            m_MirrorPortal.m_CloneWeapon.SetActive(true);   
        }
    }
}
