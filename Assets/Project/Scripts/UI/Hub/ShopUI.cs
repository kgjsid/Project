using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Core.System;
using Manager;

namespace UI.Hub
{
    public class ShopUI : MonoBehaviour
    {
        private static ShopUI instance;
        public static ShopUI Instance { get { return instance; } }

        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text debtText;
        [SerializeField] private TMP_Text sellPreviewText;
        [SerializeField] private Button sellAllButton;
        [SerializeField] private Button payDebtButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_InputField payAmountInput;

        private void Awake()
        {
            instance = this;

            sellAllButton.onClick.AddListener(SellAll);
            payDebtButton.onClick.AddListener(PayDebt);
            closeButton.onClick.AddListener(Close);
        }

        private void Start()
        {
            panel.SetActive(false);
        }

        public void Open()
        {
            panel.SetActive(true);
            Refresh();
        }

        public void Close()
        {
            panel.SetActive(false);
        }

        private void Refresh()
        {
            var data = GameDataManager.Instance;

            goldText.text = $"Gold: {data.Gold}";
            debtText.text = $"Debt: {data.DebtRemaining}   ({data.CurrentDay}Days)";
            sellPreviewText.text = $"SellPrev: {CalculateTotalValue()}";
        }

        private int CalculateTotalValue()
        {
            var loadout = GameDataManager.Instance.LoadoutInventory;
            int total = 0;

            foreach (var slot in loadout.Slots)
            {
                if (slot.IsEmpty()) continue;
                total += slot.item.sellPrice * slot.count;
            }

            return total;
        }

        private void SellAll()
        {
            var loadout = GameDataManager.Instance.LoadoutInventory;
            int total = CalculateTotalValue();

            if (total <= 0) return;

            for (int i = 0; i < loadout.Slots.Length; i++)
            {
                loadout.Slots[i].Clear();
            }
            loadout.NotifyChange();

            GameDataManager.Instance.AddGold(total);
            GameDataManager.Instance.Save();
            Refresh();
        }

        private void PayDebt()
        {
            if (!int.TryParse(payAmountInput.text, out int amount)) return;

            if (GameDataManager.Instance.TryPayDebt(amount))
            {
                GameDataManager.Instance.Save();
                Refresh();
            }
        }
    }
}