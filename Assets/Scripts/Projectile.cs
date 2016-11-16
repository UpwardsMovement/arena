using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public LayerMask collisionMask; // Bullets will only collide with objects in this layer
    float speed = 10; // Default bullet speed
    float damage = 1; // Default damage

    float lifetime = 3; // How long the bullet will exist for, in seconds
    public float skinWidth = 0.1f;

    void Start() {
        Destroy(gameObject, lifetime); // Start the countdown timer for despawn

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask); // Create an array of objects collided with
        if (initialCollisions.Length > 0) { // If there are any Collider objects in the array
            OnHitObject(initialCollisions[0]); // Then collide with the first one (this is imprecise, but as long as collidable objects don't ever overlap, it's fine)
        }
    }
    
    // Write a SetDamage() method for specifying damage by weapon
    
    public void SetSpeed(float newSpeed) { // This allows the weapon that spawns the projectile to specify speed (via muzzleVelocity)
        speed = newSpeed;
    }

	void Update () {
        float moveDistance = speed * Time.deltaTime; // The distance to be moved in this frame
        CheckCollisions(moveDistance); // Check for impact
        transform.Translate(Vector3.forward * moveDistance); // If the object still exists (i.e. didn't collide with anything), then move it forward
    }

    void CheckCollisions(float moveDistance) { 
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)) {
            OnHitObject(hit);
        }
    }

    void OnHitObject(RaycastHit hit) {
        print(hit.collider.gameObject.name);
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>(); // Query if the object you collide with can be damaged
        if (damageableObject != null) { // If you don't find something that can't be damaged
            damageableObject.TakeHit(damage, hit); // Then deal damage to the found object
        }
        GameObject.Destroy(gameObject); // Destroy the bullet after it deals damage
    }

    void OnHitObject(Collider c) {
        print(c.gameObject.name);
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
        GameObject.Destroy(gameObject);
    }
}
