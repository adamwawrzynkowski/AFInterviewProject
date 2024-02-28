using System;

namespace AFSInterview.Items
{
	using TMPro;
	using UnityEngine;

	public class ItemsManager : MonoBehaviour
	{
		[SerializeField] private InventoryController inventoryController;
		[SerializeField] private int itemSellMaxValue;
		[SerializeField] private Transform itemSpawnParent;
		[SerializeField] private GameObject itemPrefab;
		[SerializeField] private BoxCollider itemSpawnArea;
		[SerializeField] private float itemSpawnInterval;

		private float nextItemSpawnTime;
		
		// Camera
		private Camera mainCamera;
		
		// Money
		private TextMeshProUGUI moneyUIText;
		private int cachedMoney;

		private void Awake()
		{
			// Edit
			// Find main camera in the Awake()
			mainCamera = Camera.main;
			
			// Edit
			// FindObjectOfType is expensive so it should be called once instead of every frame.
			moneyUIText = FindObjectOfType<TextMeshProUGUI>();
		}

		private void Update()
		{
			if (Time.time >= nextItemSpawnTime)
				SpawnNewItem();
			
			if (Input.GetMouseButtonDown(0))
				TryPickUpItem();
			
			if (Input.GetKeyDown(KeyCode.Space))
				inventoryController.SellAllItemsUpToValue(itemSellMaxValue);

			// Edit
			// We don't need to assign new text every frame.
			// To not produce Garbage, the text should be changed only when Money amount will change.
			if (cachedMoney == inventoryController.Money) return;
			cachedMoney = inventoryController.Money;
			moneyUIText.text = "Money: " + inventoryController.Money;
		}

		private void SpawnNewItem()
		{
			nextItemSpawnTime = Time.time + itemSpawnInterval;
			
			var spawnAreaBounds = itemSpawnArea.bounds;
			var position = new Vector3(
				Random.Range(spawnAreaBounds.min.x, spawnAreaBounds.max.x),
				0f,
				Random.Range(spawnAreaBounds.min.z, spawnAreaBounds.max.z)
			);
			
			// Edit
			// Instantiate can be optimized by using Pools
			// This project is not huge enough to this call be a problem, but in the
			// larger projects, Pooler could be useful
			// Alternatively we can just use DOTS for high amount of objects.
			Instantiate(itemPrefab, position, Quaternion.identity, itemSpawnParent);
		}

		private void TryPickUpItem()
		{
			// Edit
			// Getting Camera.main every time is expensive. I changed this part so the main camera is
			// assigned and cached on Awake()
			var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
			var layerMask = LayerMask.GetMask("Item");
			if (!Physics.Raycast(ray, out var hit, 100f, layerMask) || !hit.collider.TryGetComponent<IItemHolder>(out var itemHolder))
				return;
			
			var item = itemHolder.GetItem(true);
            inventoryController.AddItem(item);
            Debug.Log("Picked up " + item.Name + " with value of " + item.Value + " and now have " + inventoryController.ItemsCount + " items");
		}
	}
}