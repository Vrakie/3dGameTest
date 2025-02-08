using Photon.Pun;
using TMPro;

public class PlayerInfo : MonoBehaviourPun, IPunObservable
{
    #region VARIABLE
    public TMP_Text nomdujoueur; 
    private string playerName;
    #endregion
    void Start()
    {
        if (nomdujoueur == null)
            nomdujoueur = GetComponent<TMP_Text>();

        if (photonView.IsMine) 
        {
            playerName = PhotonNetwork.NickName;
            photonView.RPC("UpdatePlayerName", RpcTarget.AllBuffered, playerName);
        }
    }

    [PunRPC] 
    void UpdatePlayerName(string newName)
    {
        playerName = newName;
        if (nomdujoueur != null)
            nomdujoueur.text = playerName;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(playerName);
        else
        {
            playerName = (string)stream.ReceiveNext();
            if (nomdujoueur != null)
                nomdujoueur.text = playerName;
        }
    }
}
