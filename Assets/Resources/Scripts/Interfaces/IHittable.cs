using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    void TakeDamage(int _damage);
    bool IsInvulnerable();

    void Knockback(Vector3 _sourcePosition, float _pushForce);
}
