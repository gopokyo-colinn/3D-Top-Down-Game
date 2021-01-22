using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabaseManager : MonoBehaviour
{
    private static ItemDatabaseManager instance;
    public static ItemDatabaseManager Instance {
        get
        {
            if (instance == null)
            {
                instance = new ItemDatabaseManager();
            }
            return instance;
        }
    }

    public ItemsDatabase itemDatabase;
    public List<ItemContainer> worldItemsLst;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        worldItemsLst = new List<ItemContainer>();
    }

    public Item GetItemByID(string _sID)
    {
        List<Item> _allItemsLst = instance.itemDatabase.allItemsLst;
        for (int i = 0; i < _allItemsLst.Count; i++)
        {
            if(_allItemsLst[i].sID == _sID)
            {
                return _allItemsLst[i];
            }
        }
        return null;
    }
}
