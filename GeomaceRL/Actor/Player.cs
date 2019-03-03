using GeomaceRL.Command;
using Optional;

namespace GeomaceRL.Actor
{
    public class Player : Actor
    {
        //public EquipmentHandler Equipment { get; }

        public Player(in Loc pos) : base(pos, Constants.PLAYER_HP, Colors.Player, '@')
        {
            //Equipment = new EquipmentHandler();
            Speed = 2;
        }

        // Wait for the input system to set NextCommand. Since Commands don't repeat, clear
        // NextCommand once it has been sent.
        public override Option<ICommand> GetAction()
        {
            return Option.None<ICommand>();
        }

        public override void TriggerDeath() => Game.GameOver();
    }
}
