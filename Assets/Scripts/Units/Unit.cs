using System;
using AFSInterview.Units;
using UnityEngine;

// Struct to determine additional parameters of the unit
[Serializable]
public struct AdditionalDamage
{
    [SerializeField] private int additionalDamage;
    [SerializeField] private UnitAttributes against;

    public int AdditionalDamageValue => additionalDamage;
    public UnitAttributes Against => against;
}

namespace AFSInterview.Units
{
    [Serializable]
    public class Unit
    {
        // Unit's name
        [SerializeField] private string name;
        
        // Unit's health and armor
        [Header("General")]
        [SerializeField] private int health;
        [SerializeField] private int armor;
        
        // Unit's attack variables
        [Header("Attack")]
        [SerializeField] private int attackInterval;
        [SerializeField] private float attackRange;
        [SerializeField] private int damage;
        [SerializeField] private AdditionalDamage[] additionalDamages;

        // Unit's attributes
        [Header("Attributes")]
        [SerializeField] private UnitAttributes attributes;

        // Unit's model name reference
        [Header("Visuals")]
        [SerializeField] private string resourcesModelName;
        
        // Public get/set for handling parameters
        public string Name { get => name; set => name = value; }
        public int Health { get => health; set => health = value; }
        public int Armor { get => armor; set => armor = value; }
        public int AttackInterval { get => attackInterval; set => attackInterval = value; }
        public float AttackRange { get => attackRange; set => attackRange = value; }
        public int Damage { get => damage; set => damage = value; }
        public AdditionalDamage[] AdditionalDamages { get => additionalDamages; set => additionalDamages = value; }
        public UnitAttributes Attributes { get => attributes; set => attributes = value; }
        public string ResourcesModelName => resourcesModelName;
    }
}
