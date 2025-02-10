using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class CustomCharacter : MonoBehaviourPunCallbacks
{
    public List<DataCustomCharacter> list = new List<DataCustomCharacter>();
    [SerializeField] private TMP_InputField nomdujoueur;
    [SerializeField] private Launcher launcher;

    private void Start()
    {
        // Si c'est le joueur local
        if (photonView.IsMine)
        {
            // Charger les objets actifs pour le joueur local
            SaveManager.LoadActiveGameObjects(this);
        }
        else
        {
            // Désactiver les objets pour les autres joueurs
            DisableOtherPlayerObjects();
        }
    }

    public void SetNextElement(int i) => NextElement(i);
    public void SetPreviousElement(int i) => PreviousElement(i);

    private void NextElement(int index) => ActivateElement(list[index], GetNextIndex(list[index], 1));
    private void PreviousElement(int index) => ActivateElement(list[index], GetNextIndex(list[index], -1));

    private int GetNextIndex(DataCustomCharacter data, int offset)
    {
        data.currentIndex = (data.currentIndex + offset + data.dataList.Count) % data.dataList.Count;
        return data.currentIndex;
    }

    private void ActivateElement(DataCustomCharacter data, int index)
    {
        foreach (GameObject obj in data.dataList)
            obj.SetActive(false);

        if (index >= 0 && index < data.dataList.Count)
            data.dataList[index].SetActive(true);
    }

    public void Randomclick()
    {
        foreach (var data in list)
        {
            if (data.dataList.Count > 0)
            {
                int randomIndex = Random.Range(0, data.dataList.Count);
                ActivateElement(data, randomIndex);
            }
        }
    }

    public void click()
    {
        PhotonNetwork.NickName = nomdujoueur.text;
        SaveManager.SaveGame(nomdujoueur.text, 1);

        // Liste temporaire pour stocker les noms des objets actifs
        List<string> activeObjectsNames = new List<string>();

        foreach (DataCustomCharacter obj in list)
        {
            foreach (GameObject go1 in obj.dataList)
            {
                if (go1.activeSelf) // Vérifie si l'objet est actif
                {
                    activeObjectsNames.Add(go1.name); // Ajoute le nom du GameObject
                }
            }
        }

        // Convertir la liste en une seule chaîne avec ";"
        string savedData = string.Join(";", activeObjectsNames);

        // Sauvegarde dans PlayerPrefs
        PlayerPrefs.SetString("ActiveGameObjects", savedData);
        PlayerPrefs.Save();

        Debug.Log($" Sauvegarde des objets actifs : {savedData}");

        // Décharger la scène et ouvrir le menu
        SceneManager.UnloadSceneAsync(2);
        MenuManager.Instance.OpenMenu("title");
        // launcher.nomdujoueur.text = PhotonNetwork.NickName;
    }

    public void LoadActiveGameObjects()
    {
        if (PlayerPrefs.HasKey("ActiveGameObjects"))
        {
            string savedData = PlayerPrefs.GetString("ActiveGameObjects");
            string[] activeObjectNames = savedData.Split(';'); // Récupère la liste

            Debug.Log($" Chargement des objets actifs : {savedData}");

            // Désactive tous les objets, puis réactive uniquement ceux enregistrés
            foreach (DataCustomCharacter obj in list)
            {
                foreach (GameObject go1 in obj.dataList)
                {
                    go1.SetActive(false); // Désactiver tous les objets
                    if (System.Array.Exists(activeObjectNames, name => name == go1.name))
                    {
                        go1.SetActive(true); // Réactiver si le nom est dans la liste
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning(" Aucune sauvegarde d'objets actifs trouvée !");
        }
    }

    // Nouvelle méthode pour désactiver les objets pour les autres joueurs
    private void DisableOtherPlayerObjects()
    {
        foreach (DataCustomCharacter obj in list)
        {
            foreach (GameObject go in obj.dataList)
            {
                go.SetActive(false); // Désactive les objets pour les autres joueurs
            }
        }
    }
}

[System.Serializable]
public class DataCustomCharacter
{
    public List<GameObject> dataList = new List<GameObject>();
    [HideInInspector] public int currentIndex = 0;
}



/* using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class CustomCharacter : MonoBehaviour
{
    [SerializeField] List<DataCustomCharacter> list = new List<DataCustomCharacter>();
    [SerializeField] TMP_InputField nomdujoueur;
    [SerializeField] Launcher launcher;
    private void Start()
    {
        launcher = GameObject.Find("TitleManager").GetComponent<Launcher>();
    }
    public void SetNextElement(int i) => NextElement(i);
    public void SetPreviousElement(int i) => PreviousElement(i);
    void NextElement(int index) => ActivateElement(list[index], GetNextIndex(list[index], 1));
    void PreviousElement(int index) => ActivateElement(list[index], GetNextIndex(list[index], -1));
    int GetNextIndex(DataCustomCharacter data, int offset)
    {
        data.currentIndex = (data.currentIndex + offset + data.dataList.Count) % data.dataList.Count;
        return data.currentIndex;
    }
    void ActivateElement(DataCustomCharacter data, int index)
    {
        foreach (GameObject obj in data.dataList)
            obj.SetActive(false);

        if (index >= 0 && index < data.dataList.Count)
            data.dataList[index].SetActive(true);
    }
    public void Randomclick()
    {
        foreach (var data in list)
        {
            if (data.dataList.Count > 0)
            {
                int randomIndex = Random.Range(0, data.dataList.Count);
                ActivateElement(data, randomIndex);
            }
        }
    }
    public void click()
    {
        PhotonNetwork.NickName = nomdujoueur.text;
        SaveManager.SaveGame(nomdujoueur.text, 1);
        foreach (DataCustomCharacter obj in list)
        {
            foreach (GameObject go1 in obj.dataList)
            {

            }
        }
        SceneManager.UnloadSceneAsync(2);

        MenuManager.Instance.OpenMenu("title");
        launcher.nomdujoueur.text = PhotonNetwork.NickName;
    }
}
[System.Serializable]
public class DataCustomCharacter
{
    public List<GameObject> dataList = new List<GameObject>();
    [HideInInspector] public int currentIndex = 0;
} */