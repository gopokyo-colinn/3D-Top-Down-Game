using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour, IHittable
{
    public int health = 1;
    public ParticleSystem destroyParticleEffect;
    Collider coli;

    bool destroyed = false;
    // Start is called before the first frame update
    void Start()
    {
        destroyed = false;
        coli = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            DestroyObject();
        }
    }

    public void TakeDamage(int _damage)
    {
        health -= _damage;
    }

    public void DestroyObject()
    {
        if (!destroyed)
        {
            coli.enabled = false;
            ParticleSystem particle = Instantiate(destroyParticleEffect, transform.position + new Vector3(0,0.4f,0), transform.rotation, transform);
            gameObject.GetComponentInChildren<Renderer>().enabled = false;
            destroyed = true;
            Destroy(gameObject, 2f);
        }
    }

    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider)
        {
            if (_collider.gameObject.GetComponent<ICanDamage>() != null)
            {
                TakeDamage(_collider.gameObject.GetComponent<ICanDamage>().Damage());
            }
        }
    }
}
