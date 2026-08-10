using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.HUD
{
    public class WeightDisplayUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text weightText;
        [SerializeField] private Slider weightSlider;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color overweightColor = new Color(1f, 0.4f, 0.4f);

        private Core.System.Inventory targetInventory;

        private void OnDestroy()
        {
            if(targetInventory != null)
            {
                targetInventory.OnInventoryChanged -= Refresh;
                targetInventory = null;
            }
        }

        public void SetTarget(Core.System.Inventory inventory)
        {
            if (targetInventory != null) targetInventory.OnInventoryChanged -= Refresh;

            targetInventory = inventory;
            targetInventory.OnInventoryChanged += Refresh;

            Refresh();
        }

        private void Refresh()
        {
            float current = targetInventory.CurrentWeight;
            float max = targetInventory.MaxWeight;

            weightText.text = $"{current:0.#} / {max:0.#}";
            weightText.color = targetInventory.IsOverweight ? overweightColor : normalColor;

            if (weightSlider != null)
            {
                weightSlider.value = max > 0f ? Mathf.Clamp01(current / max) : 0f;
            }
        }
    }
}