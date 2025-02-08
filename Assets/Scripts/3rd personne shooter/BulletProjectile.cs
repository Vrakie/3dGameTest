using UnityEngine;
using Photon.Pun;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private LayerMask layermasks;
    [SerializeField] private Rigidbody rb;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((layermasks.value & (1 << other.gameObject.layer)) != 0)
        {
            IDamageable damageableObject = other.GetComponent<IDamageable>();
            if (damageableObject != null)
            {
                damageableObject.TakeDamage(10);
            }

            TryDestroyProjectile();
        }
    }

    private void TryDestroyProjectile()
    {
        if (photonView.IsMine)
        {
            // Si le joueur est propriétaire, il peut supprimer directement l'objet
            PhotonNetwork.Destroy(gameObject);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            // Vérifier si l'objet a un propriétaire et si celui-ci est encore présent
            if (photonView.Owner != null && photonView.Owner.CustomProperties.Count > 0)
            {
                // Transférer la propriété à un autre joueur si nécessaire
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
            else
            {
                // Si le propriétaire a quitté la partie, supprimer l'objet
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

}
