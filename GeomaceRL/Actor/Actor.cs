using BearLib;
using GeomaceRL.Command;
using GeomaceRL.Interface;
using GeomaceRL.UI;
using Optional;
using System.Collections.Generic;
using System.Drawing;

namespace GeomaceRL.Actor
{
    public abstract class Actor : ISchedulable, IDrawable
    {
        public string Name { get; protected set; } = "Monster";
        public Color Color { get; }
        public char Symbol { get; }
        public bool ShouldDraw { get; set; }

        public Loc Pos { get; set; }
        public bool BlocksLight { get; protected set; } = false;

        public int MaxHealth { get; }
        public int Health { get; set; }

        public int Speed { get; protected set; } = 1;

        public bool IsDead => Health <= 0;

        protected Actor(in Loc pos, int hp, in Color color, char symbol)
        {
            Pos = pos;
            MaxHealth = hp;
            Health = hp;

            Color = color;
            Symbol = symbol;
            ShouldDraw = true;
        }

        public virtual void TriggerDeath()
        {
            Game.MapHandler.RemoveActor(this);

            if (Game.MapHandler.Field[Pos].IsVisible)
            {
                Game.MessagePanel.AddMessage($"{Name} dies");
                Game.MapHandler.Refresh();
            }
        }

        internal void TakeDamage(int power)
        {
            Health -= power;
            if (Health < 0)
                Health = 0;
        }

        public Option<ICommand> Act()
        {
            if (IsDead)
            {
                TriggerDeath();
                return Option.None<ICommand>();
            }

            return GetAction();
        }

        public virtual Option<ICommand> GetAction() =>
            Option.Some<ICommand>(new WaitCommand(this));

        public ICommand GetBasicAttack(in Loc target) => new AttackCommand(this, Constants.GEN_ATTACK, target);

        public ICommand GetBasicAttack(IEnumerable<Loc> targets) => new AttackCommand(this, Constants.GEN_ATTACK, targets);

        public void Draw(LayerInfo layer)
        {
            if (!ShouldDraw)
                return;

            if (IsDead)
            {
                Terminal.Color(Swatch.DbOldBlood);
                layer.Put(Pos.X - Camera.X, Pos.Y - Camera.Y, '%');
            }
            else
            {
                Terminal.Color(Color);
                layer.Put(Pos.X - Camera.X, Pos.Y - Camera.Y, Symbol);
            }
        }
    }
}
