using System.Collections.Generic;
using UnityEngine;

namespace AFSInterview.Units
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance { get; } = new();
        [Header("Red Kingdom")]
        [SerializeField] private List<Unit> redKingdomArmy;
        
        [Header("Blue Kingdom")]
        [SerializeField] private List<Unit> blueKingdomArmy;

        public List<Unit> RedKingdomArmy => redKingdomArmy;
        public List<Unit> BlueKingdomArmy => blueKingdomArmy;
    }
}
