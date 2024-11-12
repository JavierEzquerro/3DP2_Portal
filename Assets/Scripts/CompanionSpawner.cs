using System.Collections;
using TMPro;
using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_CompanionPrefab;
    [SerializeField] private Transform m_CompanionSpawnPoint;
    [SerializeField] private TMP_Text m_ButtonText;

    private bool isShowingText = false;
    private bool playerInTrigger = false;

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Spawn();
        }
    }

    private IEnumerator ShowTextCoroutine()
    {
        isShowingText = true;
        m_ButtonText.enabled = true;
        m_ButtonText.text = "Press E to spawn a cube";
        yield return new WaitForSeconds(2.0f);
        m_ButtonText.enabled = false;
        isShowingText = false;
    }

    public void Spawn()
    {
        GameObject companion = Instantiate(m_CompanionPrefab, m_CompanionSpawnPoint.position, m_CompanionSpawnPoint.rotation);
        companion.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;

            StartCoroutine(ShowTextCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            m_ButtonText.enabled = false;
            isShowingText = false;
        }
    }
}

/*Ray ray = m_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
RaycastHit hit;

if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("SpawnerButton"))
{
    if (isShowingText == false)
        StartCoroutine(ShowTextCoroutine());

    if (Input.GetKeyDown(KeyCode.E))
        Spawn();
}
else
{
    m_ButtonText.enabled = false;
    isShowingText = false;
}*/
