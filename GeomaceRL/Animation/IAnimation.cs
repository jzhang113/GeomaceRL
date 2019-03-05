using GeomaceRL.UI;

namespace GeomaceRL.Animation
{
    public interface IAnimation
    {
        int Turn { get; }

        // Returns true when an animation is done updating
        bool Update();

        // Draw the animation to Layer
        void Draw(LayerInfo layer);

        // Any cleanup that needs to be done, like unhiding Actors
        void Cleanup();
    }
}
