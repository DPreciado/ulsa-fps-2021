using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : MonoBehaviour
{
    PlayerInputs playerInputs;

    [SerializeField] float damage = 10f;
    [SerializeField] float range = 100f;
    [SerializeField] Transform fpsCam;

    [SerializeField] GameObject bullet;
    
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
        playerInputs.Gameplay.Shoot.performed += _ => Shoot();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range)) 
        {
            Debug.Log(hit.transform.name);
        }
    }
}
