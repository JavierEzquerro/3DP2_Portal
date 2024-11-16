using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface IRestartGame
{
    void RestartGame();
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject m_DeathUI;
    [SerializeField] private GameObject m_GameUI;
    [SerializeField] private CharacterController m_CharacterController;
    [SerializeField] private Player_Controller m_PlayerController;
    [SerializeField] private PortalWeaponController m_PortalWeaponController;
    [SerializeField] private TMP_Text m_WinDeathText;
    [SerializeField] private TMP_Text m_NewGameButtonText;
    [SerializeField] private FadeController m_FadeController;

    public List<IRestartGame> m_RestartGame = new List<IRestartGame>();

    private List<Turret> m_TurretsToRestart = new List<Turret>();

    public bool m_Restart;
    private bool m_GameHasEnded = false;

    private void Awake()
    {
        instance = this;
    }

    public void SetPlayer(Player_Controller l_Player)
    {
        m_PlayerController = l_Player;
    }

    public Player_Controller GetPlayer()
    {
        return m_PlayerController;
    }

    public void AddRestartGame(IRestartGame l_Restart)
    {
        m_RestartGame.Add(l_Restart);
    }

    public void AddTurretToRestart(Turret l_Turret)
    {
        m_TurretsToRestart.Add(l_Turret);
    }

    public void ReStartGame(bool l_EndGame)
    {
        if (l_EndGame)
        {
            //m_Player.StopAnimation();
            m_WinDeathText.text = "You Win";
            m_NewGameButtonText.text = "Menu";
            m_GameHasEnded = true;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        m_CharacterController.enabled = false;
        m_PlayerController.enabled = false;
        m_PortalWeaponController.enabled = false;
        m_GameUI.SetActive(false);
        m_DeathUI.SetActive(true);
    }

    public void RestartPosition()
    {
        if (m_GameHasEnded == false)
        {
            m_GameUI.SetActive(true);
            m_DeathUI.SetActive(false);

            m_Restart = true;

            foreach (IRestartGame l_Controller in m_RestartGame)
            {
                l_Controller.RestartGame();
            }

            foreach (Turret l_Turret in m_TurretsToRestart)
            {
                l_Turret.RestartGame();
            }

            Cursor.lockState = CursorLockMode.Locked;
            StartCoroutine(PlayerActive());
        }
    }

    private IEnumerator PlayerActive()
    {
        yield return new WaitForSeconds(0.1f);
        m_CharacterController.enabled = true;
        m_PlayerController.enabled = true;
        m_PortalWeaponController.enabled = true;
        m_PlayerController.SetSpeed();
        m_Restart = false;
    }

    public void NewGame()
    {
        StartCoroutine(NewGameCoroutine());
    }

    public IEnumerator NewGameCoroutine()
    {
        if (m_GameHasEnded == true)
        {
            m_FadeController.StartFade();
            yield return new WaitForSeconds(2.0f);
            m_GameHasEnded = false;
            SceneManager.LoadSceneAsync("MainMenu");
        }

        m_FadeController.StartFade();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

