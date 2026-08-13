using UnityEngine;

using UI.Inventory;

namespace Manager
{
    public class HubManager : MonoBehaviour
    {
        private void Start()
        {
            InventoryUI.Instance.SetTargetInventory(GameDataManager.Instance.LoadoutInventory);
            EquipUI.Instance.SetTargetInventory(GameDataManager.Instance.LoadoutEquipInventory);
            GameDataManager.Instance.Save();
        }
    }
}