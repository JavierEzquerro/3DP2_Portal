using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

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
    [SerializeField] private TMP_Text m_TextCanvas;
    [SerializeField] private TMP_Text m_NewGameText;
    //[SerializeField] private FadeController m_FadeController;
    [SerializeField] private int m_MaxBulletEnemyOnScene;

    public List<IRestartGame> m_RestartGames = new List<IRestartGame>();
    //private List<Enemies> m_ListEnemies = new List<Enemies>();


    public Vector3 m_direction;
    private float m_SpeedPlayer;
    public bool m_Restart;

    private bool m_GameHasEnded = false;
    private List<Item> m_ItemListDestroy = new List<Item>();
    private Player_Controller m_Player;

    private void Awake()
    {
        instance = this;
    }
    
    public void SetPlayer(Player_Controller l_player)
    {
        m_Player = l_player;
    }

    public Player_Controller GetPlayer()
    {
        return m_Player;
    }

    /*
        public void AddRestartGame(IRestartGame l_restart)
        {
            m_RestartGames.Add(l_restart);
        }

        public void AddEnemies(Enemies l_Enemies)
        {
            m_ListEnemies.Add(l_Enemies);
        }

        public void ReStartGame(bool l_EndGame)
        {
            if (l_EndGame)
            {
                m_Player.StopAnimation();
                m_TextCanvas.text = "You Win";
                m_NewGameText.text = "Menu";
                m_GameHasEnded = true;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            m_CharacterController.enabled = false;
            m_PlayerController.enabled = false;
            m_WeaponController.enabled = false;
            m_GameUI.SetActive(false);
            m_DeathUI.SetActive(true);
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

        public void RestartPosition()
        {
            if (m_GameHasEnded == false)
            {
                m_GameUI.SetActive(true);
                m_DeathUI.SetActive(false);

                m_Restart = true;

                foreach (Enemies l_enemy in m_ListEnemies)
                {
                    l_enemy.gameObject.SetActive(true);
                }

                foreach (IRestartGame l_controller in m_RestartGames)
                {
                    l_controller.RestartGame();
                }

                Cursor.lockState = CursorLockMode.Locked;
                StartCoroutine(PlayerActive());
            }
        }
        public void AddItemToDestroy(Item l_Item)
        {
            m_ItemListDestroy.Add(l_Item);
        }

        private IEnumerator PlayerActive()
        {
            yield return new WaitForSeconds(0.1f);
            m_CharacterController.enabled = true;
            m_PlayerController.enabled = true;
            m_PlayerController.SetSpeed(0);
            m_Restart = false;
        }

        public void ActivePlayer()
        {
            m_PlayerController.SetSpeed(m_Player.m_StartSpeed);
            m_WeaponController.enabled = true;
        }
     
     */

    public void ExitGame()
    {
        Application.Quit();
    }
}

