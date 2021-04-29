using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]

public class PlayerController : NetworkBehaviour
{
    Rigidbody rb;
    PlayerInputs playerInputs;

    [SerializeField, Range(0.1f, 10f)] float moveSpeed = 5f;

    [SerializeField] Transform camTrs;
    [SerializeField, Range(0.1f, 180f)] float camRotSpeed = 90f;
    [SerializeField] float xClamp = 85f;
    float xRotation = 0f;
    float camRotationAmounthY;

    [SerializeField, Range(0.1f, 15f)] float jumpForce = 5f;
    [SerializeField, Range(0.1f, 10f)] float rayLength = 1f;
    [SerializeField] Color rayColor = Color.magenta;
    [SerializeField] LayerMask detectionLayer;
    [SerializeField]Vector3 rayPosition;

    [SerializeField, Range(1f, 5f)] float augmentedFactor = 1f;
    float augmentedSpeed = 1f;
    float baseSpeed = 1f;

    [SerializeField] List<Weapon> weapons;
    int weaponIndex = 0;
    AudioSource audioSource;
    [SerializeField] AudioClip walkFootStepSFX;
    [SerializeField] AudioClip runFootStepSFX;
    [SerializeField, Range(-2f, 2f)] float walkFootStepPitch = 1;
    [SerializeField, Range(-2f, 2f)] float runFootStepPitch = 1;
    [SerializeField] AudioClip jumpoSFX;
    AudioClip WeaponsShotSFX;
    [SerializeField] AudioClip WeaponsChangeSFX;
    [SerializeField] NetworkVariableFloat health = new NetworkVariableFloat(20f);
    [SerializeField] Slider sldHealth;
    [SerializeField] Button btnHealthTest;

    public override void NetworkStart()
    {
        base.NetworkStart();
        btnHealthTest.onClick.AddListener(()=>{
            if(IsLocalPlayer)
            {
                health.Value--;
            }
        });
        health.OnValueChanged += (float oldValue, float newValue)=>{
            if(IsOwner && IsClient)
            {
                sldHealth.value = health.Value;
            }
            else
            {
                sldHealth.gameObject.SetActive(false);
                btnHealthTest.gameObject.SetActive(false);
            }
        };
        /* foreach(MLAPI.Connection.NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            Debug.Log(client.PlayerObject.name);
        } */
        /* NetworkManager.Singleton.OnClientConnectedCallback += client =>{
            Debug.Log(client);
        }; */
        NetworkObject.name = Gamemanager.instance.currentUsername;
        Debug.Log(NetworkObject.name);
        //NetworkManager.Singleton.LocalClientId;
    }

    void Awake() {
        rb ??= GetComponent<Rigidbody>();
        playerInputs ??= new PlayerInputs();
        audioSource ??= GetComponent<AudioSource>();
    }

    void OnEnable() {
        playerInputs?.Enable();
    }

    void OnDisable() {
        playerInputs?.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(IsLocalPlayer)
        {
            //Cursor.lockState = CursorLockMode.Locked;
            playerInputs.Gameplay.Jump.performed += _ => Jump();
            playerInputs.Gameplay.Run.performed += _ => Run();
            playerInputs.Gameplay.Run.canceled += _ => CancelRun();
            playerInputs.Gameplay.Shoot.performed += _ => Shoot();
            playerInputs.Gameplay.Movement.performed += _ => Movement();
            playerInputs.Gameplay.Movement.canceled += _ => CancelMovement();
        }else
        {
            camTrs.gameObject.SetActive(false);
        }
    }

    void Shoot()
    {
        CurrentWeapon.Shoot();
        WeaponsShotSFX = CurrentWeapon.shootSFX;
        audioSource.PlayOneShot(WeaponsShotSFX);
    }

    void Run()
    {
        augmentedSpeed = augmentedFactor;

        if(!Grounding) return;
        audioSource.clip = runFootStepSFX;
        audioSource.loop = true;
        audioSource.pitch = runFootStepPitch;
        audioSource?.Play();
    }

    void CancelRun()
    {
        augmentedSpeed = baseSpeed;
        audioSource?.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.pitch = 1f;
        if(Axis != Vector2.zero)
        {
            audioSource.clip = walkFootStepSFX;
            audioSource.loop = true;
            audioSource.pitch = walkFootStepPitch;
            audioSource?.Play();
        }
    }

    void Movement()
    {
        if(audioSource.isPlaying || !Grounding) return;
        audioSource.clip = walkFootStepSFX;
        audioSource.loop = true;
        audioSource.pitch = walkFootStepPitch;
        audioSource.Play();
    }

    void CancelMovement()
    {
        audioSource?.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsLocalPlayer) return;
        //if(!NetworkManager.Singleton.IsHost) return;
        camRotationAmounthY += CamAxis.x * camRotSpeed * Time.deltaTime;
        rb.rotation = Quaternion.Euler(rb.rotation.x, camRotationAmounthY, rb.rotation.z);
        rb.position += Forward *moveSpeed * augmentedSpeed * Time.deltaTime;

        //restriccion de la camara en Y
        xRotation -= CamAxis.y * camRotSpeed * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        camTrs.eulerAngles = targetRotation;

        if(WheelAxisYClamped != 0f)
        {
            //audio fx inicio
            audioSource.PlayOneShot(WeaponsChangeSFX);
            //audio fx fin
            CurrentWeapon.Active(false);
            //inicio cambio de arma
            /*if(WheelAxisYClampInt + weaponIndex >= 0 && WheelAxisYClampInt + weaponIndex < weapons.Count)
            {
                weaponIndex += WheelAxisYClampInt;
            }
            else if()*/
           /* weaponIndex += WheelAxisYClampInt + weaponIndex >= 0 && 
            WheelAxisYClampInt + weaponIndex < weapons.Count ? 
            WheelAxisYClampInt + weaponIndex == weapons.Count ?
            0:  WheelAxisYClampInt : weapons.Count - 1;
            Debug.Log(weaponIndex);*/
            if(WheelAxisYClampInt + weaponIndex >= 0)
            {
                if(WheelAxisYClampInt + weaponIndex < weapons.Count)
                {
                    weaponIndex += WheelAxisYClampInt;
                }
                else
                {
                    if(WheelAxisYClampInt + weaponIndex >= weapons.Count)
                    {
                        weaponIndex = 0;
                    }
                }
            }
            else
            {
                weaponIndex = weapons.Count - 1;
            }
            //fin cambio de arma
            CurrentWeapon.Active(true);

            Debug.Log(weaponIndex);
        }
    }

    void Jump()
    {
        if(!Grounding) return;
        //audioSource.clip = jumpoSFX;
        audioSource?.Stop();
        audioSource.PlayOneShot(jumpoSFX, 7f);
        rb.AddForce(JumpDirection, ForceMode.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = rayColor;
        Gizmos.DrawRay(RelativeRayPosition, -transform.up * rayLength);
    }

    bool Grounding => Physics.Raycast(RelativeRayPosition, -transform.up, rayLength, detectionLayer);

    Vector2 Axis => playerInputs.Gameplay.Movement.ReadValue<Vector2>();

    float CamAxisX => playerInputs.Gameplay.CamAxisX.ReadValue<float>();
    float CamAxisY => playerInputs.Gameplay.CamAxisY.ReadValue<float>();

    Vector2 CamAxis => new Vector2(CamAxisX, CamAxisY);
    Vector3 MovementAxis => new Vector3(Axis.x, 0f, Axis.y);
    Vector3 Forward => rb.rotation * MovementAxis;
    Vector3 JumpDirection => Vector3.up * jumpForce;
    Vector3 RelativeRayPosition => rayPosition + transform.position;

    int WheelAxisYClampInt => (int)Mathf.Ceil(WheelAxisYClamped);

    float WheelAxisYClamped => Mathf.Clamp(WheelAxisY, -1, 1);

    float WheelAxisY => playerInputs.Gameplay.WeaponChange.ReadValue<float>();

    Weapon CurrentWeapon => weapons[weaponIndex];

    public NetworkVariableFloat Health => health;

    //Weapon CurrentWeapon => weapons[0];
    

}
