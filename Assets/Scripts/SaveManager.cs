using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static void SaveGame(string playerName, int level)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.SetInt("Level", level);

        // Sauvegarde des GameObjects actifs
        SaveActiveGameObjects();

        PlayerPrefs.Save();
        Debug.Log("Sauvegarde r�ussie !");
    }

    public static void LoadGame(out string playerName, out int level)
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        level = PlayerPrefs.GetInt("Level", 1);

        Debug.Log($"Chargement r�ussi : Nom {playerName}, Niveau {level}");
    }

    public static void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Sauvegarde r�initialis�e !");
    }

    public static void SaveActiveGameObjects()
    {
        List<string> activeObjectsNames = new List<string>();

        // R�cup�rer tous les objets CustomCharacter dans la sc�ne
        CustomCharacter customCharacter = Object.FindFirstObjectByType<CustomCharacter>();

        if (customCharacter != null)
        {
            foreach (DataCustomCharacter obj in customCharacter.list)
            {
                foreach (GameObject go in obj.dataList)
                {
                    if (go.activeSelf) // V�rifie si l'objet est actif
                    {
                        activeObjectsNames.Add(go.name); // Ajoute le nom du GameObject
                    }
                }
            }
        }

        // Convertir la liste en une seule cha�ne avec ";"
        string savedData = string.Join(";", activeObjectsNames);
        PlayerPrefs.SetString("ActiveGameObjects", savedData);

        Debug.Log($"Objets actifs sauvegard�s : {savedData}");
    }

    public static void LoadActiveGameObjects(CustomCharacter customCharacter)
    {
        if (PlayerPrefs.HasKey("ActiveGameObjects"))
        {
            string savedData = PlayerPrefs.GetString("ActiveGameObjects");
            string[] activeObjectNames = savedData.Split(';'); // Convertir en liste

            Debug.Log($"Chargement des objets actifs : {savedData}");

            foreach (DataCustomCharacter obj in customCharacter.list)
            {
                foreach (GameObject go in obj.dataList)
                {
                    go.SetActive(false); // D�sactiver tous les objets
                    if (System.Array.Exists(activeObjectNames, name => name == go.name))
                    {
                        go.SetActive(true); // R�activer les objets sauvegard�s
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde d'objets actifs trouv�e !");
        }
    }
}
