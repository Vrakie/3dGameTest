using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Ennemi : MonoBehaviour, IDamageable
{
    public float health = 100f;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Slider healthSlider;

    private Vector3 spawnPosition;  // Position de r�apparition
    private Quaternion spawnRotation;  // Rotation de r�apparition

    private void Awake()
    {
        photonView = this.GetComponent<PhotonView>();
        healthSlider = this.GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        spawnPosition = transform.position;  // Enregistre la position de spawn de l'ennemi
        spawnRotation = transform.rotation;  // Enregistre la rotation initiale de l'ennemi
        healthSlider.value = health;
    }

    // M�thode RPC pour mettre � jour la barre de vie
    [PunRPC]
    void UpdateHealthBar(float currentHealth)
    {
        healthSlider.value = currentHealth;  // Met � jour la barre de vie avec la valeur actuelle de la sant�
    }

    // M�thode pour recevoir les d�g�ts
    public void TakeDamage(float damage)
    {
        health -= damage;

        // Appel RPC pour synchroniser la mise � jour de la barre de vie sur tous les clients
        photonView.RPC("UpdateHealthBar", RpcTarget.AllBuffered, health);

        if (health <= 0)
        {
            // R�initialise la sant� sans d�sactiver l'objet
            photonView.RPC("Respawn", RpcTarget.AllBuffered);  // Appel RPC pour r�initialiser la sant� et la position
        }
    }

    // M�thode RPC pour d�marrer la r�apparition
    [PunRPC]
    private void Respawn()
    {
        // R�initialise les propri�t�s de l'ennemi
        health = 100f;  // R�initialise la sant� de l'ennemi
        if (healthSlider != null) healthSlider.value = health;  // R�initialise la barre de vie

        // R�initialise la position et la rotation de l'ennemi sans d�sactiver l'objet
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;  // R�initialise la rotation � la valeur de spawn

        Debug.Log("L'ennemi a r�apparu !");
    }
}
