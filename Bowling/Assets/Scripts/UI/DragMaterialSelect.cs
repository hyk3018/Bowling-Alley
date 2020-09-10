using UnityEngine;
using UnityEngine.EventSystems;

public class DragMaterialSelect : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Material newMaterial;

    Vector3 _initialPosition;
    CanvasGroup _canvasGroup;

    void Start()
    {
        _canvasGroup = GetComponentInParent<CanvasGroup>();
        _initialPosition = transform.localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(eventData.position.x, eventData.position.y);
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = _initialPosition;
        _canvasGroup.blocksRaycasts = true;
    }
}