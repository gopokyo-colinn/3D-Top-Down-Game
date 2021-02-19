using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTarget : MonoBehaviour, ICanDamage
{
    public enum DamagerType { Weapon, Projectile, Enemy, Player }
    public float fDamage;
    //public Collider dmgColi;
    public bool bIsProjectileAttack;
    public bool bIsTypeOfEnemy;
    public bool bIsPlayer;
    public bool bAttackCompleted;
    public float fKnockForce = 8f;

    public Collider attackCollider;
    public GameObject particleEffectPrefab;
    private GameObject[] maxParticleEffects;

    Collider[] allHitTargetsInSweep;
    Collider[] maxHitTargets; // max number of targets sword can hit in one sweep, it is also used to check that a target dont get hit twice in a sweep

    Vector3 startPos;

    public void OnEnable()
    {
        Debug.Log("I am enabled");
    }
    public void Start()
    {
        if (GetComponentInParent<PlayerController>())
            bIsPlayer = true;
        if (GetComponent<Enemy>() || GetComponentInParent<Enemy>())
            bIsTypeOfEnemy = true;
        if (GetComponent<Projectile>())
            bIsProjectileAttack = true;
        if (bIsProjectileAttack)
            startPos = transform.position;

        attackCollider.isTrigger = true;
        maxHitTargets = new Collider[4];
        maxParticleEffects = new GameObject[4];
    }

    private void Update()
    {
        CheckAttackBox(attackCollider);
    }
    int iIndex = 0;
    public void CheckAttackBox(Collider _col)
    {
        if (_col.enabled)
        {
            allHitTargetsInSweep = Physics.OverlapBox(_col.bounds.center, _col.bounds.extents, _col.transform.rotation);
          
            foreach (Collider _coli in allHitTargetsInSweep)
            {
               
                if (_coli.transform.root == transform.root || CheckPreviousHitObjects(_coli))
                {
                    continue;
                }
                else
                {
                    if (CheckHitEffects(_coli))
                    {
                        maxHitTargets[iIndex] = _coli;

                        InstantiateHitParticles(_coli, iIndex);
                       
                        if (iIndex < maxHitTargets.Length - 1)
                            iIndex++;
                        break;
                    }
                }
            }
            bAttackCompleted = true;
        }
        else
        {
            if (bAttackCompleted)
            {
                iIndex = 0;
                for (int i = 0; i < maxHitTargets.Length; i++)
                {
                    maxHitTargets[i] = null;
                }
                bAttackCompleted = false;
            }
        }
    }
    public bool CheckPreviousHitObjects(Collider _coli)
    {
        for (int i = 0; i < maxHitTargets.Length; i++)
        {
            if (maxHitTargets[i] == _coli)
                return true;
        }
        return false;
    }
    public void InstantiateHitParticles(Collider _coli, int _iIndex)
    {
        if (particleEffectPrefab)
        {
            if (maxParticleEffects[_iIndex] == null)
                maxParticleEffects[_iIndex] = Instantiate(particleEffectPrefab, _coli.ClosestPoint(transform.position), Quaternion.identity);
            else
            {
                for (int i = 0; i < maxParticleEffects.Length; i++)
                {
                    if (maxParticleEffects[i] != null && !maxParticleEffects[i].activeSelf)
                    {
                        maxParticleEffects[i].SetActive(true);
                        maxParticleEffects[i].transform.position = _coli.ClosestPoint(transform.position);
                        break;
                    }
                }
            }
        }
    }
    public float Damage()
    {
        return fDamage;
    }

    private bool CheckHitEffects(Collider _other)
    { 
        if (bIsTypeOfEnemy) // if an attack done by enemy
        {
            if (_other.gameObject.layer == LayerMask.NameToLayer("Player")) /// this is so that enemies ignore damaging each other
            {
                IHittable _hitTarget = _other.GetComponent<IHittable>();
                return DoDamageAndApplyKnockback(_hitTarget, fDamage, fKnockForce);
            }
            else if (_other.gameObject.layer == LayerMask.NameToLayer("Weapon")) // To check if the attack hit a shield then knockback is halved
            {
                if (_other.gameObject.GetComponent<Shield>())
                {
                    IHittable _hitTarget = _other.GetComponentInParent<IHittable>();
                    return DoDamageAndApplyKnockback(_hitTarget, 0, fKnockForce / 2);
                }
            }
            return false;
        }
        else if (bIsProjectileAttack)
        {
            if (_other.gameObject.layer == LayerMask.NameToLayer("Player")) /// this is so that enemies ignore damaging each other
            {
                IHittable _hitTarget = _other.GetComponent<IHittable>();
                return DoDamageAndApplyKnockback(_hitTarget, fDamage, fKnockForce);
            }
            else if (_other.gameObject.layer == LayerMask.NameToLayer("Weapon")) // To check if the attack hit a shield then knockback is halved
            {
                if (_other.gameObject.GetComponent<Shield>())
                {
                    IHittable _hitTarget = _other.GetComponentInParent<IHittable>();
                    return DoDamageAndApplyKnockback(_hitTarget, 0, fKnockForce / 2);
                }
            }
            if (_other)
                return true;
            return false;
        }
        else // if attack done by someone other than an enemy, could be player or an ally
        {
            if (_other.gameObject.layer == LayerMask.NameToLayer("Weapon")) // to check if attack hit the shield
            {
                if (_other.gameObject.GetComponent<Shield>())
                {
                    IHittable _hitTarget = _other.GetComponentInParent<IHittable>();
                    return DoDamageAndApplyKnockback(_hitTarget, 0, fKnockForce / 2);
                }
            }
            else
            {
                IHittable _hitTarget = _other.GetComponent<IHittable>();
                return DoDamageAndApplyKnockback(_hitTarget, fDamage, fKnockForce);
            }
            return false;
        }
    }
    public void InitializeStats(ItemContainer _itemContainer)
    {
        if (_itemContainer)
        {
            if (_itemContainer.item.eType == ItemType.PrimaryWeapon || _itemContainer.item.eType == ItemType.SecondaryWeapon)
            {
                fDamage = _itemContainer.item.fEffectValue;
                fKnockForce = _itemContainer.item.fWeaponKnockback;
            }
        }
    }
    public bool DoDamageAndApplyKnockback(IHittable _objectToHit, float _damage, float _knockbackForce)
    {
        if (_objectToHit != null)
        {
            _objectToHit.ApplyKnockback(transform.position, _knockbackForce);
            _objectToHit.ApplyDamage(_damage);
            return true;
        }
        return false;
    }
}
