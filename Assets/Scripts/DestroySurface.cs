using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DestroySurface : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CompanionCube"))
        {
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Turret"))
        {
            Turret l_Turret = other.GetComponent<Turret>();
            StartCoroutine(l_Turret.TurretDeathCoroutine(l_Turret, other.gameObject));
        }
    }
}
