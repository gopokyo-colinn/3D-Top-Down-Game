using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour
{
    const float fMIN_PROJECTILE_POWER = 4f;
    float fThrowForce;

    public Projectile attackPrefab;
    public Transform attackPos;
    public float fSpeed;
    public float fParabolicForce;
    public bool bCanAttack = false;
    public Collider userCollider;
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameController.Instance.player;
        if(userCollider == null)
        {
            userCollider = GetComponentInParent<Collider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bCanAttack)
        {
            bCanAttack = false;
            InitializeProjectile();
        }
    }

    public void InitializeProjectile()
    {
        Projectile _projectile = Instantiate(attackPrefab, attackPos.position, Quaternion.identity);

        _projectile.Initialize(userCollider);

        float _dist = (player.transform.position - transform.position).sqrMagnitude;

        fThrowForce = Mathf.Sqrt(_dist);

        if (fThrowForce < fMIN_PROJECTILE_POWER)
        {
            fThrowForce = fMIN_PROJECTILE_POWER;
        }

        _projectile.SetDirectionAndForce(player.transform.position, fMIN_PROJECTILE_POWER + fSpeed, Random.Range(0.4f, fParabolicForce + 0.1f), GetComponentInParent<Collider>());
    }
}
