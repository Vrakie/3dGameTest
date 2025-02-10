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
    private new PhotonView photonView;

    private void Start()
    {
        if (photonView == null)
            photonView = GetComponentInParent<PhotonView>();

        if (photonView.IsMine)
        {
            SaveManager.LoadActiveGameObjects(this);
        }

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

        List<string> activeObjectsNames = new List<string>();

        foreach (DataCustomCharacter obj in list)
        {
            foreach (GameObject go1 in obj.dataList)
            {
                if (go1.activeSelf)
                {
                    activeObjectsNames.Add(go1.name);
                }
            }
        }

        string savedData = string.Join(";", activeObjectsNames);

        PlayerPrefs.SetString("ActiveGameObjects", savedData);
        PlayerPrefs.Save();

        SceneManager.UnloadSceneAsync(2);
        MenuManager.Instance.OpenMenu("title");
    }

    public void LoadActiveGameObjects()
    {
        if (PlayerPrefs.HasKey("ActiveGameObjects"))
        {
            string savedData = PlayerPrefs.GetString("ActiveGameObjects");
            string[] activeObjectNames = savedData.Split(';');
            List<GameObject> activeObjects = new List<GameObject>();

            foreach (DataCustomCharacter obj in list)
            {
                foreach (GameObject go1 in obj.dataList)
                {
                    go1.SetActive(false); // Désactive tous les objets au départ
                    if (System.Array.Exists(activeObjectNames, name => name == go1.name))
                    {
                        activeObjects.Add(go1); // Ajoute les objets actifs à la liste
                    }
                }
            }
            foreach (GameObject go in activeObjects)
            {
                photonView.RPC("Rpc_InitializePlayerSkins", RpcTarget.OthersBuffered, go, true);
            }
        }
    }
    [PunRPC]
    public void Rpc_InitializePlayerSkins(GameObject go, bool isActive = true)
    {
        // Active ou désactive les objets reçus dans le tableau
            go.SetActive(isActive);
    }
}

[System.Serializable]
public class DataCustomCharacter
{
    public List<GameObject> dataList = new List<GameObject>();
    [HideInInspector] public int currentIndex = 0;
}
