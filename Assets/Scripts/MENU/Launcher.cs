using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using System.Collections;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region VARIABLE
    public static Launcher Instance;
    [SerializeField] TMP_InputField roomNameInputField, playerNameInputField;
    [SerializeField] TMP_Text errorText, roomNameText, nomdujoueur;
    [SerializeField] Transform roomListContent, playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab, startGameButton, CanvasTitleScreen, roomListItemPrefab;
    #endregion
    void Awake() => Instance = this;
    void Start() => PhotonNetwork.ConnectUsingSettings();
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public override void OnJoinedLobby() => MenuManager.Instance.OpenMenu("custom room");
    public void CustomValide()
    {
        MenuManager.Instance.OpenMenu("title");
        CanvasTitleScreen.SetActive(true);
        PhotonNetwork.NickName = playerNameInputField.text;
        nomdujoueur.text = PhotonNetwork.NickName;
    }
    public void AppQuit() => Application.Quit();
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
            return;
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform child in playerListContent)
            Destroy(child.gameObject);
        for (int i = 0; i < players.Count(); i++)
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
#if UNITY_EDITOR
        if (PhotonNetwork.CurrentRoom.Name == "TEST ROOM")
        {
            PhotonNetwork.LoadLevel(1);
        }
#endif
    }
    public override void OnMasterClientSwitched(Player newMasterClient) => startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message);
        MenuManager.Instance.OpenMenu("error");
    }
    public void StartGame() => PhotonNetwork.LoadLevel(1);
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loading");
    }
    public override void OnLeftRoom() => MenuManager.Instance.OpenMenu("title");
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
            Destroy(trans.gameObject);

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) => Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
#if UNITY_EDITOR
    public void ONTESTROOM()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.NickName = "TEST GAME";
            StartCoroutine(WaitForConnectionAndCreateRoom());
            return;
        }
        PhotonNetwork.NickName = "TEST GAME";
        nomdujoueur.text = PhotonNetwork.NickName;
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LoadLevel(1);
            return;
        }
        PhotonNetwork.CreateRoom("TEST ROOM", new RoomOptions { MaxPlayers = 4 });
    }
    private IEnumerator WaitForConnectionAndCreateRoom()
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;
        }
        PhotonNetwork.CreateRoom("TEST ROOM", new RoomOptions { MaxPlayers = 4 });
    }
#endif
}