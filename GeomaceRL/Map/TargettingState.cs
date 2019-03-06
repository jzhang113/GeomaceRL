using BearLib;
using GeomaceRL.Command;
using GeomaceRL.Input;
using GeomaceRL.UI;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL.State
{
    internal class TargettingState : IState
    {
        private readonly Actor.Actor _source;
        private readonly TargetZone _targetZone;
        private readonly Func<IEnumerable<Loc>, ICommand> _callback;
        private readonly IEnumerable<Loc> _inRange;
        private readonly IList<Actor.Actor> _targettableActors;

        // drawing
        private readonly IList<Loc> _targetted;
        private readonly IList<Loc> _path;

        private int _index;
        private Loc _cursor;

        public TargettingState(Actor.Actor source, TargetZone zone, Func<IEnumerable<Loc>, ICommand> callback)
        {
            _source = source;
            _targetZone = zone;
            _callback = callback;
            _targettableActors = new List<Actor.Actor>();
            _targetted = new List<Loc>();
            _path = new List<Loc>();

            ICollection<Loc> tempRange = new HashSet<Loc>();
            _inRange = Game.MapHandler.GetPointsInRadius(_source.Pos, zone.Range).ToList();

            // Filter the targettable range down to only the tiles we have a direct line on.
            foreach (Loc point in _inRange)
            {
                Loc collision = point;
                foreach (Loc current in Game.MapHandler.GetStraightLinePath(_source.Pos, point))
                {
                    if (Game.MapHandler.Field[current].IsWall)
                    {
                        collision = current;
                        break;
                    }
                }

                tempRange.Add(collision);
            }
            _inRange = tempRange;

            // Pick out the interesting targets.
            // TODO: select items for item targetting spells
            foreach (Loc point in _inRange)
            {
                Game.MapHandler.GetActor(point)
                    .MatchSome(actor =>
                    _targettableActors.Add(actor));
            }

            // Initialize the targetting to an interesting target.
            var firstActor = Option.None<Actor.Actor>();
            foreach (Actor.Actor target in _targettableActors)
            {
                if (!(target is Actor.Player) && Game.MapHandler.Field[target.Pos].IsVisible)
                {
                    firstActor = Option.Some(target);
                    break;
                }
            }

            firstActor.Match(
                some: first => _cursor = first.Pos,
                none: () => _cursor = source.Pos);

            DrawTargettedTiles();
        }

        // ReSharper disable once CyclomaticComplexity
        public Option<ICommand> HandleKeyInput(int key)
        {
            switch (InputMapping.GetTargettingInput(key))
            {
                case TargettingInput.None:
                    return Option.None<ICommand>();
                case TargettingInput.JumpW:
                    JumpTarget(Direction.W);
                    break;
                case TargettingInput.JumpS:
                    JumpTarget(Direction.S);
                    break;
                case TargettingInput.JumpN:
                    JumpTarget(Direction.N);
                    break;
                case TargettingInput.JumpE:
                    JumpTarget(Direction.E);
                    break;
                case TargettingInput.JumpNW:
                    JumpTarget(Direction.NW);
                    break;
                case TargettingInput.JumpNE:
                    JumpTarget(Direction.NE);
                    break;
                case TargettingInput.JumpSW:
                    JumpTarget(Direction.SW);
                    break;
                case TargettingInput.JumpSE:
                    JumpTarget(Direction.SE);
                    break;
                case TargettingInput.MoveW:
                    MoveTarget(Direction.W);
                    break;
                case TargettingInput.MoveS:
                    MoveTarget(Direction.S);
                    break;
                case TargettingInput.MoveN:
                    MoveTarget(Direction.N);
                    break;
                case TargettingInput.MoveE:
                    MoveTarget(Direction.E);
                    break;
                case TargettingInput.MoveNW:
                    MoveTarget(Direction.N);
                    MoveTarget(Direction.W);
                    break;
                case TargettingInput.MoveNE:
                    MoveTarget(Direction.N);
                    MoveTarget(Direction.E);
                    break;
                case TargettingInput.MoveSW:
                    MoveTarget(Direction.S);
                    MoveTarget(Direction.W);
                    break;
                case TargettingInput.MoveSE:
                    MoveTarget(Direction.S);
                    MoveTarget(Direction.E);
                    break;
                case TargettingInput.NextActor:
                    if (_targettableActors.Count > 0)
                    {
                        Actor.Actor nextActor;
                        do
                        {
                            nextActor = _targettableActors[++_index % _targettableActors.Count];
                        } while (!Game.MapHandler.Field[nextActor.Pos].IsVisible);
                        _cursor = nextActor.Pos;
                    }
                    else
                    {
                        _cursor = _source.Pos;
                    }
                    break;
            }

            IEnumerable<Loc> targets = DrawTargettedTiles();
            return (InputMapping.GetTargettingInput(key) == TargettingInput.Fire)
                ? Option.Some(_callback(targets))
                : Option.None<ICommand>();
        }

        private void MoveTarget(in Loc direction)
        {
            (int dx, int dy) = direction;
            Loc next = new Loc(_cursor.X + dx, _cursor.Y + dy);

            if (Game.MapHandler.Field.IsValid(next) && _inRange.Contains(next))
            {
                _cursor = next;
            }
        }

        private void JumpTarget(in Loc direction)
        {
            (int dx, int dy) = direction;
            Loc next = new Loc(_cursor.X + dx, _cursor.Y + dy);

            while (Game.MapHandler.Field.IsValid(next) && _inRange.Contains(next))
            {
                _cursor = next;
                next = new Loc(next.X + dx, next.Y + dx);
            }
        }

        private IEnumerable<Loc> DrawTargettedTiles()
        {
            IEnumerable<Loc> targets = _targetZone.GetTilesInRange(_source, _cursor);
            _targetted.Clear();

            // Draw the projectile path if any.
            foreach (Loc point in _targetZone.Trail)
            {
                _path.Add(point);
            }

            // Draw the targetted tiles.
            foreach (Loc point in targets)
            {
                _targetted.Add(point);
            }

            return targets;
        }

        public void Draw(LayerInfo layer)
        {
            Game.MapHandler.Draw(layer);

            Terminal.Layer(layer.Z - 1);
            foreach (Loc point in _inRange)
            {
                Terminal.Color(Colors.TargetBackground);
                layer.Put(point.X - Camera.X, point.Y - Camera.Y, '█');
            }

            foreach(Loc point in _targetted)
            {
                Terminal.Color(Colors.Target);
                layer.Put(point.X - Camera.X, point.Y - Camera.Y, '█');
            }

            foreach(Loc point in _path)
            {
                Terminal.Color(Colors.Path);
                layer.Put(point.X - Camera.X, point.Y - Camera.Y, '█');
            }

            Terminal.Color(Colors.Cursor);
            layer.Put(_cursor.X - Camera.X, _cursor.Y - Camera.Y, '█');
            Terminal.Layer(layer.Z);
        }
    }
}
