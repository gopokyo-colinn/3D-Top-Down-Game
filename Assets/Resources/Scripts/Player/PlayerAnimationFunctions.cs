using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPartToAttachTo { spine, leftHand, rightHand }

public class PlayerAnimationFunctions : MonoBehaviour
{
    PlayerController player;

    public Transform shieldDefault;
    public Transform swordDefault;
    public Transform shieldHandParent;
    public Transform swordHandParent;

    public GameObject shield;
    public GameObject sword;
    public GameObject swordSlashParticles;

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }
    public void ShieldActivate(BodyPartToAttachTo bodyPart)
    {
        if(bodyPart == BodyPartToAttachTo.spine)
            shield.transform.parent = shieldDefault.transform;
        else if(bodyPart == BodyPartToAttachTo.leftHand)
            shield.transform.parent = shieldHandParent.transform;

        shield.transform.localPosition = Vector3.zero;
        shield.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    public void SwordActivate(BodyPartToAttachTo bodyPart)
    {
        if(bodyPart == BodyPartToAttachTo.spine)
            sword.transform.parent = swordDefault.transform;
        else if(bodyPart == BodyPartToAttachTo.rightHand)
            sword.transform.parent = swordHandParent.transform;

        sword.transform.localPosition = Vector3.zero;
        sword.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void EnableSlashParticles()
    {
        swordSlashParticles.gameObject.SetActive(true);
      //  swordSlashParticles.transform.GetChild(0).GetComponent<ParticleSimulationSpaceChanger>().useLocal = true;
       // swordSlashParticles.transform.GetChild(1).GetComponent<ParticleSimulationSpaceChanger>().useLocal = true;
    }
    public void DisableSlashParticles()
    {
       // swordSlashParticles.transform.GetChild(0).GetComponent<ParticleSimulationSpaceChanger>().useLocal = false;
       // swordSlashParticles.transform.GetChild(1).GetComponent<ParticleSimulationSpaceChanger>().useLocal = false;
       //tartCoroutine(disableGameObjectAfter(swordSlashParticles, false));
        swordSlashParticles.gameObject.SetActive(false);
    }
    public void SetAttackFalse()
    {
       player.isAttacking = false;
    }

    IEnumerator disableGameObjectAfter(GameObject _go, bool _enableDisable)
    {
        yield return new WaitForSeconds(.2f);
        _go.SetActive(_enableDisable);
    }

    
}
