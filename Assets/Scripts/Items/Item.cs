namespace AFSInterview.Items
{
	using System;
	using UnityEngine;
	
	[Serializable]
	public class Item
	{
		[SerializeField] private string name;
		[SerializeField] private int value;
		
		[Space]
		[SerializeField] private bool consumable;
		
		[Space]
		[SerializeField] private bool addMoney;
		[SerializeField] private int addMoneyValue;
		
		[Space]
		[SerializeField] private bool addItem;
		[SerializeField] private ItemScriptable addItemHolder;
		[SerializeField] private int addItemNum;
		
		[Space]
		[SerializeField] private bool consume;
		[SerializeField] private bool consumed;

		public string Name => name;
		public int Value => value;
		public bool Consumed => consumed;
		
		public void Use()
		{
			// Edit
			// This function is called by clicking on the Unity's Inspector button.
			// Currently there is no any in-game system that allows to
			// consume items by using in-game UI.
			
			// If consumable has option to give money - give it
			if (addMoney) InventoryController.Instance.Money += addMoneyValue;
			
			// If consumable has option to give item(s) - give it/them
			// This function will iterate by amount
			if (addItem)
			{
				for (var i = 0; i < addItemNum; i++)
				{
					InventoryController.Instance.AddItem(addItemHolder.item);
				}
			}
			
			// Remove item from Inventory after use
			InventoryController.Instance.RemoveItem(this);
			Debug.Log("Using " + Name);
		}
	}
}