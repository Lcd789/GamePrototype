using JRPG2D5.Combat.AI;
using JRPG2D5.Combat.Systems;
using JRPG2D5.Core.EventBus;

namespace JRPG2D5.Combat.States
{
    public interface ICombatState
    {
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }

    public class CombatStateMachine
    {
        ICombatState _current;

        public CombatStateMachine(CombatSession session)
        {
        }

        public ICombatState Current => _current;

        public void ChangeState(ICombatState next)
        {
            _current?.Exit();
            _current = next;
            _current?.Enter();
        }

        public void Tick(float deltaTime)
        {
            _current?.Tick(deltaTime);
        }
    }

    public sealed class NormalTurnState : ICombatState
    {
        readonly CombatSession _session;

        public NormalTurnState(CombatSession session)
        {
            _session = session;
        }

        public void Enter() { }

        public void Tick(float deltaTime)
        {
            if (_session.AutoMode != JRPG2D5.Combat.AI.AutoCombatMode.Off)
                _session.RunAutoCombatTick();
        }

        public void Exit() { }
    }

    public sealed class AnotherForceState : ICombatState
    {
        readonly CombatSession _session;

        public AnotherForceState(CombatSession session)
        {
            _session = session;
        }

        public void Enter()
        {
            _session.AnotherForce?.TryStart();
        }

        public void Tick(float deltaTime)
        {
            if (_session.AnotherForce == null || !_session.AnotherForce.IsActive)
            {
                _session.Flow.ChangeState(new NormalTurnState(_session));
                return;
            }

            if (_session.AutoMode != JRPG2D5.Combat.AI.AutoCombatMode.Off)
                _session.RunAutoCombatTick();
        }

        public void Exit() { }
    }

    public sealed class AutoBattleState : ICombatState
    {
        readonly CombatSession _session;
        readonly AutoCombatMode _mode;

        public AutoBattleState(CombatSession session, AutoCombatMode mode)
        {
            _session = session;
            _mode = mode;
        }

        public void Enter()
        {
            _session.SetAutoMode(_mode);
        }

        public void Tick(float deltaTime)
        {
            _session.RunAutoCombatTick();
        }

        public void Exit() { }
    }

    public sealed class VictoryState : ICombatState
    {
        readonly CombatSession _session;

        public VictoryState(CombatSession session)
        {
            _session = session;
        }

        public void Enter()
        {
            GameEvents.TriggerCombatEnded();
        }

        public void Tick(float deltaTime) { }

        public void Exit() { }
    }
}
