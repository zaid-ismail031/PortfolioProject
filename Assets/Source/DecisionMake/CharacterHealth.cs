using UnityEngine;

namespace Source.DecisionMake
{
    [CreateAssetMenu(fileName = "ControllerHealth", menuName = "ScriptableObjects/Character")]
    public class CharacterHealth : ScriptableObject
    {
        private float currentHealth;
        private float maxHealth;
        
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
    }
}