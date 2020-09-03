using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public bool inPlayMode;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        inPlayMode = true;
    }
}
