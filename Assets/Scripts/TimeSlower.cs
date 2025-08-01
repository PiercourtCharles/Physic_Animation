using TMPro;
using UnityEngine;

public class TimeSlower : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textDisplay;

    private void Start()
    {
        _textDisplay.text = @$"Time scale : {Mathf.Round(Time.timeScale * 100)}%";
    }

    public void Slow(float value)
    {
        if (Time.timeScale > 0.1f)
            Time.timeScale -= value;
        _textDisplay.text = @$"Time scale : {Mathf.Round(Time.timeScale * 100)}%";
    }

    public void Accelerate(float value)
    {
        Time.timeScale += value;
        _textDisplay.text = @$"Time scale : {Mathf.Round(Time.timeScale * 100)}%";
    }
}
