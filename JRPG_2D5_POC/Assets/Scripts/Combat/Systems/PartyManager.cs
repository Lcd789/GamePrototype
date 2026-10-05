using System.Collections.Generic;
using JRPG2D5.Combat.Runtime;
using JRPG2D5.Core.EventBus;
using JRPG2D5.Data;
using UnityEngine;

namespace JRPG2D5.Combat.Systems
{
    public class PartyManager : MonoBehaviour
    {
        [SerializeField] int frontlineSize = 4;
        [SerializeField] int reserveSize = 2;
        [SerializeField, Range(0f, 1f)] float reserveHpRegenPercent = 0.08f;
        [SerializeField, Range(0f, 1f)] float reserveMpRegenPercent = 0.08f;

        readonly List<Combatant> _frontline = new List<Combatant>();
        readonly List<Combatant> _reserve = new List<Combatant>();

        public IReadOnlyList<Combatant> Frontline => _frontline;
        public IReadOnlyList<Combatant> Reserve => _reserve;

        public void SetupParty(IList<CharacterData> roster)
        {
            _frontline.Clear();
            _reserve.Clear();
            if (roster == null) return;

            int index = 0;
            for (; index < roster.Count && _frontline.Count < frontlineSize; index++)
            {
                var unit = new Combatant(roster[index]);
                unit.IsFrontline = true;
                _frontline.Add(unit);
            }

            for (; index < roster.Count && _reserve.Count < reserveSize; index++)
            {
                var unit = new Combatant(roster[index]);
                unit.IsFrontline = false;
                _reserve.Add(unit);
            }
        }

        public bool TrySwap(int frontIndex, int reserveIndex)
        {
            if (frontIndex < 0 || frontIndex >= _frontline.Count) return false;
            if (reserveIndex < 0 || reserveIndex >= _reserve.Count) return false;

            var outgoing = _frontline[frontIndex];
            var incoming = _reserve[reserveIndex];
            if (outgoing == null || incoming == null || !incoming.IsAlive) return false;

            outgoing.IsFrontline = false;
            incoming.IsFrontline = true;
            _frontline[frontIndex] = incoming;
            _reserve[reserveIndex] = outgoing;

            GameEvents.TriggerCharacterSwapped(frontIndex, reserveIndex);
            GameEvents.TriggerValorChant(incoming);
            return true;
        }

        public void TickReserveRegen()
        {
            for (int i = 0; i < _reserve.Count; i++)
            {
                var unit = _reserve[i];
                if (unit == null || !unit.IsAlive) continue;
                unit.Heal(Mathf.RoundToInt(unit.MaxHP * reserveHpRegenPercent));
                unit.RestoreMp(Mathf.RoundToInt(unit.MaxMP * reserveMpRegenPercent));
            }
        }

        public void ApplyStunToFrontline(bool stunned)
        {
            for (int i = 0; i < _frontline.Count; i++)
            {
                if (_frontline[i] != null)
                    _frontline[i].IsStunned = stunned;
            }
        }

        public bool IsPartyWiped()
        {
            for (int i = 0; i < _frontline.Count; i++)
            {
                if (_frontline[i] != null && _frontline[i].IsAlive) return false;
            }
            for (int i = 0; i < _reserve.Count; i++)
            {
                if (_reserve[i] != null && _reserve[i].IsAlive) return false;
            }
            return true;
        }

        public IEnumerable<Combatant> AllMembers()
        {
            for (int i = 0; i < _frontline.Count; i++) yield return _frontline[i];
            for (int i = 0; i < _reserve.Count; i++) yield return _reserve[i];
        }
    }
}
