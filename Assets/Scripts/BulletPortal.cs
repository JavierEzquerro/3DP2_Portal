using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPortal : MonoBehaviour
{
    [SerializeField]
    private float m_LerpSpeed;

    [SerializeField]
    private float m_SizeBulletIncrement; 

    private float m_LerpIncrement = 0;
    private Vector3 m_FinalSize = new Vector3(0.1f, 0.1f, 0.1f); 

    void Start()
    {
        transform.localScale = Vector3.zero;
        m_FinalSize *= m_LerpIncrement; 
    }
    
    void Update()
    {
        m_LerpIncrement += Time.deltaTime *  m_LerpSpeed;
        transform.localScale = Vector3.Lerp(transform.localScale, m_FinalSize, m_LerpIncrement); 
    }
}
