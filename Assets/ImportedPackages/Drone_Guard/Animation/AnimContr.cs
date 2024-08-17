using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimContr : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;

    public float moveAmplitude = 0.1f; // Amplitude der Bewegung in alle Richtungen
    public float moveSpeed = 0.2f;     // Geschwindigkeit der Bewegung
    public float initialHeightOffset = 0.5f; // Start-Höhenoffset für die Drohne

    private Vector3 startPosition;
    private Vector3 randomOffset;
    private float timeCounter = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        startPosition = transform.position + Vector3.up * initialHeightOffset;
        transform.position = startPosition;

        // Erstellen eines anfänglichen zufälligen Offsets für die Bewegung
        randomOffset = new Vector3(
            Random.Range(-moveAmplitude, moveAmplitude),
            Random.Range(-moveAmplitude, moveAmplitude),
            Random.Range(-moveAmplitude, moveAmplitude)
        );

    }

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime * moveSpeed;

        // Berechne die Bewegung in alle Richtungen um die Startposition herum
        Vector3 offset = new Vector3(
            Mathf.Sin(timeCounter) * randomOffset.x,
            Mathf.Sin(timeCounter) * randomOffset.y,
            Mathf.Sin(timeCounter) * randomOffset.z
        );

        // Setze die Drohne an die neue Position
        transform.position = startPosition + offset;
    }
}

/* Previous Code, Radiusförmige-Bewegung
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
    public float initialHeightOffset = 1.0f; //Start-Höhenoffset für die Drohne

    private Vector3 startPosition;
    private float angle;
    void Start()
    {
        anim = GetComponent<Animator>();
        startPosition = transform.position + Vector3.up * initialHeightOffset;
        angle = 0.0f;

        // Setze die Drohne sofort in die erhöhte Startposition
        transform.position = startPosition;
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
*/