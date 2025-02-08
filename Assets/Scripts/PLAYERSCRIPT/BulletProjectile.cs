using UnityEngine;
using Photon.Pun;

public class BulletProjectile : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private LayerMask layermasks;
    private PhotonView photonView;
    #endregion
    private void Awake() => photonView = GetComponent<PhotonView>();
    private void OnTriggerEnter(Collider other)
    {
        if ((layermasks.value & (1 << other.gameObject.layer)) != 0)
        {
            IDamageable damageableObject = other.GetComponent<IDamageable>();
            if (damageableObject != null)
                damageableObject.TakeDamage(10);
            TryDestroyProjectile();
        }
    }
    private void TryDestroyProjectile()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
        else if (PhotonNetwork.IsMasterClient)
        {
            if (photonView.Owner != null && photonView.Owner.CustomProperties.Count > 0)
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            else
                PhotonNetwork.Destroy(gameObject);
        }
    }
}
