using UnityEngine;
using System.Collections;

using Core.System;
using Item.Data;
using Actors.UI;

namespace DebugTester
{
    public class InventoryTester : MonoBehaviour
    {
        public Inventory playerInventory;
        public ItemData itemA;
        public ItemData itemB;

        private void Start()
        {
            StartCoroutine(TestRoutine());

            playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
        }

        IEnumerator TestRoutine()
        {
            yield return new WaitUntil(() => playerInventory != null);

            playerInventory.AddItem(itemA, 1);
            playerInventory.AddItem(itemB, 5);

            if (InventoryUI.Instance.gameObject.activeSelf)
                InventoryUI.Instance.RefreshAll();

            Debug.Log("테스트 아이템 주입 완료!");
        }
    }
}
