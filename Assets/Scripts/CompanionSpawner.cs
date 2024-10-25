using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_CompanionPrefab;
    [SerializeField] private Transform m_CompanionSpawnPoint;

    public void Spawn()
    {
        GameObject l_Companion = GameObject.Instantiate(m_CompanionPrefab);
        l_Companion.SetActive(true);
        l_Companion.transform.position = m_CompanionSpawnPoint.position;
        l_Companion.transform.rotation = m_CompanionSpawnPoint.rotation;
    }
}
