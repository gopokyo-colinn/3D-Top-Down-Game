using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    void ApplyDamage(int _damage);
    bool IsInvulnerable();

    void ApplyKnockback(Vector3 _sourcePosition, float _pushForce);
}
