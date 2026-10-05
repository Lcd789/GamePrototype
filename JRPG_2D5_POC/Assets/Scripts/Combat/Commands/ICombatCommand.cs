namespace JRPG2D5.Combat.Commands
{
    /// <summary>
    /// Command Pattern: player input, auto-combat, and follow-ups all issue the same action type.
    /// </summary>
    public interface ICombatCommand
    {
        bool CanExecute();
        void Execute();
    }
}
