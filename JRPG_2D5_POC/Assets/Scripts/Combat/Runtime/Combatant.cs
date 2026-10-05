using JRPG2D5.Data;

namespace JRPG2D5.Combat.Runtime
{
    /// <summary>
    /// Runtime combat actor. ScriptableObject data stays immutable; this holds HP/MP/cooldowns.
    /// </summary>
    public class Combatant
    {
        public CharacterData CharacterData { get; }
        public EnemyData EnemyData { get; }
        public bool IsPlayer { get; }
        public string DisplayName { get; }
        public int MaxHP { get; }
        public int MaxMP { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int CurrentHP { get; private set; }
        public int CurrentMP { get; private set; }
        public float AfCooldownRemaining { get; private set; }
        public bool IsFrontline { get; set; }
        public bool IsStunned { get; set; }
        public bool IsAlive => CurrentHP > 0;

        public Combatant(CharacterData data)
        {
            CharacterData = data;
            IsPlayer = true;
            DisplayName = data != null ? data.characterName : "Hero";
            MaxHP = data != null ? data.maxHP : 1;
            MaxMP = data != null ? data.maxMP : 0;
            Attack = data != null ? data.attack : 1;
            Defense = data != null ? data.defense : 1;
            CurrentHP = MaxHP;
            CurrentMP = MaxMP;
            IsFrontline = true;
        }

        public Combatant(EnemyData data)
        {
            EnemyData = data;
            IsPlayer = false;
            DisplayName = data != null ? data.enemyName : "Enemy";
            MaxHP = data != null ? data.maxHP : 1;
            MaxMP = 0;
            Attack = data != null ? data.attack : 1;
            Defense = data != null ? data.defense : 1;
            CurrentHP = MaxHP;
            CurrentMP = 0;
            IsFrontline = true;
        }

        public bool TrySpendMp(int cost)
        {
            if (CurrentMP < cost) return false;
            CurrentMP -= cost;
            return true;
        }

        public int ApplyDamage(int amount)
        {
            int applied = System.Math.Max(0, amount);
            CurrentHP = System.Math.Max(0, CurrentHP - applied);
            return applied;
        }

        public int Heal(int amount)
        {
            int before = CurrentHP;
            CurrentHP = System.Math.Min(MaxHP, CurrentHP + System.Math.Max(0, amount));
            return CurrentHP - before;
        }

        public int RestoreMp(int amount)
        {
            int before = CurrentMP;
            CurrentMP = System.Math.Min(MaxMP, CurrentMP + System.Math.Max(0, amount));
            return CurrentMP - before;
        }

        public void SetAfCooldown(float seconds)
        {
            AfCooldownRemaining = System.Math.Max(0f, seconds);
        }

        public void TickAfCooldown(float deltaTime)
        {
            if (AfCooldownRemaining <= 0f) return;
            AfCooldownRemaining = System.Math.Max(0f, AfCooldownRemaining - deltaTime);
        }

        public bool CanUseSkillInAf() => IsAlive && !IsStunned && AfCooldownRemaining <= 0f;
    }
}
