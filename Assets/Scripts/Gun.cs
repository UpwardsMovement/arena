using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public Transform muzzle; // Bullet spawn location (and rotation)
    public Projectile projectile; // The projectile prefab to instantiate
    public float msBetweenShots = 100; // Firerate controller
    public float muzzleVelocity = 35; // The speed given to the bullet
    public float nextShotTime; // Time tracking for firerate

    public void Shoot() {
        if (Time.time > nextShotTime) // If the cooldown period is over
        {
            nextShotTime = Time.time + msBetweenShots / 1000; // 
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile; // Create a projectile and store in a variable for modification
            newProjectile.SetSpeed(muzzleVelocity); // Set the speed specified by the gun
        }
    }

}
