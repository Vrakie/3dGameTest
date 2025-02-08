using UnityEngine;

public class Ennemi2 : MonoBehaviour, IDamageable
{
    public float health = 100f;

    public void TakeDamage(float damage)
    {

        health -= damage;
        if(health <=0 ) Debug.Log("test");
    }
}
