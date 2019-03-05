using GeomaceRL.UI;
using System;

namespace GeomaceRL.Animation
{
    public interface IAnimation
    {
        int Turn { get; }

        TimeSpan Duration { get; }

        TimeSpan StartTime { get; }

        TimeSpan EndTime { get; }

        // Returns true when an animation is done updating
        bool Update();

        // Draw the animation to Layer
        void Draw(LayerInfo layer);

        // Any cleanup that needs to be done, like unhiding Actors
        void Cleanup();
    }
}
