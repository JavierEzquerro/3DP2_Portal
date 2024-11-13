using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private CharacterController m_CharacterController;
    [SerializeField] private GameObject m_GoToPosition;
    // private Animator m_Animator;
    public Transform m_PitchController;
    private float m_Yaw;
    private float m_Pitch;
    private float m_FootstepTimer;
    private float m_JumpDelay = 0.1f;
    private float m_JumpDelayTimer = 0f;
    public bool m_CanMove { get; set; } = true;


    public Camera m_Camera;

    [SerializeField] private float m_YawSpeed;
    [SerializeField] private float m_pitchSpeed;
    [SerializeField] private float m_minPitch;
    [SerializeField] private float m_maxPitch;
    [SerializeField] public float m_Speed;
    [SerializeField] private float m_speedMultiplier;
    [SerializeField] private float m_verticalSpeed;
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] private float m_footstepInterval;

    [Header("Keys")]
    private KeyCode m_LeftKeyCode = KeyCode.A;
    private KeyCode m_RightKeyCode = KeyCode.D;
    private KeyCode m_UpKeyCode = KeyCode.W;
    private KeyCode m_DownKeyCode = KeyCode.S;
    private KeyCode m_LeftShiftCode = KeyCode.LeftShift;
    private KeyCode m_JumpKeyCode = KeyCode.Space;

    [Header("Audio")]
    [SerializeField] private AudioClip m_MetalJumpSound;
    [SerializeField] private AudioClip m_MetalLandingSound;
    private string m_CurrentSurfaceTag;

    [Header("Surfaces")]
    [SerializeField] private float m_MinBounceForce;
    private float m_InitialBounceSpeed;
    private bool m_HasBounced = false;
    public static Action OnPlayerLaunched;

    [Header("Portal")]
    public Vector3 m_MovementDirection; 
    public float m_TeleportOffset;
    private bool m_EnterPortal;
    private Portal m_Portal;

    [Header("ZeroGravity")]
    private ZeroGravity m_ZeroGravity;
    private bool m_GravityZone;
    float m_StartSpeed;
    private Vector3 m_PreviousOffsetFromPortal;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        //m_Animator = GetComponent<Animator>();
        GameManager.instance.SetPlayer(this);
        m_Yaw = transform.eulerAngles.y;
        m_Pitch = m_PitchController.localRotation.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        m_FootstepTimer = 0f;
        m_GravityZone = false;
        m_StartSpeed = m_Speed; 
    }


    void Update()
    {
        // Camera Movement
        float l_Horizontal = Input.GetAxis("Mouse X");
        float l_vertical = -Input.GetAxis("Mouse Y");

        m_Yaw = m_Yaw + l_Horizontal * m_YawSpeed * Time.deltaTime;
        m_Pitch = m_Pitch + l_vertical * m_pitchSpeed * Time.deltaTime;
        m_Pitch = Mathf.Clamp(m_Pitch, m_minPitch, m_maxPitch);

        transform.rotation = Quaternion.Euler(0, m_Yaw, 0);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0, 0);

        float l_forwardAngle = m_Yaw * Mathf.Deg2Rad;
        float l_rightAngle = (m_Yaw + 90.0f) * Mathf.Deg2Rad;

        Vector3 l_forward = new Vector3(Mathf.Sin(l_forwardAngle), 0, Mathf.Cos(l_forwardAngle));
        Vector3 l_right = new Vector3(Mathf.Sin(l_rightAngle), 0, Mathf.Cos(l_rightAngle));

        if (!m_CanMove) return;

        m_MovementDirection = Vector3.zero;

        if (Input.GetKey(m_RightKeyCode))
            m_MovementDirection = l_right;

        else if (Input.GetKey(m_LeftKeyCode))
            m_MovementDirection = -l_right;

        if (Input.GetKey(m_UpKeyCode))
            m_MovementDirection += l_forward;

        else if (Input.GetKey(m_DownKeyCode))
            m_MovementDirection -= l_forward;

        m_MovementDirection.Normalize();

        if (m_JumpDelayTimer > 0)
        {
            m_JumpDelayTimer -= Time.deltaTime;
        }

        DetectSurface();

        if (m_CharacterController.isGrounded && Input.GetKey(m_JumpKeyCode) && m_JumpDelayTimer <= 0f)
        {
            m_verticalSpeed = m_JumpSpeed;
            //SoundsManager.instance.PlaySoundClip(m_MetalJumpSound, transform, 0.2f);
            m_JumpDelayTimer = m_JumpDelay;
        }

        float l_speedMultiplier = 1.0f;

        /*
        if (Input.GetKey(m_LeftShiftCode))
            l_speedMultiplier = m_speedMultiplier;
         */

        Vector3 l_MovementDirection = m_MovementDirection * m_Speed * l_speedMultiplier * Time.deltaTime;

        if (m_GravityZone)
        {
            Debug.Log("ZERO Gravity"); 
            m_verticalSpeed = 0;
            l_MovementDirection.y = 0;
            m_Speed = m_StartSpeed/4;
            l_MovementDirection += m_ZeroGravity.m_Direction * m_ZeroGravity.m_Speed * Time.deltaTime;    
        }
        else
        {
            m_Speed = m_StartSpeed;
            m_verticalSpeed += Physics.gravity.y * Time.deltaTime;
            l_MovementDirection.y = m_verticalSpeed * Time.deltaTime;
        }
        
        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_MovementDirection);

        if ((l_CollisionFlags & CollisionFlags.Below) != 0 && !m_GravityZone)
        {
            m_verticalSpeed = 0;
            l_speedMultiplier = 1;
        }

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_verticalSpeed > 0.0f && !m_GravityZone)
            m_verticalSpeed = 0;


        if (m_CharacterController.velocity.magnitude > 0.01f && m_CharacterController.isGrounded)
        {
            // m_Animator.SetBool("Walking", true);
            HandleFootstepSound();
        }
        //else
        //m_Animator.SetBool("Walking", false);


        if (m_EnterPortal)
        {
            Vector3 l_Offset = m_Portal.transform.position - transform.position;
        
             
            if (Vector3.Dot(m_Portal.transform.forward, l_Offset.normalized) >= -0.054 && Vector3.Dot(transform.forward, m_Portal.transform.forward) <= -0.75f)
            {
                Teleport(m_Portal);
                m_EnterPortal = false;
            }
        }
    }



    private void DetectSurface()
    {
        Vector3 l_FeetPosition = m_CharacterController.bounds.center - new Vector3(0, m_CharacterController.bounds.extents.y, 0);

        Ray ray = new Ray(l_FeetPosition, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.5f))
            m_CurrentSurfaceTag = hit.collider.tag;
        else
            m_CurrentSurfaceTag = "Untagged";
    }

    private void HandleFootstepSound()
    {
        m_FootstepTimer -= Time.deltaTime;

        if (m_FootstepTimer <= 0f)
        {
            switch (m_CurrentSurfaceTag)
            {
                case "Metal":
                    SoundsManager.instance.PlayFootstepSound(transform, 0.4f, SoundsManager.SurfaceType.Metal);
                    break;
                case "Rock":
                    SoundsManager.instance.PlayFootstepSound(transform, 0.4f, SoundsManager.SurfaceType.Rock);
                    break;
                default:
                    SoundsManager.instance.PlayFootstepSound(transform, 0.4f, SoundsManager.SurfaceType.Default);
                    break;
            }

            m_FootstepTimer = m_footstepInterval;
        }
    }

    public void SetSpeed(float l_speed)
    {
        m_Speed = l_speed;
    }

    public float GetSpeed() { return m_speed; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            m_EnterPortal = true;
            m_Portal = other.GetComponent<Portal>();
            Physics.IgnoreCollision(m_CharacterController, m_Portal.m_WallPortaled, true); 
            m_PreviousOffsetFromPortal = m_Portal.transform.position - transform.position;
        }

        if (other.CompareTag("GravityZero"))
        {
            m_GravityZone = true;
            m_ZeroGravity = other.GetComponent<ZeroGravity>();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            m_EnterPortal = false;
            m_Portal = other.GetComponent<Portal>();
            Physics.IgnoreCollision(m_CharacterController, m_Portal.m_WallPortaled, false);
        }
        
        if (other.CompareTag("GravityZero"))
        {
            m_ZeroGravity = other.GetComponent<ZeroGravity>(); 
            m_GravityZone = false;
        }
    }

    private void Teleport(Portal l_portal)
    {
        Vector3 l_Position = transform.position + m_MovementDirection * m_TeleportOffset; //Obtener Posicion en mundo
        Vector3 l_LocalPosition = l_portal.m_OtherPortalTransform.InverseTransformPoint(l_Position); //Pasar de Posicion mundo a local
        Vector3 l_WorldPosition = l_portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition); //Convertir la local al otro portal. 

        Vector3 l_Forward = m_MovementDirection; 
        Vector3 l_LocalForward = l_portal.m_OtherPortalTransform.InverseTransformDirection(l_Forward);
        Vector3 l_WorldForward = l_portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);

        Physics.IgnoreCollision(m_CharacterController, m_Portal.m_WallPortaled, false);

        m_CharacterController.enabled = false;   
        transform.position = l_WorldPosition; 
        transform.forward = l_WorldForward; 
        m_Yaw = transform.eulerAngles.y;
        m_CharacterController.enabled = true;
        Debug.Log("Teleport"); 

        m_CharacterController.enabled = false;
        transform.position = l_WorldPosition;
        transform.forward = l_WorldForward;
        m_Yaw = transform.eulerAngles.y;
        m_CharacterController.enabled = true;
    }

    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.collider.CompareTag("LaunchingSurface"))
        {
            OnPlayerLaunched?.Invoke();
        }
        else if (collision.collider.CompareTag("BouncingSurface"))
        {
            if (!m_HasBounced)
            {
                m_InitialBounceSpeed = Mathf.Max(Mathf.Abs(m_verticalSpeed), m_MinBounceForce);
                m_HasBounced = true;
            }

            m_verticalSpeed = m_InitialBounceSpeed;
        }
        else
        {
            m_HasBounced = false;
        }
    }
}

