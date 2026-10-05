namespace JRPG2D5.Core.StateMachine
{
    public interface IGameState
    {
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}
