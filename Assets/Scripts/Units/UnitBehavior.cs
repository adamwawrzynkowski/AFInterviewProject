using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AFSInterview.Units
{
    public class UnitBehavior : MonoBehaviour
    {
        // Assigned unit
        [SerializeField] private Unit inheritedUnit;

        private int currentUnitKingdomID;
        private int currentUnitTurnInterval;

        // Set Kingdom (team)
        public void SetKingdomID(int ID) => currentUnitKingdomID = ID;
        
        // Set unit color (based on the Kingdoms colors)
        public void SetUnitAppearance(Color color) => GetComponentInChildren<MeshRenderer>().material.color = color;

        // Set unit parameters
        // While setting this, new Unit will be created to avoid override problems
        public void SetUnit(Unit unit)
        {
            inheritedUnit = new Unit
            {
                Name = unit.Name,
                Health = unit.Health,
                Armor = unit.Armor,
                AttackInterval = unit.AttackInterval,
                AttackRange = unit.AttackRange,
                Damage = unit.Damage,
                Attributes = unit.Attributes,
                AdditionalDamages = unit.AdditionalDamages
            };

            // Get and set UI information
            uUI = GetComponent<UnitUI>();
            uUI.SetData(inheritedUnit.Name, inheritedUnit.Health, inheritedUnit.Armor, inheritedUnit.AttackRange, currentUnitKingdomID == 0 ? -90.0f : 90.0f);
            uUI.unitTurn.SetActive(false);
        }

        // Used for getting currently assigned unit
        public Unit GetUnit() { return inheritedUnit; }

        // This bool is used for checking if unit executed its moves
        private bool executed;
        public bool Executed() { return executed; }
        public void SetExecuted(bool exec) => executed = exec;

        // Lerp cache
        private bool positionAchieved = true;
        private Vector3 previousPosition;
        private Vector3 newPosition;
        private float lerpTimeElapsed;
        
        private bool alive = true;
        public bool Alive() { return alive; }
        
        private bool additionalMoveExecuted;
        private UnitUI uUI;
        
        // Function to execute unit's move
        // All moves begins from there
        public void Execute()
        {
            // Enable UI
            uUI.unitTurn.SetActive(true);
            
            // Reset lerp cache
            ResetLerp();
            
            // Delay execute to actually sse what is happening on the screen - we want to see the moves, right? :P
            StartCoroutine(DoDelayExecute());
        }

        // Reset lerp cache
        private void ResetLerp()
        {
            lerpTimeElapsed = 0.0f;
        }

        private void Update()
        {
            // Check if position is achieved
            // If not - move unit to the target position
            if (positionAchieved) return;
            if (lerpTimeElapsed < 2.5f)
            {
                transform.position = Vector3.Lerp(previousPosition, newPosition, lerpTimeElapsed / 2.5f);
                float distance = Vector3.Distance(transform.position, newPosition);
                if (distance <= 3.5f) positionAchieved = true;
                lerpTimeElapsed += Time.deltaTime;
            }
        }

        // Delay execute to actually sse what is happening on the screen - we want to see the moves, right? :P
        private IEnumerator DoDelayExecute()
        {
            yield return new WaitForSeconds(1.0f);
            
            // Check if unit can move
            if (currentUnitTurnInterval <= 0)
            {
                // Set unit's pause turns amount
                currentUnitTurnInterval = inheritedUnit.AttackInterval;
                
                // Check if unit has Fast attribute and if so - make pause duration shorter
                if (inheritedUnit.Attributes == UnitAttributes.Fast)
                {
                    currentUnitTurnInterval--;
                    if (currentUnitTurnInterval < 0) currentUnitTurnInterval = 0;
                    uUI.SetDynamicText("[Fast Attribute] Attack Interval subtracted by 1!");
                    
                    StartCoroutine(DoDelayMove());
                    yield break;
                }
                
                StartCoroutine(DoDelayMove());
            }
            else
            {
                // Unit can't move, display that in the UI
                uUI.SetDynamicText("Next turn in: " + currentUnitTurnInterval);
                
                // Check if unit has Mechanical attribute and if so - give unit some health while its waiting for turn
                if (inheritedUnit.Attributes == UnitAttributes.Mechanical)
                {
                    inheritedUnit.Health++;
                    uUI.SetDynamicText("[Mechanical Attribute] Health increased by 1! Unit is repairing...");
                    uUI.UpdateUI(inheritedUnit.Health, inheritedUnit.Armor);
                }
                
                // Subtract this turn from waiting list
                currentUnitTurnInterval--;
                
                // Finish execution
                StartCoroutine(Finish(0.5f, true));
            }
        }

        // Delay execute to actually sse what is happening on the screen - we want to see the moves, right? :P
        private IEnumerator DoDelayMove()
        {
            yield return new WaitForSeconds(2.0f);
            DoMove();
        }
        
        private void DoMove()
        {
            // Find all units on the map
            // This function is not best optimized, but for this purpose it's ok
            // If the game were bigger, FindObjectsOfType should be replaced
            UnitBehavior[] units = FindObjectsOfType<UnitBehavior>();
            for (int i = 0; i < units.Length; i++)
            {
                // Skip if currently checking unit is from the same Kingdom
                if (units[i].currentUnitKingdomID == currentUnitKingdomID)
                    continue;

                // Count distance to the enemy
                float distance = Vector3.Distance(transform.position, units[i].transform.position);
                
                // If enemy is too far, skip it
                if (distance > inheritedUnit.AttackRange) continue;
                
                // If enemy is in range, attack him
                StartCoroutine(DoAttack(units[i]));
                return;
            }
            
            // Will be executed if no enemy will be found
            // Display UI
            uUI.SetDynamicText("No range!");
            
            // Get random position on the battle area bounds to lerp to this position
            var battleArea = CombatManager.Instance.BattleArea;
            var bounds = battleArea.bounds;
            previousPosition = transform.position;
            newPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                0f,
                Random.Range(bounds.min.z, bounds.max.z)
            );
            positionAchieved = false;
            
            // Finish execution
            StartCoroutine(Finish());
        }

        private IEnumerator DoAttack(UnitBehavior unit)
        {
            yield return new WaitForSeconds(2.0f);
            Debug.Log(inheritedUnit.Name + " is attacking " + unit.inheritedUnit.Name);

            // Set standard damage value
            int damage = inheritedUnit.Damage;
            
            // Check if unit has any additional damage attributes
            if (inheritedUnit.AdditionalDamages != null)
            {
                if (inheritedUnit.AdditionalDamages.Length > 0)
                {
                    foreach (var additionalDamages in inheritedUnit.AdditionalDamages)
                    {
                        // Check if unit's additional damage attributes matches with enemy attributes
                        if (additionalDamages.Against.HasFlag(unit.inheritedUnit.Attributes))
                        {
                            uUI.SetDynamicText("[Weak Target] This unit has additional damage against enemy's attribute!");
                            
                            // Add additional damage
                            damage += additionalDamages.AdditionalDamageValue;
                            break;
                        }
                    }
                }
            }

            // Deal damage
            unit.Damage(damage);
            
            // Finish execution
            StartCoroutine(Finish());
        }

        private void Damage(int damage)
        {
            string additionalText = "";
            bool ignoreHealth = false;
            
            // Make sure damage is always 1 or greater
            if (damage <= 0) damage = 1;
            
            // Check if unit has armor and if so - deal damage to armor instead of health
            if (inheritedUnit.Armor > 0)
            {
                // Check if unit has Armored attribute and if so - divide dealt damage by two
                if (inheritedUnit.Attributes == UnitAttributes.Armored)
                    damage /= 2;
                
                // Check if damage is not less than 1 after divide 
                if (damage < 1) damage = 1;
                
                // Deal damage to armor
                inheritedUnit.Armor -= damage;
                ignoreHealth = true;
                additionalText += "\n[Armored Attribute] Damage reduced by half!";
            }
            
            // Make sure armor value is not negative
            if (inheritedUnit.Armor < 0)
            {
                // If armor value is negative, make it 0 and subtract the difference from health
                inheritedUnit.Health -= -inheritedUnit.Armor;
                inheritedUnit.Armor = 0;
                ignoreHealth = true;
            }

            // Deal damage to health if armor is not used
            if (!ignoreHealth) inheritedUnit.Health -= damage;
            
            // Update UI values
            uUI.UpdateUI(inheritedUnit.Health, inheritedUnit.Armor);
            uUI.SetDynamicText("-" + damage + additionalText, 1.5f);
            
            // If health is 0 or less - kill the unit
            if (inheritedUnit.Health <= 0)
            {
                Kill();
            }
        }

        private void Kill()
        {
            // Remove unit from list and reconstruct it
            CombatManager.Instance.RemoveUnitFromTurnOrder(this);
            CombatManager.Instance.ReconstructIterations();
            
            // Destroy UI references
            Destroy(uUI.unitTurn);
            Destroy(uUI.UnitDynamicText.gameObject);

            // Set unit as dead
            alive = false;
            SetExecuted(true);
            
            // Destroy object
            DestroyUnit();
        }

        public void DestroyUnit()
        {
            Destroy(gameObject);
        }
        
        private IEnumerator<WaitForSeconds> Finish(float OverrideTime = -1, bool isResting = false)
        {
            // Check if unit has Light attribute and if so - give unit ability to perform one more move in this turn
            if (inheritedUnit.Attributes == UnitAttributes.Light && !additionalMoveExecuted && !isResting)
            {
                uUI.SetDynamicText("[Light Attribute] This unit can perform additional move during this turn!");
                currentUnitTurnInterval = 0;
                additionalMoveExecuted = true;
                StartCoroutine(DoDelayMove());
                yield break;
            }
            
            // End execution
            yield return new WaitForSeconds(Math.Abs(OverrideTime - (-1)) < 1.0 ? 4.0f : OverrideTime);
            SetExecuted(true);
            additionalMoveExecuted = false;
            uUI.unitTurn.SetActive(false);
        }
    }
}
