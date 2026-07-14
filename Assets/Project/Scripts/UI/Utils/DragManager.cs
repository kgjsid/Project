using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using UI.Inventory;

public class DragManager : MonoBehaviour
{
    private static DragManager instance;

    public SlotUI currentDragSlotUI;
    public GameObject dragIconPrefab;   // 드래그 시 생성될 임시 프리팹
    public Transform renderContent;     // Image가 렌더링 될 캔버스

    private GameObject currentDragIcon;
    private RectTransform dragRect;

    public static DragManager Instance { get { return instance; } }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void StartDrag(SlotUI slot)
    {
        currentDragSlotUI = slot;

        currentDragIcon = Instantiate(dragIconPrefab, renderContent);
        dragRect = currentDragIcon.GetComponent<RectTransform>();

        Image dragIcon = currentDragIcon.GetComponent<Image>();
        dragIcon.sprite = slot.itemIcon.sprite;
        dragIcon.raycastTarget = false;

        UpdateDrag();
    }

    public void UpdateDrag()
    {
        if (currentDragIcon == null) return;
        currentDragIcon.transform.position = Mouse.current.position.ReadValue();
    }

    public void EndDrag()
    {
        if (currentDragIcon != null)
        {
            Destroy(currentDragIcon.gameObject);
            currentDragIcon = null;
        }
        currentDragSlotUI = null;
    }
}
