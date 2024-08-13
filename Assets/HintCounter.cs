using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HintCounter : MonoBehaviour
{
    private int _hintCounter = 0;
    private TextMeshProUGUI _text;
    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }
    private void OnDestroy()
    {
    }

    private void ResetCounter()
    {
        _hintCounter = 0;
        UpdateText();
    }


    public void IncreaseCounter()
    {
        _hintCounter++;
        Debug.Log("Increased Hint Counter");
        UpdateText();
    }

    private void UpdateText()
    {
        _text.text = _hintCounter.ToString();
    }
}