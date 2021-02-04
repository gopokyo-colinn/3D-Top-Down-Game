using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTarget : MonoBehaviour, ICanDamage
{
    public float fDamage;
    //public Collider dmgColi;
    public bool bIsProjectileAttack;
    public bool bIsTypeOfEnemy;
    public bool bIsPlayer; 
    public float fKnockForce = 8f;
    public Collider attackCollider;

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

                CheckHitEffects(_coli);
            }
        }
    }
    public float Damage()
    {
        return fDamage;
    }

    private void CheckHitEffects(Collider _other)
    {
       // if (other)
        {
            if (bIsTypeOfEnemy)
            {
                if (_other.gameObject.layer == LayerMask.NameToLayer("Player")) /// this is so that enemies ignore damaging each other
                {
                    IHittable _hitTarget = _other.GetComponent<IHittable>();

                    if (bIsProjectileAttack)
                    {
                        _hitTarget.ApplyKnockback(startPos, fKnockForce);
                    }
                    else
                    {
                        _hitTarget.ApplyKnockback(transform.position, fKnockForce);
                    }
                    _hitTarget.ApplyDamage(Damage());
                }
                if (_other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
                {
                    if (_other.gameObject.GetComponent<Shield>())
                    {
                        IHittable _hitTarget = _other.GetComponentInParent<IHittable>();

                        if (bIsProjectileAttack)
                        {
                            _hitTarget.ApplyKnockback(startPos, fKnockForce / 2f);
                        }
                        else
                        {
                            _hitTarget.ApplyKnockback(transform.position, fKnockForce / 2f);
                        }
                        _hitTarget.ApplyDamage(0);
                    }
                }
            }
            else 
            {
                if (_other.gameObject.layer == LayerMask.NameToLayer("Weapon"))
                {
                    if (_other.gameObject.GetComponent<Shield>())
                    {
                        IHittable _hitTarget = _other.GetComponentInParent<IHittable>();

                        if (bIsProjectileAttack)
                        {
                            _hitTarget.ApplyKnockback(startPos, fKnockForce / 2f);
                        }
                        else
                        {
                            _hitTarget.ApplyKnockback(transform.position, fKnockForce / 2f);
                        }
                        _hitTarget.ApplyDamage(0);
                    }
                }
                else
                {
                    IHittable _hitTarget = _other.GetComponent<IHittable>();

                    if (_hitTarget != null)
                    {
                        if (bIsProjectileAttack)
                        {
                            _hitTarget.ApplyKnockback(startPos, fKnockForce);
                        }
                        else
                        {
                            _hitTarget.ApplyKnockback(transform.position, fKnockForce);
                        }
                        _hitTarget.ApplyDamage(Damage());
                    }
                }
            }
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
}
