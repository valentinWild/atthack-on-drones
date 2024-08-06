using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimContr : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;

    public float radius = 0.1f; // Radius des Kreises, in dem sich die Drohne bewegt
    public float speed = 0.2f;  // Geschwindigkeit der Bewegung
    public float heightAmplitude = 0.05f; // Amplitude für die Höhenbewegung
    public float heightSpeed = 0.3f; // Geschwindigkeit für die Höhenbewegung

    private Vector3 startPosition;
    private float angle;
    void Start()
    {
        anim = GetComponent<Animator>();
        startPosition = transform.position; // Startposition der Drohne speichern
        angle = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Kreisbewegung um die Y-Achse
        angle += speed * Time.deltaTime;
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Höhenbewegung nach oben und unten
        float y = Mathf.Sin(Time.time * heightSpeed) * heightAmplitude;

        // Neue Position berechnen
        Vector3 newPosition = new Vector3(x, y, z) + startPosition;

        // Drohne an die neue Position setzen
        transform.position = newPosition;
    }
}
