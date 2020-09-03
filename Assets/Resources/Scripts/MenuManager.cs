using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public DialogBoxPopup dialogBoxPopup;

    void Start()
    {
        Instance = this;   
    }

    void Update()
    {
        
    }
}
