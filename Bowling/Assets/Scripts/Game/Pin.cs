using System;
using UnityEngine;

public class Pin : MonoBehaviour
{
    [SerializeField] float fallThreshold = 0.2f;
    
    public event Action PinDropped;

    ParticleSystem _particleSystem;
    bool _toppled;
    bool _exploded;

    void OnCollisionEnter(Collision other)
    {
        if (!_exploded && other.gameObject.CompareTag("Player"))
        {
            _particleSystem.Play();
            _exploded = true;
        }
    }

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        GetComponent<Rigidbody>().centerOfMass = Vector3.up * 3f; 
    }

    void Update()
    {
        if (_toppled || !(transform.up.y < fallThreshold)) return;
        
        _toppled = true;
        PinDropped?.Invoke();
        Destroy(gameObject, 2);
    }
}