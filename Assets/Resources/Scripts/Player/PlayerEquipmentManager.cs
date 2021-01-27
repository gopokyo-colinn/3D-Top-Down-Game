using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPartToAttachTo { spine, leftHand, rightHand }

public class PlayerEquipmentManager : MonoBehaviour
{
    PlayerController player;
    // Assigned equipped weapons Placeholder
    public Transform phShieldEquipped;
    public Transform phPrimaryWeaponEquipped;
    public Transform phSecondaryWeaponEquipped;
    // Assigned Wepaons and Equipment Unequipped Placeholder
    public Transform phShieldUnEquipped;
    public Transform phPrimaryWeaponUnEquipped;
    public Transform phSecondaryWeaponUnEquipped;
    //  Assigned Weapons and Shiled Itself
    public GameObject shield;
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;
    // Assigned Weapons and Equipment SFX
    public GameObject trialEffects;
    Animator trialEffectAnimator;
    GameObject swish1;

    //public GameObject swordSlashParticles;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        trialEffectAnimator = trialEffects.GetComponent<Animator>();
        swish1 = trialEffects.transform.GetChild(0).gameObject;
        swish1.SetActive(false);
    }
    public void ShieldActivate(BodyPartToAttachTo bodyPart)
    {
        if(shield != null)
        {
            if (bodyPart == BodyPartToAttachTo.spine)
                shield.transform.parent = phShieldUnEquipped.transform;
            else if (bodyPart == BodyPartToAttachTo.leftHand)
                shield.transform.parent = phShieldEquipped.transform;

            shield.transform.localPosition = Vector3.zero;
            shield.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
    public void PrimaryWeaponActivate(BodyPartToAttachTo bodyPart)
    {
        if(primaryWeapon != null)
        {
            if(bodyPart == BodyPartToAttachTo.spine)
                primaryWeapon.transform.parent = phPrimaryWeaponUnEquipped.transform;
            else if(bodyPart == BodyPartToAttachTo.rightHand)
                primaryWeapon.transform.parent = phPrimaryWeaponEquipped.transform;
            primaryWeapon.transform.localPosition = Vector3.zero;
            primaryWeapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void EnableSlashParticles()
    {
        trialEffectAnimator.SetBool("slash_1b", player.IsAttacking());
    }
    public void DisableSlashParticles()
    {
        trialEffectAnimator.SetBool("slash_1b", false);
    }
    public void SetAttackFalse()
    {
       player.SetCanAttack(true);
    }

    IEnumerator disableGameObjectAfter(GameObject _go, bool _enableDisable)
    {
        yield return new WaitForSeconds(.2f);
        _go.SetActive(_enableDisable);
    }

    public void SetPrimaryWeapon(GameObject _primaryWeapon)
    {
        if(primaryWeapon != null)
        {
            Destroy(primaryWeapon);
        }
        primaryWeapon = _primaryWeapon;
    }
    public void SetSecondaryWeapon(GameObject _secondaryWeapon)
    {
        if (secondaryWeapon != null)
        {
            Destroy(primaryWeapon);
        }
        secondaryWeapon = _secondaryWeapon;
    }
    public void SetShield(GameObject _shield)
    {
        if (shield != null)
        {
            Destroy(primaryWeapon);
        }
        shield = _shield;
    }
    public Item GetPrimaryWeaponItem()
    {
        return primaryWeapon.GetComponent<ItemContainer>().item;
    }
    public Item GetSecondaryWeaponItem()
    {
        return secondaryWeapon.GetComponent<ItemContainer>().item;
    }
    public Item GetShieldItem()
    {
        return shield.GetComponent<ItemContainer>().item;
    }
}
