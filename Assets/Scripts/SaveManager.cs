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
        Debug.Log("Sauvegarde réussie !");
    }

    public static void LoadGame(out string playerName, out int level)
    {
        playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        level = PlayerPrefs.GetInt("Level", 1);

        Debug.Log($"Chargement réussi : Nom {playerName}, Niveau {level}");
    }

    public static void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Sauvegarde réinitialisée !");
    }

    public static void SaveActiveGameObjects()
    {
        List<string> activeObjectsNames = new List<string>();

        // Récupérer tous les objets CustomCharacter dans la scène
        CustomCharacter customCharacter = Object.FindFirstObjectByType<CustomCharacter>();

        if (customCharacter != null)
        {
            foreach (DataCustomCharacter obj in customCharacter.list)
            {
                foreach (GameObject go in obj.dataList)
                {
                    if (go.activeSelf) // Vérifie si l'objet est actif
                    {
                        activeObjectsNames.Add(go.name); // Ajoute le nom du GameObject
                    }
                }
            }
        }

        // Convertir la liste en une seule chaîne avec ";"
        string savedData = string.Join(";", activeObjectsNames);
        PlayerPrefs.SetString("ActiveGameObjects", savedData);

        Debug.Log($"Objets actifs sauvegardés : {savedData}");
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
                    go.SetActive(false); // Désactiver tous les objets
                    if (System.Array.Exists(activeObjectNames, name => name == go.name))
                    {
                        go.SetActive(true); // Réactiver les objets sauvegardés
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde d'objets actifs trouvée !");
        }
    }
}
