using JRPG2D5.Combat.Systems;

namespace JRPG2D5.Combat.Commands
{
    public class SwapCommand : ICombatCommand
    {
        readonly PartyManager _party;
        readonly int _frontIndex;
        readonly int _reserveIndex;

        public SwapCommand(PartyManager party, int frontIndex, int reserveIndex)
        {
            _party = party;
            _frontIndex = frontIndex;
            _reserveIndex = reserveIndex;
        }

        public bool CanExecute()
        {
            return _party != null
                   && _frontIndex >= 0
                   && _reserveIndex >= 0
                   && _frontIndex < _party.Frontline.Count
                   && _reserveIndex < _party.Reserve.Count;
        }

        public void Execute()
        {
            if (!CanExecute()) return;
            _party.TrySwap(_frontIndex, _reserveIndex);
        }
    }
}
