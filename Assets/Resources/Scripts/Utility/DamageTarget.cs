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
    public float fKnockForce = 8f;

    public Collider attackCollider;
    public GameObject particleEffectPrefab;
    private GameObject particleEffect;

    Vector3 startPos;

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
    }

    private void Update()
    {
        CheckAttackBox(attackCollider);
    }
    public void CheckAttackBox(Collider _col)
    {
        if (_col.enabled)
        {
            Collider[] _coliLst = Physics.OverlapBox(_col.bounds.center, _col.bounds.extents, _col.transform.rotation);

            foreach (Collider _coli in _coliLst)
            {
                if (_coli.transform.root == transform.root)
                {
                    continue;
                }
                if (CheckHitEffects(_coli))
                {
                    if (particleEffectPrefab)
                    {
                        if (particleEffect == null)
                            particleEffect = Instantiate(particleEffectPrefab, _coli.ClosestPoint(transform.position), Quaternion.identity);
                        else
                        {
                            if (!particleEffect.activeSelf)
                            {
                                particleEffect.SetActive(true);
                                particleEffect.transform.position = _coli.ClosestPoint(transform.position);
                            }
                        }
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
            if (bIsProjectileAttack) // if the damaging object is project then knock is calclulted from its starting direction
            {
                _objectToHit.ApplyKnockback(startPos, _knockbackForce);
            }
            else
            {
                _objectToHit.ApplyKnockback(transform.position, _knockbackForce);
            }
            _objectToHit.ApplyDamage(_damage);
            return true;
        }
        return false;
    }
}
