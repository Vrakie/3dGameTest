using Photon.Pun;
public class DesactivationCam : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (!photonView.IsMine)
           gameObject.SetActive(false);
    }
}
