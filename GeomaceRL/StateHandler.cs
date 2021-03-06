﻿using BearLib;
using GeomaceRL.Command;
using GeomaceRL.State;
using GeomaceRL.UI;
using Optional;
using System;
using System.Collections.Generic;

namespace GeomaceRL
{
    public class StateHandler
    {
        public LayerInfo CurrentLayer => _consoles[_states.Peek().GetType()];

        private readonly Stack<IState> _states;
        private readonly IDictionary<Type, LayerInfo> _consoles;

        public StateHandler(IDictionary<Type, LayerInfo> consoleMapping)
        {
            _states = new Stack<IState>();
            _consoles = consoleMapping;
            _states.Push(MenuState.Instance);
        }

        public void Reset()
        {
            _states.Clear();
            _states.Push(MenuState.Instance);
        }

        public Option<ICommand> HandleInput()
        {
            if (!Terminal.HasInput())
                return Option.None<ICommand>();

            IState currentState = _states.Peek();

            int key = Terminal.Read();
            if (key == Terminal.TK_CLOSE)
            {
                Game.Exit();
                return Option.None<ICommand>();
            }

            if (key == Terminal.TK_ESCAPE)
            {
                PopState();
                if (_states.Count == 0)
                    Game.Exit();

                return Option.None<ICommand>();
            }

            if (key == Terminal.TK_MOUSE_MOVE)
            {
                return currentState.HandleMouseMove(Terminal.State(Terminal.TK_MOUSE_X), Terminal.State(Terminal.TK_MOUSE_Y));
            }
            else
            {
                return currentState.HandleKeyInput(key);
            }
        }

        public void PopState() => _states.Pop();

        public Option<IState> Peek() =>
            (_states.Count == 0) ? Option.None<IState>() : Option.Some(_states.Peek());

        public void PushState(IState state) => _states.Push(state);

        public void Draw()
        {
            if (_states.Count == 0)
                return;

            IState current = _states.Peek();
            LayerInfo info = _consoles[current.GetType()];
            Terminal.Layer(info.Z);
            current.Draw(info);
        }
    }
}
