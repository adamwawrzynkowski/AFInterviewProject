using System;
using UnityEngine;

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
        [SerializeField] private int damage;

        [Header("Attributes")]
        [SerializeField] private UnitAttributes attributes;
    }
}
