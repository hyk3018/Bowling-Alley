using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] Ball ball = null;
    [SerializeField] GameObject pinsPrefab = null;
    [SerializeField] TextMeshProUGUI totalScoreText = null;
    [SerializeField] TextMeshProUGUI scoreThisRoundText = null;
    
    // Usually I use event Action to decouple the
    // subscriber but I'm trying UnityEvent out here
    // Can't seem to find a way to subscribe to events
    // from the listener's side but if it's possible
    // I would like to know.

    public static GameManager Instance => _instance;

    public UnityEvent OnStrike;
    public UnityEvent OnSpare;
    public UnityEvent OnScore;

    static GameManager _instance;
    GameObject _currentPins;
    int _score;
    int _totalPins;
    int _currentPinsDropped;
    Coroutine _endGame;

    void Awake()
    {
        if (_instance == null || _instance != this)
            _instance = this;
    }

    void Start()
    {
        ball.playing = true;
        ball.BowledTwice += () =>
        {
            if (_endGame == null)
                _endGame = StartCoroutine(EndGame());
        };
        ResetPins();
        totalScoreText.text = "Score : " + _score;
    }

    // Delete current pins and respawns 
    // We could reposition them instead - that requires
    // storing initial positions, rotations and
    // setting rigidbody velocities to 0 
    void ResetPins()
    {
        _totalPins = 0;
        _currentPinsDropped = 0;
        
        if (_currentPins != null)
            Destroy(_currentPins.gameObject);

        _currentPins = Instantiate(pinsPrefab);

        foreach (Transform pinTransform in _currentPins.transform)
        {
            _totalPins++;
            Pin pin = pinTransform.GetComponentInChildren<Pin>();
            pin.PinDropped += () =>
            {
                _currentPinsDropped++;
                if (_currentPinsDropped == _totalPins)
                { 
                    if (_endGame == null)
                        _endGame = StartCoroutine(EndGame());
                }
            };
        }
    }

    IEnumerator EndGame()
    {
        ball.playing = false;

        if (_currentPinsDropped == _totalPins)
        {
            if (ball.BowlCount == 2)
                OnSpare?.Invoke();
            else
                OnStrike?.Invoke();
        }
        else
        {
            scoreThisRoundText.text = _currentPinsDropped.ToString();
            OnScore?.Invoke();
        }

        _score += _currentPinsDropped;
        totalScoreText.text = "Score : " + _score;

        yield return new WaitForSeconds(2);

        ResetPins();
        ball.Reset();
        ball.BowlCount = 0;
        ball.playing = true;
        _endGame = null;
    }

    public void SetPause(bool pause)
    {
        ball.playing = !pause;
    }
}