using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public Transform container;
    public Image bgImage;

    private void Awake()
    {
        container.gameObject.SetActive(false);
    }
    public virtual void open()
    {
        container.gameObject.SetActive(true);
    }

    public virtual void close()
    {
        container.gameObject.SetActive(false);
    }
    public bool IsActive()
    {
        return container.gameObject.activeSelf;
    }
}
