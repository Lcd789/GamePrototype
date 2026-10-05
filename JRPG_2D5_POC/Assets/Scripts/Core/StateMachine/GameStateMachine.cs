using UnityEngine;

namespace JRPG2D5.Core.StateMachine
{
    public class GameStateMachine : MonoBehaviour
    {
        IGameState _current;

        public IGameState Current => _current;

        void Start()
        {
            ChangeState(new ExplorationState(this));
        }

        public void ChangeState(IGameState next)
        {
            _current?.Exit();
            _current = next;
            _current?.Enter();
        }

        void Update()
        {
            _current?.Tick(Time.deltaTime);
        }
    }
}
