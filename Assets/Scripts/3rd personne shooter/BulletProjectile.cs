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
            // Si le joueur est propri�taire, il peut supprimer directement l'objet
            PhotonNetwork.Destroy(gameObject);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            // V�rifier si l'objet a un propri�taire et si celui-ci est encore pr�sent
            if (photonView.Owner != null && photonView.Owner.CustomProperties.Count > 0)
            {
                // Transf�rer la propri�t� � un autre joueur si n�cessaire
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
            else
            {
                // Si le propri�taire a quitt� la partie, supprimer l'objet
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

}
