using UnityEngine;
using UnityEngine.InputSystem;

public class DroneExplosion2 : MonoBehaviour
{
    public GameObject projectilePrefab; // Das Projektil-GameObject, das abgefeuert wird
    public Transform firePoint; // Der Punkt, von dem das Projektil abgefeuert wird (z.B. die Hand des Spielers)

    private PlayerInput playerInput;
    private InputAction shootAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>(); // PlayerInput auf demselben GameObject

        // Weisen Sie die Shoot-Aktion zu
        shootAction = playerInput.actions["Shoot"];
        shootAction.performed += OnShoot;
    }

    private void OnEnable()
    {
        shootAction.Enable(); // Aktiviert die Aktion
    }

    private void OnDisable()
    {
        shootAction.Disable(); // Deaktiviert die Aktion
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        FireProjectile(); // Methode zum Abfeuern des Projektils
    }

    void FireProjectile()
    {
        // Erzeuge das Projektil an der Position des firePoint
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Bestimme die Richtung des Projektils (in die Richtung, in die der Controller zeigt)
        ProjectileShoot projScript = projectile.GetComponent<ProjectileShoot>();
        if (projScript != null)
        {
            projScript.SetDirection(firePoint.forward); // Schieﬂt in die Richtung, in die der Controller zeigt
        }
    }
}