using JRPG2D5.Core.EventBus;

namespace JRPG2D5.Core.StateMachine
{
    public sealed class ExplorationState : IGameState
    {
        readonly GameStateMachine _machine;
        bool _listening;

        public ExplorationState(GameStateMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            if (_listening) return;
            GameEvents.OnCombatStarted += HandleCombatStarted;
            _listening = true;
        }

        public void Tick(float deltaTime) { }

        public void Exit()
        {
            if (!_listening) return;
            GameEvents.OnCombatStarted -= HandleCombatStarted;
            _listening = false;
        }

        void HandleCombatStarted()
        {
            _machine.ChangeState(new CombatGameState(_machine));
        }
    }

    public sealed class CombatGameState : IGameState
    {
        readonly GameStateMachine _machine;
        bool _listening;

        public CombatGameState(GameStateMachine machine)
        {
            _machine = machine;
        }

        public void Enter()
        {
            if (_listening) return;
            GameEvents.OnCombatEnded += HandleCombatEnded;
            _listening = true;
        }

        public void Tick(float deltaTime) { }

        public void Exit()
        {
            if (!_listening) return;
            GameEvents.OnCombatEnded -= HandleCombatEnded;
            _listening = false;
        }

        void HandleCombatEnded()
        {
            _machine.ChangeState(new ExplorationState(_machine));
        }
    }
}
