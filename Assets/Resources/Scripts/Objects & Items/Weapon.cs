using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Collider coli;
    PlayerController player;
    public ParticleSystem weaponTrialEffect;

    //TODO: Add functionality to be used by enemies too...

    public void Start()
    {
        coli = GetComponent<Collider>();
        coli.enabled = false;
        player = PlayerController.Instance;
        coli.isTrigger = true;
    }


    public void WeaponColiSetActive(bool _bToEnable)
    {
        coli.enabled = _bToEnable;
    }

}