using GeomaceRL.Command;
using Optional;

namespace GeomaceRL.Actor
{
    public class Player : Actor
    {
        //public EquipmentHandler Equipment { get; }

        public Player(in Loc pos) : base(pos, 10, Colors.Player, '@')
        {
            //Equipment = new EquipmentHandler();
        }

        // Wait for the input system to set NextCommand. Since Commands don't repeat, clear
        // NextCommand once it has been sent.
        public override Option<ICommand> GetAction()
        {
            return Game.StateHandler.HandleInput();
        }

        public override void TriggerDeath() => Game.GameOver();
    }
}
