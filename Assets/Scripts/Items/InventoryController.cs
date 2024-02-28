namespace AFSInterview.Items
{
	using System.Collections.Generic;
	using UnityEngine;

	public class InventoryController : MonoBehaviour
	{
		public static InventoryController Instance { get; } = new();
		[SerializeField] private List<Item> items;
		[SerializeField] private int money;

		public List<Item> Items => items;
		public int Money { get; set; }
		public int ItemsCount => items.Count;

		public void SellAllItemsUpToValue(int maxValue)
		{
			for (var i = 0; i < items.Count; i++)
			{
				var itemValue = items[i].Value;
				if (itemValue > maxValue)
					continue;

				money += itemValue;
				items.RemoveAt(i);
				
				// Edit - BugFix
				// We need to subtract index by one because list will re-sort after RemoveAt() and it will no longer
				// be equal real item index.
				i--;
			}
		}

		public void AddItem(Item item)
		{
			items.Add(item);
		}

		public void RemoveItem(Item item)
		{
			items.RemoveAt(items.IndexOf(item));
		}
	}
}