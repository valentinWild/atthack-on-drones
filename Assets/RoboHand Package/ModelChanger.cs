using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;



public class ModelChanger : MonoBehaviour
{

    public int MNumber;
    public bool changed = false;
    public int ListLength;
    private bool MenuPressVal;
    private bool swapping = false;

    public XRController controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool MenuButton))
        {
            MenuPressVal = MenuButton;
            
        }
        if (MenuPressVal == false)
        {
            swapping = false;
        }


        MNumber = PlayerPrefs.GetInt("HandModel#");
        ModelCheck();
    }

    public void ModelCheck()
    {
        if (MenuPressVal == true && swapping == false)
        {

            GameObject rightHand = GameObject.Find("Right Hand Presence");
            RoboHand_Presence roboHandScript = rightHand.GetComponent<RoboHand_Presence>();
            ListLength = roboHandScript.LengthOfModelList;

            if (MNumber == ListLength)

            {
                MNumber = 0;
            }
            else
            {
                MNumber = (MNumber + 1);
            }

                PlayerPrefs.SetInt("HandModel#", MNumber);
            MenuPressVal = false;
            swapping = true;
        }

       
    }
}
