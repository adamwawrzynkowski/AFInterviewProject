using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AFSInterview.Units
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;

        public void Awake()
        {
            Instance = this;
        }

        [Header("Area")]
        [SerializeField] private BoxCollider battleArea;
        
        [Header("Red Kingdom")]
        [SerializeField] private List<Unit> redKingdomArmy;
        [SerializeField] private BoxCollider redKingdomSpawnArea;
        [SerializeField] private Color redKingdomAppearance;
        
        [Header("Blue Kingdom")]
        [SerializeField] private List<Unit> blueKingdomArmy;
        [SerializeField] private BoxCollider blueKingdomSpawnArea;
        [SerializeField] private Color blueKingdomAppearance;
        
        [Header("UI")]
        [SerializeField] private TMP_Text turnInfo;

        public BoxCollider BattleArea => battleArea;

        private List<UnitBehavior> turnOrderUnits = new List<UnitBehavior>();
        public void RemoveUnitFromTurnOrder(UnitBehavior unit) => turnOrderUnits.Remove(unit);

        private int turn            = 0;
        private int turnIteration   = 0;
        private int turnIterations = 0;

        private void Start()
        {
            SpawnArmy(0, redKingdomArmy, redKingdomSpawnArea, redKingdomAppearance);
            SpawnArmy(1, blueKingdomArmy, blueKingdomSpawnArea, blueKingdomAppearance);
            ConstructTurn();
        }

        private void SpawnArmy(int kingdomID, List<Unit> army, BoxCollider spawnArea, Color appearance)
        {
            foreach (var unit in army)
            {
                GameObject unitModel = Resources.Load<GameObject>("Models/Armies/" + unit.ResourcesModelName);
                if (unitModel == null)
                {
                    Debug.LogError("Can't add unit. ResourceModel doesn't exist. Skipping...");
                    continue;
                }

                Bounds spawnAreaBounds = spawnArea.bounds;
                Vector3 spawnPosition = new Vector3(
                    Random.Range(spawnAreaBounds.min.x, spawnAreaBounds.max.x),
                    0f,
                    Random.Range(spawnAreaBounds.min.z, spawnAreaBounds.max.z)
                );

                GameObject unitObject = Instantiate(unitModel, spawnPosition, spawnArea.transform.rotation);
                UnitBehavior behavior = unitObject.AddComponent<UnitBehavior>();
                behavior.SetKingdomID(kingdomID);
                behavior.SetUnit(unit);
                behavior.SetUnitAppearance(appearance);

                turnOrderUnits.Add(behavior);
            }
        }

        private void ConstructTurn()
        {
            ShuffleTurnOrder();
            NextTurn();
        }

        private void ShuffleTurnOrder()
        {
            int listCount = turnOrderUnits.Count;
            int lastIndex = --listCount;
            for (int i = 0; i < lastIndex; ++i)
            {
                int random = Random.Range(i, listCount);
                (turnOrderUnits[i], turnOrderUnits[random]) = (turnOrderUnits[random], turnOrderUnits[i]);
            }
        }

        private void NextTurn()
        {
            turn++;
            turnIteration = 0;
            NextIteration();
        }

        private void NextIteration()
        {
            if (turnOrderUnits.Count <= 0)
            {
                turnInfo.text = "Finished!";
                return;
            }
            
            turnIteration++;
            turnIterations = turnOrderUnits.Count;
            if (turnIteration - 1 >= turnOrderUnits.Count)
            {
                NextTurn();
                return;
            }
            
            UnitBehavior unit = turnOrderUnits[turnIteration - 1];
            turnInfo.text = "Turn: " + turn + "\n" + "Current unit: " + unit.GetUnit().Name;
            unit.SetExecuted(false);
            unit.Execute();
            StartCoroutine(WaitForUnit(turnOrderUnits[turnIteration - 1]));
        }

        public void ReconstructIterations() => turnIteration--;

        private IEnumerator<WaitUntil> WaitForUnit(UnitBehavior unit)
        {
            yield return new WaitUntil(unit.Executed);
            if (!unit.Alive()) unit.DestroyUnit();
            if (turnOrderUnits.Count > 0 || turnOrderUnits != null) NextIteration();
        }
    }
}
