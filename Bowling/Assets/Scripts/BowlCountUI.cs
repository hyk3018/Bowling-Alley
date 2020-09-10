using TMPro;
using UnityEngine;

public class BowlCountUI : MonoBehaviour
{
    [SerializeField] Ball ball = null;

    TextMeshProUGUI _textMeshProUGUI;
    
    void Start()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        ball.BowlCountChange += number => { _textMeshProUGUI.text = "Bowl Number : " + number; };
    }
}