using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Make sure to include this for TextMeshPro

public class StoryIntroManager : MonoBehaviour
{
    public GameObject startGameCanvas;
    public GameObject skipCanvas;
    public Button skip;

    public AudioSource intro1;
    public AudioSource intro2;

    private Coroutine firstAudioCoroutine;
    private Coroutine secondAudioCoroutine;
    private bool isSkipped = false; // Flag to check if skip has been pressed

    // private string oldstory = "Hello, traitor. Yes, we're talking to you. You wonder why you're stuck in here? Well, let us assure you, if you're looking for an exit,\" +\r\n        \" you're in no luck. You tried to break into our main server and deactivate our system, and you thought you'd just get through with it? Hah, fat chance.\" +\r\n        \" We'll tell you what happens now: Either you stay in here, and rot the rest of your pathetic life away, or you and your friend in the other cell give us \" +\r\n        \"one last performance before your inevidable death. We'll give you one more try to escape - just for our amusement. ";
    private string story = "Hello, friends. Are you awake yet? Man, that was a close one, what you and your friend just pulled here! You tried to break into our" +
        " main server and deactivate the whole system to kill all Murder Drones? Well, let's just say you were very lucky we found you before they managed to kill you!" +
        " Why are you looking at me like this? I may look like a Murder Drone, but I assure you, I'm not! Can you see the blue light in my middle? " +
        "I'm one of the friendly drones, I would never hurt you! In fact, I can try to give you both one last shot at getting into our main server - " +
        "I want the Murder Drones gone just as much as you! It will be dangerous, though, so be careful! " +
        "One of you will get sent into our server corridors, where you have to run and try to find the main server, whilest trying to to get shot at " +
        "from the Murder Drones. We'll be there as well - if you see one of us, try to catch us to get a hint, where the main server is hidden! We'll bring " +
        "the other one in our lab room, where you will recieve all collected hints and try to decode them before the other one in the corridors dies. To help " +
        "them survive, the lab person will be able to create potions which we will bring to the person in the corridor. Ok, no time to loose - time to decide " +
        "what role each of you wants to have!";
    // Start is called before the first frame update
    void Start()
    {
        startGameCanvas.gameObject.SetActive(false);
        skipCanvas.gameObject.SetActive(true);
        skip.onClick.AddListener(SkipIntro); // Attach the SkipIntro function to the button click event
        firstAudioCoroutine = StartCoroutine(FirstAudio());
    }

    IEnumerator FirstAudio()
    {
        intro1.Play();
        yield return new WaitWhile(() => intro1.isPlaying && !isSkipped); // Wait while audio is playing and not skipped
        if (!isSkipped) // Only start the second coroutine if not skipped
        {
            secondAudioCoroutine = StartCoroutine(SecondAudio());
        }
    }

    IEnumerator SecondAudio()
    {
        intro2.Play();
        yield return new WaitWhile(() => intro2.isPlaying && !isSkipped); // Wait while audio is playing and not skipped
        if (!isSkipped) // Only change UI if not skipped
        {
            skipCanvas.gameObject.SetActive(false);
            startGameCanvas.gameObject.SetActive(true);
        }
    }

    void SkipIntro()
    {
        isSkipped = true; // Set the skip flag to true

        // Stop both coroutines if they're running
        if (firstAudioCoroutine != null) StopCoroutine(firstAudioCoroutine);
        if (secondAudioCoroutine != null) StopCoroutine(secondAudioCoroutine);

        // Stop the audio if it's playing
        if (intro1.isPlaying) intro1.Stop();
        if (intro2.isPlaying) intro2.Stop();

        // Immediately show the start game canvas
        skipCanvas.gameObject.SetActive(false);
        startGameCanvas.gameObject.SetActive(true);
    }
}
