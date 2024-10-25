using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private CharacterController m_CharacterController;
   // private Animator m_Animator;
    public Transform m_PitchController;
    private float m_Yaw;
    private float m_Pitch;
    private float m_FootstepTimer;
    private float m_JumpDelay = 0.1f;
    private float m_JumpDelayTimer = 0f;

    public Camera m_Camera;

    [SerializeField] private float m_YawSpeed;
    [SerializeField] private float m_pitchSpeed;
    [SerializeField] private float m_minPitch;
    [SerializeField] private float m_maxPitch;
    [SerializeField] public float m_speed;
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
    private string m_CurrentSurfaceTag;
    [SerializeField] private AudioClip m_MetalJumpSound;
    [SerializeField] private AudioClip m_MetalLandingSound;

    [Header("Portal")]
    Vector3 m_MovementDirection; 
    public float m_TeleportOffset; 

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

        m_MovementDirection = Vector3.zero;

        if (Input.GetKey(m_RightKeyCode))
            m_MovementDirection = l_right;

        else if (Input.GetKey(m_LeftKeyCode))
            m_MovementDirection = -l_right;

        if (Input.GetKey(m_UpKeyCode))
            m_MovementDirection += l_forward;

        else if (Input.GetKey(m_DownKeyCode))
            m_MovementDirection -= l_forward;

        m_MovementDirection = m_MovementDirection.normalized;

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

        m_verticalSpeed += Physics.gravity.y * Time.deltaTime;

        float l_speedMultiplier = 1.0f;
        if (Input.GetKey(m_LeftShiftCode))
            l_speedMultiplier = m_speedMultiplier;

        m_MovementDirection *= m_speed * l_speedMultiplier * Time.deltaTime;
        m_MovementDirection.y = m_verticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(m_MovementDirection);

        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_verticalSpeed = 0;
            l_speedMultiplier = 1;
        }

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_verticalSpeed > 0.0f)
            m_verticalSpeed = 0;

        m_CharacterController.Move(m_MovementDirection);

        if (m_CharacterController.velocity.magnitude > 0.01f)
        {
           // m_Animator.SetBool("Walking", true);
            HandleFootstepSound();
        }
        //else
        //m_Animator.SetBool("Walking", false);
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
                    //SoundsManager.instance.PlayFootstepSound(transform, 0.4f, SoundsManager.SurfaceType.Metal);
                    break;
                case "Mud":
                    //SoundsManager.instance.PlayFootstepSound(transform, 0.4f, SoundsManager.SurfaceType.Mud);
                    break;
                default:
                    //SoundsManager.instance.PlayFootstepSound(transform, 0.4f, SoundsManager.SurfaceType.Default);
                    break;
            }

            m_FootstepTimer = m_footstepInterval;
        }
    }

    public void SetSpeed(float l_speed)
    {
        m_speed = l_speed;
    }

    public float GetSpeed() { return m_speed; } 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            Portal l_portal; 
            Teleport(l_portal = other.GetComponent<Portal>()); 
        }
    }

    private void Teleport(Portal l_portal)
    {
        m_MovementDirection.Normalize();
        Debug.Log(m_MovementDirection);
        Vector3 l_Position = transform.position + m_MovementDirection * m_TeleportOffset;
        Vector3 l_LocalPosition = l_portal.m_OtherPortalTransform.InverseTransformPoint(l_Position);
        Vector3 l_WorldPosition = l_portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        Vector3 l_Forward = m_MovementDirection; 
        Vector3 l_LocalForward = l_portal.m_OtherPortalTransform.InverseTransformDirection(m_MovementDirection);
        Vector3 l_WorldForward = l_portal.m_MirrorPortal.transform.TransformDirection(l_LocalForward);
      
        m_CharacterController.enabled = false;   
        transform.position = l_WorldPosition; 
        transform.forward = l_WorldForward; 
        m_Yaw = transform.eulerAngles.y;
        m_CharacterController.enabled = true; 
    }
}

