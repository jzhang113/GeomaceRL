using GeomaceRL.Animation;
using GeomaceRL.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL
{
    public class AnimationHandler
    {
        private IDictionary<int, List<IAnimation>> _current { get; }

        public AnimationHandler()
        {
            _current = new Dictionary<int, List<IAnimation>>();
        }

        public void Clear() => _current.Clear();

        public void Add(int id, IAnimation animation)
        {
            if (_current.TryGetValue(id, out List<IAnimation> queue))
                queue.Add(animation);
            else
                _current.Add(id, new List<IAnimation>() { animation });
        }

        public bool IsDone()
        {
            foreach ((int _, List<IAnimation> queue) in _current)
            {
                if (queue.Count == 0)
                    return true;
            }

            return true;
        }

        public void Run(TimeSpan frameTime, double remaining)
        {
            foreach ((int _, List<IAnimation> queue) in _current)
            {
                IAnimation animation = queue.FirstOrDefault();

                if (animation == null)
                    continue;

                if (animation.Update(frameTime) || EventScheduler.Turn > animation.Turn + 1)
                {
                    animation.Cleanup();
                    queue.Remove(animation);

                    if (animation is MoveAnimation currMove
                        && Game.MapHandler.Field[currMove._source.Pos].IsVisible)
                    {
                        currMove._source.ShouldDraw = true;

                        foreach (IAnimation other in queue)
                        {
                            if (other != animation
                                && other is MoveAnimation nextMove
                                && nextMove._source == currMove._source)
                            {
                                nextMove._source.ShouldDraw = false;
                                nextMove._multmove = false;
                            }
                        }
                    }
                }
            }
        }

        public void Draw(LayerInfo _mapLayer)
        {
            foreach ((int _, List<IAnimation> queue) in _current)
            {
                IAnimation animation = queue.FirstOrDefault();

                if (animation != null)
                    animation.Draw(_mapLayer);
            }
        }
    }
}