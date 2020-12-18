using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsAssets : MonoBehaviour
{
    private static ItemsAssets instance;

    public static ItemsAssets Instance { 
		get {
			if (instance == null)
			{
				instance = FindObjectOfType<ItemsAssets>();
				if (instance == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(ItemsAssets).Name;
					instance = obj.AddComponent<ItemsAssets>();
				}
			}
			return instance;
		} 
	}

	public ItemContainer healthPotion;

	public ItemContainer GetPrefab(Item _item)
    {
        switch (_item.eType)
        {
			case ItemType.HealthPotion:
				healthPotion.item = _item;
				return healthPotion;
			default:
				return null;
        }
    }

}
