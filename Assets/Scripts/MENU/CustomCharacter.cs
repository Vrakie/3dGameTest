using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class CustomCharacter : MonoBehaviourPunCallbacks
{
    #region VARIABLE
    public List<DataCustomCharacter> list = new List<DataCustomCharacter>();
    [SerializeField] private TMP_InputField nomdujoueur;
    [SerializeField] private Launcher launcher;
    private new PhotonView photonView;
    #endregion

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "CustomPlayer")
            photonView = GetComponent<PhotonView>();
        else if (SceneManager.GetActiveScene().name == "CustomPlayer")
            SaveManager.LoadActiveGameObjects(this);
        if (photonView == null)
            return;
        if (photonView.IsMine)
            LoadActiveGameObjects();
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
            if (data.dataList.Count > 0)
            {
                int randomIndex = Random.Range(0, data.dataList.Count);
                ActivateElement(data, randomIndex);
            }
    }

    public void click()
    {
        PhotonNetwork.NickName = nomdujoueur.text;
        SaveManager.SaveGame(nomdujoueur.text, 1);
        List<string> activeObjectsNames = new List<string>();
        foreach (DataCustomCharacter obj in list)
            foreach (GameObject go1 in obj.dataList)
                if (go1.activeSelf)
                    activeObjectsNames.Add(go1.name);
        string savedData = string.Join(";", activeObjectsNames);
        PlayerPrefs.SetString("ActiveGameObjects", savedData);
        PlayerPrefs.Save();
        SceneManager.UnloadSceneAsync(2);
        MenuManager.Instance.OpenMenu("title");
    }

    public void LoadActiveGameObjectsInCustom()
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

    public void LoadActiveGameObjects()
    {
        if (PlayerPrefs.HasKey("ActiveGameObjects"))
        {
            string savedData = PlayerPrefs.GetString("ActiveGameObjects");
            string[] activeObjectNames = savedData.Split(';');

            List<string> activeObjects = new List<string>();

            foreach (DataCustomCharacter obj in list)
            {
                foreach (GameObject go in obj.dataList)
                {
                    go.SetActive(false);
                    if (System.Array.Exists(activeObjectNames, name => name == go.name))
                    {
                        go.SetActive(true);
                        activeObjects.Add(go.name);
                    }
                }
            }
            if (SceneManager.GetActiveScene().name != "CustomPlayer")
                photonView.RPC("Rpc_InitializePlayerSkins", RpcTarget.OthersBuffered, activeObjects.ToArray());
        }
    }
    [PunRPC]
    public void Rpc_InitializePlayerSkins(string[] activeObjectNames)
    {
        foreach (string objectName in activeObjectNames)
        {
            foreach (DataCustomCharacter obj in list)
            {
                foreach (GameObject go in obj.dataList)
                {
                    if (go.name == objectName)
                    {
                        go.SetActive(true);
                        Debug.Log($"Objet activé via RPC : {go.name}");
                    }
                }
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