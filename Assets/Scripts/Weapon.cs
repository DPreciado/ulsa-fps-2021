using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
   // [SerializeField] Transform spawnPoint;
   // [SerializeField] Object srcProjectile;

[SerializeField, Range(1f, 10f)] int damage = 1;
    
    [SerializeField] Transform rayOrigin;
    [SerializeField] Color rayColor = Color.red;
    [SerializeField, Range(0.1f, 100f)] float rayDistance = 10f;
    [SerializeField] LayerMask targetMask;

    Rigidbody currentTarget;

    RaycastHit hit;

    [SerializeField, Range(0.1f, 100f)] float force = 10f;
    [SerializeField] Transform reticleTrs;
    [SerializeField] Vector3 reticleInitialScale;
    [SerializeField] public AudioClip shootSFX;
    //GameObject currentProjectile;
    
    public void Shoot()
    {
        currentTarget?.AddForce(-hit.normal * force, ForceMode.Impulse);
        Target target = currentTarget?.GetComponent<Target>();
        target?.GetDamage(damage);
    } 

void Start()
    {
        reticleInitialScale = reticleTrs.localScale;
        reticleTrs.localScale = reticleInitialScale;
    }

    void FixedUpdate()
    {
        if(Physics.Raycast(rayOrigin.position, transform.forward, out hit, rayDistance, targetMask))
        {
            currentTarget = hit.collider.GetComponent<Rigidbody>();
            reticleTrs.position = hit.point;
            reticleTrs.localScale = reticleInitialScale * hit.distance;
        }else{
            currentTarget = null;
            reticleTrs.localScale = reticleInitialScale;
            reticleTrs.localPosition = Vector3.forward;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = rayColor;
        Gizmos.DrawRay(rayOrigin.position, transform.forward * rayDistance);
    }

    public void Active(bool visible) =>gameObject.SetActive(visible);
}
