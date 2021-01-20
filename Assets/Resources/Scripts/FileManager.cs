using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class FileManager 
{
    static string sDirectory = "/SaveGames/";
    public static bool WriteToFile(string _sFileName, string _sFileContents)
    {
        string _dir = Application.persistentDataPath + sDirectory;

        if (!Directory.Exists(_dir))
        {
            Directory.CreateDirectory(_dir);
        }

        var fullPath = Path.Combine(_dir, _sFileName);

        try
        {
            File.WriteAllText(fullPath, _sFileContents);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string _sFileName, out string _sResult)
    {
        string _dir = Application.persistentDataPath + sDirectory;

        var fullPath = Path.Combine(_dir, _sFileName);

        try
        {
            _sResult = File.ReadAllText(fullPath);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            _sResult = "";
            return false;
        }
    }
}
