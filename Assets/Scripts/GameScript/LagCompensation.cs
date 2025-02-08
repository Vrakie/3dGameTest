using Photon.Pun;
using UnityEngine;

public class LagCompensation : MonoBehaviourPun, IPunObservable
{
    #region VARIABLE
    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero, latestPos;
    Quaternion rotationAtLastPacket = Quaternion.identity, latestRot;
    public bool teleportIfFar;
    [Header("Lerping[Experimental]")]
    public float smoothpos = 0.5f , smoothRot = 0.5f, teleportIfFarDistance;
    #endregion
    void Awake()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
            positionAtLastPacket = transform.position;
            rotationAtLastPacket = transform.rotation;
        }
    }
    void FixedUpdate()
    {
        if (photonView.IsMine) return;
        double timeToReachGoal = currentPacketTime - lastPacketTime;
        currentTime += Time.deltaTime;
        if (timeToReachGoal > 0)
        {
            float lerpFactor = (float)(currentTime / timeToReachGoal);
            lerpFactor = Mathf.Clamp01(lerpFactor);
            transform.position = Vector3.Lerp(positionAtLastPacket, latestPos, lerpFactor);
            transform.rotation = Quaternion.Lerp(rotationAtLastPacket, latestRot, lerpFactor);
        }
        else
        {
            transform.position = latestPos;
            transform.rotation = latestRot;
        }
        if (Vector3.Distance(transform.position, latestPos) > teleportIfFarDistance)
        {
            transform.position = latestPos;
            transform.rotation = latestRot;
        }
    }
}
