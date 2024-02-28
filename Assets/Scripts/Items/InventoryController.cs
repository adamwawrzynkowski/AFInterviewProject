using System;

namespace AFSInterview.Items
{
	using System.Collections.Generic;
	using UnityEngine;

	public class InventoryController : MonoBehaviour
	{
		public static InventoryController Instance;
		private void Awake() => Instance = this;

		[SerializeField] private List<Item> items;
		[SerializeField] private int money;
		
		public int Money { get => money; set => money = value; }
		public int ItemsCount => items.Count;

		public void SellAllItemsUpToValue(int maxValue)
		{
			for (var i = 0; i < items.Count; i++)
			{
				var itemValue = items[i].Value;
				if (itemValue > maxValue)
					continue;

				Money += itemValue;
				items.RemoveAt(i);
				
				// Edit - BugFix
				// We need to subtract index by one because list will re-sort after RemoveAt() and it will no longer
				// be equal real item index.
				i--;
			}
		}

		// This function is used by ItemEditor.cs to refresh items when user click on the Inspector's Consume button
		public void RefreshItems()
		{
			for (var i = 0; i < ItemsCount; i++)
			{
				// Check if item has been consumed and call Use() function if so
				if (items[i].Consumed)
				{
					items[i].Use();
				}
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