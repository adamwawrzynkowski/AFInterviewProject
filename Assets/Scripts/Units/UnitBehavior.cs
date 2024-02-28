using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace AFSInterview.Units
{
    public class UnitBehavior : MonoBehaviour
    {
        [SerializeField] private Unit inheritedUnit;

        private int currentUnitKingdomID;
        private int currentUnitTurnInterval;

        public void SetKingdomID(int ID) => currentUnitKingdomID = ID;

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

            uUI = GetComponent<UnitUI>();
            uUI.SetData(inheritedUnit.Name, inheritedUnit.Health, inheritedUnit.Armor, inheritedUnit.AttackRange, currentUnitKingdomID == 0 ? -90.0f : 90.0f);
            uUI.unitTurn.SetActive(false);
        }

        public Unit GetUnit() { return inheritedUnit; }

        public void SetUnitAppearance(Color color) => GetComponentInChildren<MeshRenderer>().material.color = color;

        private bool executed;
        public bool Executed() { return executed; }
        public bool SetExecuted(bool exec) => executed = exec;

        private bool alive = true;
        private UnitUI uUI;

        private bool positionAchieved = true;
        private Vector3 previousPosition;
        private Vector3 newPosition;
        private float lerpTimeElapsed;
        private bool additionalMoveExecuted;

        public bool Alive() { return alive; }

        public void Execute()
        {
            uUI.unitTurn.SetActive(true);
            ResetLerp();
            StartCoroutine(DoDelayExecute());
        }

        private void ResetLerp()
        {
            lerpTimeElapsed = 0.0f;
        }

        private void Update()
        {
            if (!positionAchieved)
            {
                if (lerpTimeElapsed < 2.5f)
                {
                    transform.position = Vector3.Lerp(previousPosition, newPosition, lerpTimeElapsed / 2.5f);
                    float distance = Vector3.Distance(transform.position, newPosition);
                    if (distance <= 3.5f) positionAchieved = true;
                    lerpTimeElapsed += Time.deltaTime;
                }
            }
        }

        private IEnumerator DoDelayExecute()
        {
            yield return new WaitForSeconds(1.0f);
            if (currentUnitTurnInterval <= 0)
            {
                currentUnitTurnInterval = inheritedUnit.AttackInterval;
                if (inheritedUnit.Attributes == UnitAttributes.Fast)
                {
                    currentUnitTurnInterval--;
                    if (currentUnitTurnInterval < 0) currentUnitTurnInterval = 0;
                    uUI.SetDynamicText("[Fast Attribute] Attack Interval substracted by 1!");
                    
                    StartCoroutine(DoDelayMove());
                    yield break;
                }
                
                DoMove();
            }
            else
            {
                uUI.SetDynamicText("Next turn in: " + currentUnitTurnInterval);
                if (inheritedUnit.Attributes == UnitAttributes.Mechanical)
                {
                    inheritedUnit.Health++;
                    uUI.SetDynamicText("[Mechanical Attribute] Health increased by 1! Unit is repairing...");
                    uUI.UpdateUI(inheritedUnit.Health, inheritedUnit.Armor);
                }
                
                currentUnitTurnInterval--;
                StartCoroutine(Finish(0.5f, true));
            }
        }

        private IEnumerator DoDelayMove()
        {
            yield return new WaitForSeconds(2.0f);
            DoMove();
        }
        
        private void DoMove()
        {
            UnitBehavior[] units = FindObjectsOfType<UnitBehavior>();
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i].currentUnitKingdomID == currentUnitKingdomID)
                    continue;

                float distance = Vector3.Distance(transform.position, units[i].transform.position);
                if (distance > inheritedUnit.AttackRange) continue;
                StartCoroutine(DoAttack(units[i]));
                return;
            }
            
            uUI.SetDynamicText("No range!");
            var battleArea = CombatManager.Instance.BattleArea;
            var bounds = battleArea.bounds;
            previousPosition = transform.position;
            newPosition = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                0f,
                Random.Range(bounds.min.z, bounds.max.z)
            );
            positionAchieved = false;
            
            StartCoroutine(Finish());
        }

        private IEnumerator DoAttack(UnitBehavior unit)
        {
            yield return new WaitForSeconds(2.0f);
            Debug.Log(inheritedUnit.Name + " is attacking " + unit.inheritedUnit.Name);

            int damage = inheritedUnit.Damage;
            if (inheritedUnit.AdditionalDamages != null)
            {
                if (inheritedUnit.AdditionalDamages.Length > 0)
                {
                    foreach (var additionalDamages in inheritedUnit.AdditionalDamages)
                    {
                        if (additionalDamages.Against.HasFlag(unit.inheritedUnit.Attributes))
                        {
                            uUI.SetDynamicText("[Weak Target] This unit has additional damage against enemy's attribute!");
                            damage += additionalDamages.AdditionalDamageValue;
                            break;
                        }
                    }
                }
            }

            unit.Damage(damage);
            StartCoroutine(Finish());
        }

        private void Damage(int damage)
        {
            string additionalText = "";
            bool ignoreHealth = false;
            if (damage <= 0) damage = 1;
            
            if (inheritedUnit.Armor > 0)
            {
                if (inheritedUnit.Attributes == UnitAttributes.Armored)
                    damage /= 2;
                
                if (damage < 1) damage = 1;
                
                inheritedUnit.Armor -= damage;
                ignoreHealth = true;
                additionalText += "\n[Armored Attribute] Damage reduced by half!";
            }
            
            if (inheritedUnit.Armor < 0)
            {
                inheritedUnit.Health -= -inheritedUnit.Armor;
                inheritedUnit.Armor = 0;
                ignoreHealth = true;
            }

            if (!ignoreHealth) inheritedUnit.Health -= damage;
            uUI.UpdateUI(inheritedUnit.Health, inheritedUnit.Armor);
            uUI.SetDynamicText("-" + damage + additionalText, 1.5f);
            
            if (inheritedUnit.Health <= 0)
            {
                Kill();
            }
        }

        private void Kill()
        {
            CombatManager.Instance.RemoveUnitFromTurnOrder(this);
            CombatManager.Instance.ReconstructIterations();
            
            Destroy(uUI.unitTurn);
            Destroy(uUI.UnitDynamicText.gameObject);

            alive = false;
            SetExecuted(true);
            DestroyUnit();
        }

        public void DestroyUnit()
        {
            Destroy(gameObject);
        }
        
        private IEnumerator<WaitForSeconds> Finish(float OverrideTime = -1, bool isResting = false)
        {
            if (inheritedUnit.Attributes == UnitAttributes.Light && !additionalMoveExecuted && !isResting)
            {
                uUI.SetDynamicText("[Light Attribute] This unit can perform additional move during this turn!");
                currentUnitTurnInterval = 0;
                additionalMoveExecuted = true;
                StartCoroutine(DoDelayMove());
                yield break;
            }
            
            yield return new WaitForSeconds(Math.Abs(OverrideTime - (-1)) < 1.0 ? 4.0f : OverrideTime);
            SetExecuted(true);
            additionalMoveExecuted = false;
            uUI.unitTurn.SetActive(false);
        }
    }
}
