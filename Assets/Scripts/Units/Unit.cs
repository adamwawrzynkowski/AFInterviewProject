using System;
using AFSInterview.Units;
using UnityEngine;

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
        [SerializeField] private string name;
        
        [Header("General")]
        [SerializeField] private int health;
        [SerializeField] private int armor;
        
        [Header("Attack")]
        [SerializeField] private int attackInterval;
        [SerializeField] private float attackRange;
        [SerializeField] private int damage;
        [SerializeField] private AdditionalDamage[] additionalDamages;

        [Header("Attributes")]
        [SerializeField] private UnitAttributes attributes;

        [Header("Visuals")]
        [SerializeField] private string resourcesModelName;
        
        public string Name { get => name; set { name = value; } }
        public int Health { get => health; set { health = value; } }
        public int Armor { get => armor; set { armor = value; } }
        public int AttackInterval { get => attackInterval; set { attackInterval = value; } }
        public float AttackRange { get => attackRange; set { attackRange = value; } }
        public int Damage { get => damage; set { damage = value; } }
        public AdditionalDamage[] AdditionalDamages { get => additionalDamages; set { additionalDamages = value; } }
        public UnitAttributes Attributes { get => attributes; set { attributes = value; } }
        public string ResourcesModelName => resourcesModelName;
    }
}
