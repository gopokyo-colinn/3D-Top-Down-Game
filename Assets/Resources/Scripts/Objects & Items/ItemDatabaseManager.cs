using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabaseManager : MonoBehaviour
{
    #region Singleton
    protected static ItemDatabaseManager instance;
    public static ItemDatabaseManager Instance { get { return instance; } }
    #endregion

    public ItemsDatabase itemDatabase;
    public List<ItemContainer> worldItemsLst;

    private void Awake()
    {
        instance = this;
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
