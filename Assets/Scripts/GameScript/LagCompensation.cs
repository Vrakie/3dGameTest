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
    public Vector3 velocity; // Vitesse du joueur pour la prédiction côté client.
    #endregion

    void Awake()
    {
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
    }

    // Synchronisation des données du joueur (position et rotation)
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

    // FixedUpdate pour la compensation de la latence, interpolation et prédiction
    void FixedUpdate()
    {
        // Si c'est le joueur local, on applique la prédiction de mouvement directement
        if (photonView.IsMine)
        {
            PredictMovement();
            return;
        }

        // Calcule le temps jusqu'à ce que les données arrivent à destination
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
            // Si aucune interpolation n'est nécessaire, on applique directement la position/rotation reçues
            transform.position = latestPos;
            transform.rotation = latestRot;
        }

        // Gestion du cas où la position du joueur est trop éloignée, on applique un "téléport" direct
        if (Vector3.Distance(transform.position, latestPos) > teleportIfFarDistance)
        {
            transform.position = latestPos;
            transform.rotation = latestRot;
        }
    }

    // Prédiction simple côté client pour améliorer l'expérience
    private void PredictMovement()
    {
        // Simple prédiction en ajoutant la vitesse au mouvement
        Vector3 predictedPosition = transform.position + velocity * Time.deltaTime;
        transform.position = predictedPosition;
    }

    // Méthode pour changer la vitesse du joueur (ex. à partir des entrées utilisateur)
    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }
}
