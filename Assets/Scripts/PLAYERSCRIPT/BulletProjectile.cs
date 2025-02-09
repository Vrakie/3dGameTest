using UnityEngine;
using Photon.Pun;
using UnityEngine.Animations;

public class BulletProjectile : MonoBehaviour, IPunInstantiateMagicCallback, IPunObservable
{
    [SerializeField] private LayerMask layermasks;
    private PhotonView photonView;
    [SerializeField] private ParentConstraint parentConstraint;
    private bool isConstraintActive = true;
    private bool lastConstraintState = true;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            isConstraintActive = true;
            lastConstraintState = isConstraintActive;
            photonView.RPC("SyncConstraintState", RpcTarget.AllBuffered, isConstraintActive);
        }
    }

    private void Update()
    {
        if (photonView.IsMine && parentConstraint.constraintActive != lastConstraintState)
        {
            lastConstraintState = parentConstraint.constraintActive;
            isConstraintActive = lastConstraintState;
            photonView.RPC("SyncConstraintState", RpcTarget.AllBuffered, isConstraintActive);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (photonView.IsMine)
        {
            isConstraintActive = true;
            photonView.RPC("SyncConstraintState", RpcTarget.AllBuffered, isConstraintActive);
        }
    }

    [PunRPC]
    private void SyncConstraintState(bool constraintState)
    {
        isConstraintActive = constraintState;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((layermasks.value & (1 << other.gameObject.layer)) != 0 && !isConstraintActive)
        {
            IDamageable damageableObject = other.GetComponent<IDamageable>();
            if (damageableObject != null)
                damageableObject.TakeDamage(100);

            TryDestroyProjectile();
        }
    }

    private void TryDestroyProjectile()
    {
        if (photonView == null || gameObject == null || isConstraintActive) return;

        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
        else if (PhotonNetwork.IsMasterClient)
        {
            if (photonView.Owner != null)
            {
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                StartCoroutine(DestroyAfterOwnershipTransfer());
            }
            else
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private System.Collections.IEnumerator DestroyAfterOwnershipTransfer()
    {
        yield return new WaitForSeconds(0.1f);
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isConstraintActive);
        }
        else
        {
            isConstraintActive = (bool)stream.ReceiveNext();
        }
    }
}
