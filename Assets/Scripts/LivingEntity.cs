using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable { // This is a parent class for anything "living", i.e. can (at least) take and (optionally) deal damage.

    public float startingHealth;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start() {
        health = startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit) {
        // Use RaycastHit for things
        TakeDamage(damage); // Calls TakeDamage() for convenience
    }

    public void TakeDamage(float damage) {
        health -= damage;
        
        if (health <= 0 && !dead) // Die if you run out of health
        {
            Die();
        }
    }

    protected void Die() {
        dead = true;
        if (OnDeath != null) {
            OnDeath(); // Broadcast an event when you die
        }

        GameObject.Destroy(gameObject); // Destroy your game object once you have died
    }
}
