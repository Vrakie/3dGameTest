using Photon.Pun;
using UnityEngine;

public class LagCompensation : MonoBehaviourPun, IPunObservable
{
    #region VARIABLES
    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero, latestPos;
    Quaternion rotationAtLastPacket = Quaternion.identity, latestRot;
    public bool teleportIfFar;
    [Header("Lerping[Experimental]")]
    public float smoothPos = 0.5f, smoothRot = 0.5f, teleportIfFarDistance = 5f;
    public Vector3 velocity; // Vitesse du joueur pour la pr�diction c�t� client.
    #endregion

    void Awake()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
    }

    // Synchronisation des donn�es du joueur (position et rotation)
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

    // FixedUpdate pour la compensation de la latence, interpolation et pr�diction
    void FixedUpdate()
    {
        // Si c'est le joueur local, on applique la pr�diction de mouvement directement
        if (photonView.IsMine)
        {
            PredictMovement();
            return;
        }

        // Calcule le temps jusqu'� ce que les donn�es arrivent � destination
        double timeToReachGoal = currentPacketTime - lastPacketTime;
        currentTime += Time.deltaTime;

        // Interpolation pour rendre les mouvements plus fluides
        if (timeToReachGoal > 0)
        {
            float lerpFactor = (float)(currentTime / timeToReachGoal);
            lerpFactor = Mathf.Clamp01(lerpFactor);
            transform.position = Vector3.Lerp(positionAtLastPacket, latestPos, lerpFactor);
            transform.rotation = Quaternion.Lerp(rotationAtLastPacket, latestRot, lerpFactor);
        }
        else
        {
            // Si aucune interpolation n'est n�cessaire, on applique directement la position/rotation re�ues
            transform.position = latestPos;
            transform.rotation = latestRot;
        }

        // Gestion du cas o� la position du joueur est trop �loign�e, on applique un "t�l�port" direct
        if (Vector3.Distance(transform.position, latestPos) > teleportIfFarDistance)
        {
            transform.position = latestPos;
            transform.rotation = latestRot;
        }
    }

    // Pr�diction simple c�t� client pour am�liorer l'exp�rience
    private void PredictMovement()
    {
        // Simple pr�diction en ajoutant la vitesse au mouvement
        Vector3 predictedPosition = transform.position + velocity * Time.deltaTime;
        transform.position = predictedPosition;
    }

    // M�thode pour changer la vitesse du joueur (ex. � partir des entr�es utilisateur)
    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }
}
