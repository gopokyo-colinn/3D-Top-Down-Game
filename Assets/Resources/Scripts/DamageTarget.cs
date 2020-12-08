using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTarget : MonoBehaviour, ICanDamage
{
    public int iDamage;

    public int Damage()
    {
        return iDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other)
        {
            if(other.gameObject.GetComponent<IHittable>() != null)
            {
                other.gameObject.GetComponent<IHittable>().TakeDamage(Damage());
            }
        }
    }
}
