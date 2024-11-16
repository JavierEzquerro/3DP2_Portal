using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeController : MonoBehaviour, IRestartGame
{
    [Header("Sounds")]
    [SerializeField] private AudioClip m_DeathSound;
    [SerializeField] private AudioClip m_DeadZoneSound;

    [SerializeField] private CanvasGroup m_BloodImage;
    [SerializeField] private float m_BloodFadeInDuration;
    private Player_Controller m_PlayerController;
    private Animator m_PlayerAnimator;

    public float m_TimeToKillPlayer;
    public float m_MaxPlayerHealth = 100;
    private float m_Health;
    private float m_DamageTimer = 0f;

    private bool m_Death = false;

    private void OnEnable()
    {
        Turret.OnPlayerDamagedByLaser += ApplyLaserDamage;
    }

    private void OnDisable()
    {
        Turret.OnPlayerDamagedByLaser -= ApplyLaserDamage;
    }

    private void Start()
    {
        GameManager.instance.AddRestartGame(this);
        m_PlayerController = GetComponent<Player_Controller>();
        m_Health = m_MaxPlayerHealth;
        m_PlayerAnimator = GetComponent<Animator>();
    }

    private void ApplyLaserDamage(float l_Damage)
    {
        if (m_Death) return;

        m_DamageTimer += Time.deltaTime;

        float l_Progress = Mathf.Clamp01(m_DamageTimer / m_TimeToKillPlayer);
        m_Health -= l_Damage * Time.deltaTime;

        m_BloodImage.alpha = l_Progress;

        if (l_Progress >= 1f)
        {
            Death();
        }
    }

    public void Death()
    {
        m_BloodImage.alpha = 0.0f;
        GameManager.instance.ReStartGame(false);
    }

    public void KilledByDeadZone()
    {
        SoundsManager.instance.PlaySoundClip(m_DeadZoneSound, transform, 0.2f);
        Death();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("DeadZone"))
        {
            KilledByDeadZone();
        }
    }

    public void RestartGame()
    {
        //m_PlayerAnimator.SetBool("Death", false);
        m_Death = false;
        m_Health = m_MaxPlayerHealth;
        m_DamageTimer = 0f;
        m_BloodImage.alpha = 0.0f;
    }
}
