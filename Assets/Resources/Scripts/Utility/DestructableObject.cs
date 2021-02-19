using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour, IHittable
{
    public float fHealth = 1;
    public ParticleSystem destroyParticleEffect;
    Collider coli;

    bool bDestroyed = false;
    bool bTookDamage;
    // Start is called before the first frame update
    void Start()
    {
        bDestroyed = false;
        coli = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void ApplyDamage(float _fDamage)
    {
        if (!bTookDamage)
        {
            fHealth -= _fDamage;
            bTookDamage = true;
            StartCoroutine(HelpUtils.WaitForSeconds(delegate { bTookDamage = false; }, 0.5f));
        }
        if (fHealth <= 0)
        {
            DestroyObject();
        }
    }

    public void DestroyObject()
    {
        if (!bDestroyed)
        {
            coli.enabled = false;
            ParticleSystem particle = Instantiate(destroyParticleEffect, transform.position + new Vector3(0,0.4f,0), transform.rotation, transform);
            gameObject.GetComponentInChildren<Renderer>().enabled = false;
            bDestroyed = true;
            Destroy(gameObject, 2f);
        }
    }


    public bool IsInvulnerable()
    {
        return false;
    }

    public void ApplyKnockback(Vector3 _sourcePosition, float _pushForce)
    {
        //
    }
}
