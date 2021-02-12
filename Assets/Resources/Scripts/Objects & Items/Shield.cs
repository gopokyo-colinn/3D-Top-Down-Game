using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    Collider coli;
    // Start is called before the first frame update
    void Start()
    {
        coli = GetComponent<Collider>();
        coli.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
