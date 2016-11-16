using UnityEngine;
using System.Collections;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
public class Player : LivingEntity {
    // Player class is exclusively for taking input from the person playing the game.
    // Use PlayerController for manipulating the Player object in the game world.

    public float moveSpeed = 5;

    Camera viewCamera; // For raycasting to get look direction
    PlayerController controller; // The controller (which controls the player object)
    GunController gunController; // The gun controller (which controls equipping weapons)

	protected override void Start () {
        base.Start(); // Run Start() from LivingEntity parent class
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
	}
	
	void Update () {
        // Movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); // Get the input from the player
        Vector3 moveVelocity = moveInput.normalized * moveSpeed; // Normalise so magnitude is 1 (leaving only direction)
        controller.Move(moveVelocity); // Pass direction to Move() on the controller

        // Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition); // Create a ray from the camera to the mouse position
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // The plane that intersects with the ray (note: not the actual ground plane, this is generated only for this script)
        float rayDistance; // For storing the length of the ray when intersected

        if (groundPlane.Raycast(ray, out rayDistance)) { 
            Vector3 point = ray.GetPoint(rayDistance); // The point in space where the ray intersects with the plane
            Debug.DrawLine(ray.origin, point, Color.red); // Draw the ray up to the point of intersection (debug code)
            controller.LookAt(point); // Rotate the player object
        }

        // Weapon input
        if (Input.GetMouseButton(0)) {
            gunController.Shoot(); // Shoot the gun when you press LMB
        }
	}
}
