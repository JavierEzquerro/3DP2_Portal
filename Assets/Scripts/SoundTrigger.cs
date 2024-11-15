using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public enum TypeOfSound
    {
        Delayed,
        Multiple
    }

    public TypeOfSound m_TypeOfSound;

    [Header("Audios")]
    [SerializeField] private AudioClip[] m_SoundsToPlay;
    [SerializeField] private float m_DelayTime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            if (m_TypeOfSound == TypeOfSound.Delayed)
            {
                StartCoroutine(PlayDelayedSound());
            }
            else if (m_TypeOfSound == TypeOfSound.Multiple)
                StartCoroutine(PlayMultipleSounds());
    }

    private IEnumerator PlayDelayedSound()
    {
        yield return new WaitForSeconds(m_DelayTime);
        SoundsManager.instance.PlaySoundClip(m_SoundsToPlay[0], transform, 0.02f);
        Destroy(gameObject);
    }

    private IEnumerator PlayMultipleSounds()
    {
        foreach (var sound in m_SoundsToPlay)
        {
            SoundsManager.instance.PlaySoundClip(sound, transform, 0.02f);
            yield return new WaitForSeconds(sound.length);
        }
        Destroy(gameObject);
    }
}
