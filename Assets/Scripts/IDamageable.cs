using UnityEngine;

public interface IDamageable {

    void TakeHit(float damage, RaycastHit hit); // For taking damage exclusively from projectiles

    void TakeDamage(float damage); // For taking damage in general

}