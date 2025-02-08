using Photon.Pun;
using UnityEngine;

public class LagCompensation : MonoBehaviourPun, IPunObservable
{
    Vector3 latestPos;
    Quaternion latestRot;

    // Lag compensation
    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero;
    Quaternion rotationAtLastPacket = Quaternion.identity;

    public bool teleportIfFar;
    public float teleportIfFarDistance;

    [Header("Lerping[Experimental]")]
    public float smoothpos = 0.5f;
    public float smoothRot = 0.5f;

    void Awake()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Envoi des données de position et de rotation
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Réception des données de position et de rotation
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();

            // Compensation de lag
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
            // Calcul du facteur de Lerp pour la position et la rotation
            float lerpFactor = (float)(currentTime / timeToReachGoal);
            lerpFactor = Mathf.Clamp01(lerpFactor); // Limite la valeur entre 0 et 1

            // Interpolation de la position et de la rotation
            transform.position = Vector3.Lerp(positionAtLastPacket, latestPos, lerpFactor);
            transform.rotation = Quaternion.Lerp(rotationAtLastPacket, latestRot, lerpFactor);
        }
        else
        {
            // Si `timeToReachGoal` est trop faible, on téléporte directement (sans interpolation)
            transform.position = latestPos;
            transform.rotation = latestRot;
        }

        // Gestion de la téléportation si la distance entre la position actuelle et la position la plus récente est trop grande
        if (Vector3.Distance(transform.position, latestPos) > teleportIfFarDistance)
        {
            transform.position = latestPos;
            transform.rotation = latestRot;
        }
    }
}
