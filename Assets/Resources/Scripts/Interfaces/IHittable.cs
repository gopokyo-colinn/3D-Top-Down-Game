﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
    void TakeDamage(int _damage);
    void IsInvulnerable(bool _invulnerable);
}
