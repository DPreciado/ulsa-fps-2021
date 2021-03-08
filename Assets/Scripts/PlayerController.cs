using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerInputs playerInputs;
    [SerializeField, Range(0.1f, 10f)]
    float moveSpeed = 5f;

    [SerializeField]
    Transform camTrs;
    
    [SerializeField, Range(0.1f, 180f)]
    float camRotSpeed = 90f;

    float camRotationAmounthY;
    void Awake() {
        playerInputs = new PlayerInputs();
    }

    void OnEnable() {
        playerInputs.Enable();
    }

    void OnDisable() {
        playerInputs.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(Axis.x, 0f, Axis.y) * moveSpeed * Time.deltaTime);
        camTrs.Rotate(Vector3.right * -CamAxis.y * camRotSpeed * Time.deltaTime);
        camRotationAmounthY += CamAxis.x * camRotSpeed * Time.deltaTime;
        camTrs.rotation = Quaternion.Euler(camTrs.rotation.x, camRotationAmounthY, camTrs.rotation.z);
    }

    Vector2 Axis => playerInputs.Gameplay.Movement.ReadValue<Vector2>();

    float CamAxisX => playerInputs.Gameplay.CamAxisX.ReadValue<float>();
    float CamAxisY => playerInputs.Gameplay.CamAxisY.ReadValue<float>();

    Vector2 CamAxis => new Vector2(CamAxisX,CamAxisY);
}
