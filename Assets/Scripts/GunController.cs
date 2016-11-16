using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

    public Transform weaponHold; // The position the gun is held in (around the "hands" of the player)
    public Gun startingGun; // Can be set in the editor
    Gun equippedGun;

    void Start() {
        if (startingGun != null) { // If you have specified a weapon to start with
            EquipGun(startingGun); // Then equip it when the game starts
        }
    }

    public void EquipGun(Gun gunToEquip) {
        if (equippedGun != null) { // If you have a gun equipped
            Destroy(equippedGun.gameObject); // Then destroy it
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun; // Create a copy of the gun to be equipped
        equippedGun.transform.parent = weaponHold; // And make it a child of weaponHold
    }

    public void Shoot() {
        if (equippedGun != null) { // If you have a gun equipped
            equippedGun.Shoot(); // Then call its Shoot()x method
        }
    }
}
