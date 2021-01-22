using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item Database", menuName ="Assets/ItemDatabase")]
public class ItemsDatabase : ScriptableObject
{
    public List<Item> allItemsLst;
}
