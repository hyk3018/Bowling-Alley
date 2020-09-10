using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OpenClosePanelWithDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] float openX = 1080;
    [SerializeField] float closeX = 0;
    [SerializeField] float openCloseThreshold = 600;
    [SerializeField] float openCloseSpeed = 10;

    public UnityEvent UIOpen;
    public UnityEvent UIClose;

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameManager.Instance.SetPause(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(eventData.position.x, transform.position.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (transform.position.x > openCloseThreshold)
        {
            StartCoroutine(MoveToOpen());
        } else if (transform.position.x < openCloseThreshold)
        {
            StartCoroutine(MoveToClose());
        }
    }

    IEnumerator MoveToOpen()
    {
        while (transform.position.x < openX)
        {
            transform.position += Vector3.right * openCloseSpeed;
            yield return null;
        }

        transform.position = new Vector3(openX, transform.position.y);
    }
    IEnumerator MoveToClose()
    {
        while (transform.position.x > closeX)
        {
            transform.position += Vector3.left * openCloseSpeed;
            yield return null;
        }

        transform.position = new Vector3(closeX, transform.position.y);
        GameManager.Instance.SetPause(false);
    }
    
}
