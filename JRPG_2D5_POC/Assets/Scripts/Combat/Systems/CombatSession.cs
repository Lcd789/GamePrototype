using System.Collections.Generic;
using JRPG2D5.Combat.AI;
using JRPG2D5.Combat.Commands;
using JRPG2D5.Combat.Rules;
using JRPG2D5.Combat.Runtime;
using JRPG2D5.Combat.States;
using JRPG2D5.Core.EventBus;
using JRPG2D5.Data;
using UnityEngine;

namespace JRPG2D5.Combat.Systems
{
    /// <summary>
    /// Combat hub: party, enemies, AF, stance, command execution, auto-combat.
    /// Logic stays here; presentation should only listen to GameEvents.
    /// </summary>
    public class CombatSession : MonoBehaviour
    {
        [SerializeField] PartyManager party;
        [SerializeField] AnotherForceManager anotherForce;
        [SerializeField] StanceManager stance;
        [SerializeField] List<CharacterData> demoRoster = new List<CharacterData>();
        [SerializeField] List<EnemyData> demoEnemies = new List<EnemyData>();
        [SerializeField] AutoCombatMode autoMode = AutoCombatMode.Off;

        readonly List<Combatant> _enemies = new List<Combatant>();
        CombatStateMachine _flow;
        IAutoCombatStrategy _autoStrategy;
        FollowUpSystem _followUps;

        public PartyManager Party => party;
        public AnotherForceManager AnotherForce => anotherForce;
        public StanceManager Stance => stance;
        public IReadOnlyList<Combatant> Enemies => _enemies;
        public AutoCombatMode AutoMode => autoMode;
        public CombatStateMachine Flow => _flow;

        void Awake()
        {
            if (party == null) party = GetComponent<PartyManager>();
            if (anotherForce == null) anotherForce = GetComponent<AnotherForceManager>();
            if (stance == null) stance = GetComponent<StanceManager>();
            _flow = new CombatStateMachine(this);
            _followUps = new FollowUpSystem(this);
        }

        void OnEnable()
        {
            _followUps?.Bind();
            GameEvents.OnAFInterrupted += HandleAfInterrupted;
        }

        void OnDisable()
        {
            _followUps?.Unbind();
            GameEvents.OnAFInterrupted -= HandleAfInterrupted;
        }

        void HandleAfInterrupted()
        {
            party?.ApplyStunToFrontline(true);
        }

        public void BeginBattle()
        {
            party.SetupParty(demoRoster);
            _enemies.Clear();

            Combatant boss = null;
            for (int i = 0; i < demoEnemies.Count; i++)
            {
                var enemy = new Combatant(demoEnemies[i]);
                _enemies.Add(enemy);
                if (demoEnemies[i] != null && demoEnemies[i].isBoss)
                    boss = enemy;
            }

            IAnotherForceRule rule = null;
            if (boss != null && boss.EnemyData != null)
                rule = AnotherForceRuleFactory.Create(boss.EnemyData.anotherForceRules, boss);

            anotherForce.ResetForNewBattle();
            anotherForce.RegisterBossRule(rule);
            party.ApplyStunToFrontline(false);

            SetAutoMode(autoMode);
            GameEvents.TriggerCombatStarted();
            _flow.ChangeState(new NormalTurnState(this));
        }

        public void SetAutoMode(AutoCombatMode mode)
        {
            autoMode = mode;
            _autoStrategy = AutoCombatStrategyFactory.Create(mode);
        }

        public bool ExecuteCommand(ICombatCommand command)
        {
            if (command == null || !command.CanExecute()) return false;
            command.Execute();
            EvaluateEndConditions();
            return true;
        }

        public void EndPlayerTurn()
        {
            party.TickReserveRegen();
            EvaluateEndConditions();
        }

        public Combatant FindLivingEnemy()
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i] != null && _enemies[i].IsAlive) return _enemies[i];
            }
            return null;
        }

        public Combatant FindWeaknessTarget(JRPG2D5.Enums.Element element)
        {
            Combatant fallback = null;
            for (int i = 0; i < _enemies.Count; i++)
            {
                var enemy = _enemies[i];
                if (enemy == null || !enemy.IsAlive) continue;
                fallback ??= enemy;
                if (enemy.EnemyData != null && enemy.EnemyData.weaknesses.Contains(element))
                    return enemy;
            }
            return fallback;
        }

        public bool AreEnemiesDefeated()
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i] != null && _enemies[i].IsAlive) return false;
            }
            return _enemies.Count > 0;
        }

        public void Tick(float deltaTime)
        {
            _flow?.Tick(deltaTime);
            anotherForce?.Tick(deltaTime);
            if (anotherForce != null && anotherForce.IsActive)
            {
                foreach (var member in party.AllMembers())
                    member?.TickAfCooldown(deltaTime);
            }
        }

        public void RunAutoCombatTick()
        {
            _autoStrategy?.Tick(this);
        }

        void EvaluateEndConditions()
        {
            if (AreEnemiesDefeated())
            {
                anotherForce.ForceEnd();
                _flow.ChangeState(new VictoryState(this));
            }
            else if (party.IsPartyWiped())
            {
                anotherForce.ForceEnd();
                GameEvents.TriggerCombatEnded();
            }
        }

        void Update()
        {
            Tick(Time.deltaTime);
        }
    }
}
