using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Ennemi : MonoBehaviour, IDamageable
{
    public float health = 100f;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private Slider healthSlider;

    private Vector3 spawnPosition;  // Position de réapparition
    private Quaternion spawnRotation;  // Rotation de réapparition

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

    // Méthode RPC pour mettre à jour la barre de vie
    [PunRPC]
    void UpdateHealthBar(float currentHealth)
    {
        healthSlider.value = currentHealth;  // Met à jour la barre de vie avec la valeur actuelle de la santé
    }

    // Méthode pour recevoir les dégâts
    public void TakeDamage(float damage)
    {
        health -= damage;

        // Appel RPC pour synchroniser la mise à jour de la barre de vie sur tous les clients
        photonView.RPC("UpdateHealthBar", RpcTarget.AllBuffered, health);

        if (health <= 0)
        {
            // Réinitialise la santé sans désactiver l'objet
            photonView.RPC("Respawn", RpcTarget.AllBuffered);  // Appel RPC pour réinitialiser la santé et la position
        }
    }

    // Méthode RPC pour démarrer la réapparition
    [PunRPC]
    private void Respawn()
    {
        // Réinitialise les propriétés de l'ennemi
        health = 100f;  // Réinitialise la santé de l'ennemi
        if (healthSlider != null) healthSlider.value = health;  // Réinitialise la barre de vie

        // Réinitialise la position et la rotation de l'ennemi sans désactiver l'objet
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;  // Réinitialise la rotation à la valeur de spawn

        Debug.Log("L'ennemi a réapparu !");
    }
}
