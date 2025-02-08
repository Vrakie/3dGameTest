using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
public class health : MonoBehaviour, IDamageable
{
    #region Variable
    public float life = 100f;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Slider healthSlider;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    #endregion
    private void Awake()
    {
        photonView = this.GetComponent<PhotonView>();
        healthSlider = this.GetComponentInChildren<Slider>();
    }
    private void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
        healthSlider.value = life;
    }
    [PunRPC]
    void UpdateHealthBar(float currentHealth) => healthSlider.value = currentHealth;
    public void TakeDamage(float damage)
    {
        life -= damage;
        photonView.RPC("UpdateHealthBar", RpcTarget.AllBuffered, life);
        if (life <= 0)
            photonView.RPC("Respawn", RpcTarget.AllBuffered);
    }
    [PunRPC]
    private void Respawn()
    {
        life = 100f;
        if (healthSlider != null) healthSlider.value = life;
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
    }
}
