using UnityEngine;

public class FollowBall : MonoBehaviour
{
    [SerializeField] Transform ball = null;
    [SerializeField] float followZOffset;
    [SerializeField] float zMax = -20f;
    
    void Start()
    {
        followZOffset = transform.position.z - ball.position.z;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 
            Mathf.Min(ball.position.z + followZOffset, zMax));
    }
}
