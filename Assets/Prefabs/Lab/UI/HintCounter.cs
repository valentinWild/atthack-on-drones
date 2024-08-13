using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintCounter : MonoBehaviour
{
    [SerializeField] private int _hintCounter = 0;
    private TextMeshProUGUI _text;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        UpdateText();  // Initial update
    }

    public void IncreaseCounter()
    {
        _hintCounter++;
        Debug.Log("Increased Hint Counter to: " + _hintCounter);
        UpdateText();
    }

    private void UpdateText()
    {
        if (_text != null)
        {
            _text.text = _hintCounter.ToString();
            Debug.Log("Set Hint Counter to " + _hintCounter);
        }
    }

    public void ResetCounter()
    {
        _hintCounter = 0;
        UpdateText();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateText();
        }
    }
}
