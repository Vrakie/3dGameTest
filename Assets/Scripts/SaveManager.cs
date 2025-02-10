using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static void SaveGame(string playerName, int level)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt("Level", level);
        SaveActiveGameObjects();
        PlayerPrefs.Save();
        Debug.Log("Sauvegarde réussie !");
    }

    public static void LoadGame(out string playerName, out int level)
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        level = PlayerPrefs.GetInt("Level", 1);
    }

    public static void ResetGame() => PlayerPrefs.DeleteAll();

    public static void SaveActiveGameObjects()
    {
        List<string> activeObjectsNames = new List<string>();
        CustomCharacter customCharacter = Object.FindFirstObjectByType<CustomCharacter>();
        if (customCharacter != null)
        {
            foreach (DataCustomCharacter obj in customCharacter.list)
            {
                foreach (GameObject go in obj.dataList)
                {
                    if (go.activeSelf)
                        activeObjectsNames.Add(go.name);
                }
            }
        }
        string savedData = string.Join(";", activeObjectsNames);
        PlayerPrefs.SetString("ActiveGameObjects", savedData);
    }

    public static void LoadActiveGameObjects(CustomCharacter customCharacter)
    {
        if (PlayerPrefs.HasKey("ActiveGameObjects"))
        {
            string savedData = PlayerPrefs.GetString("ActiveGameObjects");
            string[] activeObjectNames = savedData.Split(';');
            foreach (DataCustomCharacter obj in customCharacter.list)
            {
                foreach (GameObject go in obj.dataList)
                {
                    go.SetActive(false);
                    if (System.Array.Exists(activeObjectNames, name => name == go.name))
                        go.SetActive(true);
                }
            }
        }
    }
}
