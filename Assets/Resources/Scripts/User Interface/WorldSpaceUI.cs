using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WorldSpaceUI : MonoBehaviour
{
    public TextMeshProUGUI txtUiLabel;
    ItemContainer itemContainer;
    // Start is called before the first frame update
    void Start()
    {
        itemContainer = GetComponentInParent<ItemContainer>();
        txtUiLabel.text = itemContainer.item.sItemName;
    }
}
