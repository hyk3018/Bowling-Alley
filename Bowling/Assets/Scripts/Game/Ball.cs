using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ball : MonoBehaviour
{
    [SerializeField] Vector3 ballInitialPosition = default;
    [SerializeField] float baseThrowSpeed = 200;
    [SerializeField] float bowlAngleLimit = 30;
    [SerializeField] float startDepth = 10;
    [SerializeField] float startY = 1;
    [SerializeField] float minimumSpeedBeforeStop;

    public event Action BowledTwice;
    
    // A different way to update the UI. Decouples logic with UI or anything
    // that cares about bowl count changing
    public event Action<int> BowlCountChange;

    static readonly Vector2 bowlStraight = Vector2.up;
    
    Rigidbody _rigidbody;
    
    // Bowling mechanic - only 2 bowls
    public int BowlCount
    {
        get => _bowlCount;
        set
        {
            _bowlCount = value;
            BowlCountChange?.Invoke(Mathf.Min(value + 1, 2));
        }
    }

    int _bowlCount;
    
    // Tracking ball state
    Vector3 _bowlStartPosition;
    float _bowlStartTime;
    bool _ballThrown;
    
    // Flagging throw from input in Update to FixedUpdate in current frame
    bool _ballThrownThisFrame;
    Vector3 _throwVelocity;
    public bool playing;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Reset();
        BowlCount = 0;
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Backboard"))
            Reset();
    }

    public void Reset()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = ballInitialPosition;
        _ballThrown = false;
        BowlCount++;
        
        if (BowlCount >= 2)
            BowledTwice?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        // In case we stop moving and don't touch wall
        minimumSpeedBeforeStop = 0.1f;
        if (_rigidbody.velocity.magnitude < minimumSpeedBeforeStop && _ballThrown)
            Reset();
        
        if (playing)
            HandleInput();
    }

    void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        // Bowling is determined by the velocity you flick the finger so we need to know
        // the distance the flick covers and how long the flick lasts
        if (Input.touchCount > 0 && !_ballThrown)
        {
            Touch touchData = Input.GetTouch(0);
            Vector3 position = new Vector3(touchData.position.x, touchData.position.y, startDepth);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(position);

            if (touchData.phase == TouchPhase.Began)
            {
                _rigidbody.isKinematic = true;
                _bowlStartTime = Time.time;
                _bowlStartPosition = position;
                transform.localPosition = new Vector3(worldPos.x, startY, startDepth);
            }
            else if (touchData.phase == TouchPhase.Ended)
            {
                CalculateThrowVelocity(position);
                _ballThrownThisFrame = true;
            }
            else if (touchData.phase == TouchPhase.Stationary)
            {
                _bowlStartTime = Time.time;
                _bowlStartPosition = position;
                startDepth = transform.localPosition.z;
            }
            else
            {
                float ballDepthDelta = position.y - _bowlStartPosition.y;
                transform.localPosition =
                    new Vector3(worldPos.x, transform.localPosition.y, ballDepthDelta / 100 + startDepth);
            }
        }
    }

    void CalculateThrowVelocity(Vector3 throwEndPosition)
    {
        Vector3 bowlDirection = throwEndPosition - _bowlStartPosition;
        if (Vector2.Angle(bowlStraight, bowlDirection) > bowlAngleLimit)
        {
            bowlDirection = new Vector2(Mathf.Cos(bowlAngleLimit * Mathf.Deg2Rad),
                Mathf.Sin(bowlAngleLimit * Mathf.Deg2Rad));
            if (bowlDirection.x < 0)
                bowlDirection.x = -bowlDirection.x;
        }

        float throwTime = Mathf.Max(0.25f, Time.time - _bowlStartTime);
        _throwVelocity = bowlDirection.normalized * baseThrowSpeed / throwTime;
    }

    void FixedUpdate()
    {
        if (_ballThrownThisFrame)
        {
            _rigidbody.isKinematic = false;
            _ballThrown = true;
            _ballThrownThisFrame = false;
            _rigidbody.AddForce(new Vector3(_throwVelocity.x, 0, _throwVelocity.y));
        }
    }
}