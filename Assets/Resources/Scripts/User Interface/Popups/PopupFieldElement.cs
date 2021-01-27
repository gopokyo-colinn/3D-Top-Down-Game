using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopupFieldElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public Image imgSelected;

    public void OnPointerClick(PointerEventData eventData)
    {
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public virtual void SetElement(bool _setElement)
    {

    }

    public virtual void SetSelectedElement()
    {

    }
}
