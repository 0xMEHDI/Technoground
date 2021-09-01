using System;
using UnityEngine;

namespace SGoap
{
    public class CharacterStatusController : MonoBehaviour, IDamagable
    {
        public Action<int, int, IAttacker> OnDamageTaken;
        public System.Action OnDeath;

        public int MaxHp = 10;

        public int Hp { get; set; }

        private void Awake()
        {
            Hp = MaxHp;
        }

        public void TakeDamage(int damage = 1, IAttacker attacker = null)
        {
            var previousHp = Hp;
            Hp = Mathf.Clamp(Hp - damage, 0, Hp - damage);
            OnDamageTaken?.Invoke(Hp, MaxHp, attacker);

            if (Hp <= 0 && previousHp != 0)
                OnDeath?.Invoke();
        }
    }
}
