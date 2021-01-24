using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rBody;
    bool wasFired;
    Collider colli;
    Collider userColli;

    public void Initialize(Collider _collider)
    {
        rBody = GetComponent<Rigidbody>();
        colli = GetComponent<Collider>();
        userColli = _collider;
        wasFired = true;


        Physics.IgnoreCollision(colli, userColli);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Destroy(gameObject);
    }
    public void SetDirectionAndForce(Vector3 _targetPosition, float _fThrowForce, float _fYforce, Collider _collider)
    {
        if (wasFired)
        {
            Vector3 _direction = _targetPosition - transform.position;

            rBody.AddForce(_direction.normalized * _fThrowForce - Physics.gravity * _fYforce, ForceMode.Impulse);

            wasFired = false;
        }
    }
}
