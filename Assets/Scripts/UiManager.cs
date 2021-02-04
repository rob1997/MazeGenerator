using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Slider width;
    public Slider length;

    [Space] public TextMeshProUGUI widthText;
    [Space] public TextMeshProUGUI lengthText;
    
    [Space]
    
    public Button generate;

    [Space] public MazeGenerator generator;

    private const int Limit = 45;

    private int _currentWidth;
    private int _currentLength;
    
    private void Start()
    {
        width.onValueChanged.AddListener(
            delegate
            {
                _currentWidth = Mathf.RoundToInt(width.normalizedValue * Limit);
                widthText.text = $"{_currentWidth}";
            });
        
        length.onValueChanged.AddListener(
            delegate
            {
                _currentLength = Mathf.RoundToInt(length.normalizedValue * Limit);
                lengthText.text = $"{_currentLength}";
            });
        
        generate.onClick.AddListener(delegate
        {
            for (int i = 0; i < generator.transform.childCount; i++)
            {
                Destroy(generator.transform.GetChild(i).gameObject);
            }
            
            generator.size = new IntVector2(_currentWidth, _currentLength);
            
            generator.Initialize();
            
            generator.Generate();
        });
    }
}
