using UnityEngine.PlayerLoop;

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

		public string Name => name;
		public int Value => value;
		public bool Consumable => consumable;
		public bool AddMoney => addMoney;
		public bool AddItem => addItem;
		public int AddMoneyValue => addMoneyValue;
		public ItemScriptable AddItemHolder => addItemHolder;

		public Item(string name, int value, bool consumable = false, bool addMoney = false, bool addItem = false, int addMoneyValue = 0, ItemScriptable addItemHolder = null)
		{
			this.name = name;
			this.value = value;
			this.consumable = consumable;
			this.addMoney = addMoney;
			this.addItem = addItem;
			this.addMoneyValue = addMoneyValue;
			this.addItemHolder = addItemHolder;
		}

		public void Use()
		{
			if (addMoney) InventoryController.Instance.Money += addMoneyValue;
			if (addItem)
			{
				for (int i = 0; i < addItemNum; i++)
				{
					InventoryController.Instance.AddItem(addItemHolder.item);
				}
			}
			InventoryController.Instance.RemoveItem(this);
			Debug.Log("Using" + Name);
		}
	}
}