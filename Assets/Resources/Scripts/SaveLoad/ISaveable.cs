using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveable 
{
    void SaveAllData(SaveData _saveData);
    void LoadSaveData(SaveData _saveData);
}
