using UnityEngine;
using UnityEngine.EventSystems;

public class DropMaterialSelect : MonoBehaviour, IDropHandler
{
    [SerializeField] MeshRenderer materialToChange = null;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        DragMaterialSelect droppedMaterialSelect = droppedObject.GetComponent<DragMaterialSelect>();
        if (droppedMaterialSelect)
        {
            materialToChange.material = droppedMaterialSelect.newMaterial;
        }
    }
}