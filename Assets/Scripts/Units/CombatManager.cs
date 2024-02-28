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

        private void Start()
        {
            // Spawn Red Army
            SpawnArmy(0, redKingdomArmy, redKingdomSpawnArea, redKingdomAppearance);
            
            // Spawn Blue Army
            SpawnArmy(1, blueKingdomArmy, blueKingdomSpawnArea, blueKingdomAppearance);
            
            // Initialize and Construct Turns
            ConstructTurn();
        }

        private void SpawnArmy(int kingdomID, List<Unit> army, BoxCollider spawnArea, Color appearance)
        {
            // Iterate through every unit
            foreach (var unit in army)
            {
                // Get unit model from Resources.
                // If unitModel is null, the unit will be skipped and won't be spawned.
                GameObject unitModel = Resources.Load<GameObject>("Models/Armies/" + unit.ResourcesModelName);
                if (unitModel == null)
                {
                    Debug.LogError("Can't add unit. ResourceModel doesn't exist. Skipping...");
                    continue;
                }

                // Set unit's position in the spawnArea bounds
                Bounds spawnAreaBounds = spawnArea.bounds;
                Vector3 spawnPosition = new Vector3(
                    Random.Range(spawnAreaBounds.min.x, spawnAreaBounds.max.x),
                    0f,
                    Random.Range(spawnAreaBounds.min.z, spawnAreaBounds.max.z)
                );

                // Create unit
                GameObject unitObject = Instantiate(unitModel, spawnPosition, spawnArea.transform.rotation);
                
                // Attach unit behavior controller script and configure it
                UnitBehavior behavior = unitObject.AddComponent<UnitBehavior>();
                behavior.SetKingdomID(kingdomID);
                behavior.SetUnit(unit);
                behavior.SetUnitAppearance(appearance);

                // Add newly created unit to the all units list
                turnOrderUnits.Add(behavior);
            }
        }

        private void ConstructTurn()
        {
            ShuffleTurnOrder();
            NextTurn();
        }

        // This function is used for randomize turns order
        // Function will randomize through all units, so the turn order will be set
        // regardless of which kingdom the unit comes from
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

        // Function used for skipping to the next turn
        private void NextTurn()
        {
            turn++;
            turnIteration = 0;
            NextIteration();
        }

        // Function used for skipping to the next unit in order
        private void NextIteration()
        {
            // Check if there are any units
            if (turnOrderUnits.Count <= 0)
            {
                turnInfo.text = "Finished!";
                return;
            }
            
            // Check if all units were iterated already and if so - go to the next turn
            turnIteration++;
            if (turnIteration - 1 >= turnOrderUnits.Count)
            {
                NextTurn();
                return;
            }
            
            // Get next unit and set UI information
            UnitBehavior unit = turnOrderUnits[turnIteration - 1];
            turnInfo.text = "Turn: " + turn + "\n" + "Current unit: " + unit.GetUnit().Name;
            
            // Execute the unit behavior
            unit.SetExecuted(false);
            unit.Execute();
            
            // Wait for unit to finish their tasks and all moves
            StartCoroutine(WaitForUnit(turnOrderUnits[turnIteration - 1]));
        }

        public void ReconstructIterations() => turnIteration--;

        // Wait for unit to finish their tasks and all moves
        private IEnumerator<WaitUntil> WaitForUnit(UnitBehavior unit)
        {
            yield return new WaitUntil(unit.Executed);
            if (!unit.Alive()) unit.DestroyUnit();
            if (turnOrderUnits.Count > 0 || turnOrderUnits != null) NextIteration();
        }
    }
}
