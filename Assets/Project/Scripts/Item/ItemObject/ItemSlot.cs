namespace Item.Data
{
    /// <summary>
    /// 실제 아이템 슬롯 하나에 대한 스크립트
    /// Item, Count를 저장
    /// </summary>
    public class ItemSlot
    {
        public ItemData item;
        public int count;

        public ItemSlot(ItemData item = null, int count = 0)
        {
            this.item = item;
            this.count = count;
        }

        public bool IsEmpty()
        {
            return item == null;
        }

        public void Clear()
        {
            item = null;
            count = 0;
        }
    }
}