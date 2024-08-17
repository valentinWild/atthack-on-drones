using System.Collections;
using UnityEngine;

public class PourDetector : MonoBehaviour
{
    public int pourThreshold = 45;
    public Transform origin = null;
    public GameObject streamPrefab = null;
    public AudioSource pouring; 

    private bool isPouring = false;
    private Stream currentStream = null;


    private void Update()
    {
        bool pourCheck = CalculatePourAngle() < pourThreshold; 

        if(isPouring != pourCheck)
        {
            isPouring = pourCheck;

            if(isPouring)
            {
                StartPour();
            }
            else
            {
                StopPour();
            }
        }
    }

    private void StartPour()
    {
        print("Start");
        currentStream = CreateStream();
        currentStream.Begin();
        pouring.Play();
    }

    private void StopPour()
    {
        print("End");
        currentStream.End();
        currentStream = null;
        pouring.Stop();
    }

    private float CalculatePourAngle()
    {
        return transform.forward.y * Mathf.Rad2Deg;
    }

    private Stream CreateStream() 
    { 
        GameObject streamObject = Instantiate(streamPrefab, origin.position, Quaternion.identity, transform);
        return streamObject.GetComponent<Stream>(); 
    }  
}