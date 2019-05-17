﻿using BearLib;
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
        public int CurrentSpell { get; }

        private readonly Actor.Actor _source;
        private readonly TargetZone _targetZone;
        private readonly Func<IEnumerable<Loc>, Option<ICommand>> _callback;
        private readonly IEnumerable<Loc> _inRange;
        private readonly IList<Actor.Actor> _targettableActors;

        // drawing
        private readonly IList<Loc> _targetted;
        private readonly IList<Loc> _path;

        private int _index;
        private Loc _cursor;

        public TargettingState(
            Actor.Actor source,
            TargetZone zone,
            int spellnum,
            Func<IEnumerable<Loc>, Option<ICommand>> callback)
        {
            _source = source;
            _targetZone = zone;
            CurrentSpell = spellnum;
            _callback = callback;
            _targettableActors = new List<Actor.Actor>();
            _targetted = new List<Loc>();
            _path = new List<Loc>();
            _inRange = zone.GetAllValidTargets(_source.Pos);

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
                ? _callback(targets)
                : Option.None<ICommand>();
        }

        private void MoveTarget(in Loc direction)
        {
            Loc next = _cursor + direction;
            if (Game.MapHandler.Field.IsValid(next) && _inRange.Contains(next))
                _cursor = next;
        }

        private void JumpTarget(in Loc direction)
        {
            Loc next = _cursor + direction;
            while (Game.MapHandler.Field.IsValid(next) && _inRange.Contains(next))
            {
                _cursor = next;
                next += direction;
            }
        }

        private IEnumerable<Loc> DrawTargettedTiles()
        {
            IEnumerable<Loc> targets = _targetZone.GetTilesInRange(_source.Pos, _cursor);
            _targetted.Clear();
            _path.Clear();

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
