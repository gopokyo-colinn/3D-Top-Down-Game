using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSimulationSpaceChanger : MonoBehaviour
{
    private ParticleSystem ps;
    public bool useLocal = true;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        useLocal = main.simulationSpace == ParticleSystemSimulationSpace.Local;
    }

    void Update()
    {
        var main = ps.main;
        main.simulationSpace = useLocal ? ParticleSystemSimulationSpace.Local : ParticleSystemSimulationSpace.World;
    }

}
