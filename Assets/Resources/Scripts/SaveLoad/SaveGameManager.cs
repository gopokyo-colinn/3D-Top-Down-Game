using System.IO;
using UnityEngine;

public static class SaveGameManager
{
    public static string sFileName = "gamesave"; // 1 save file for now

    public static void SaveGame(ISaveable[] _saveAbles)
    {
        SaveData _saveData = new SaveData();

        foreach (var _saveable in _saveAbles)
        {
            _saveable.SaveAllData(_saveData);
        }
        FileManager.WriteToFile(sFileName, _saveData.ToJson());
        Debug.Log("Game Saved");
    }

    public static void LoadGame(ISaveable[] _saveables) // ChANGE return type to saveData class
    {
        if (FileManager.LoadFromFile(sFileName, out var _jsonSave))
        {
            SaveData _savedData = new SaveData();
            _savedData.LoadFromJson(_jsonSave);

            foreach (var _saveable in _saveables)
            {
                _saveable.LoadSaveData(_savedData);
            }

            Debug.Log("Game Loaded");
        }
    }
}
