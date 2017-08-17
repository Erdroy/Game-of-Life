// Erdroy's Game of Life © 2016-2017 Damian 'Erdroy' Korczowski

using UnityEngine;
using UnityEngine.UI;

public class UISliderValue : MonoBehaviour
{
    public Text Slider;

    public void SetValue(float value)
    {
        Slider.text = value.ToString("f0");
    }
}
