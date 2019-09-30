using BearLib;
using GeomaceRL.Actor;

namespace GeomaceRL.UI
{
    internal static class ViewPanel
    {
        public static void Draw(LayerInfo layer)
        {
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopLeftChar = '┌',
                TopChar = '─', // 196
                LeftChar = '│' // 179
            });

            const char hpSymbol = '*';
            int drawY = 0;

            foreach (Actor.Actor actor in Game.MapHandler.Units.Values)
            {
                if (Game.MapHandler.Field[actor.Pos].IsVisible && !(actor is Pillar))
                {
                    Terminal.Color(actor.Color);
                    layer.Put(0, drawY, actor.Symbol);

                    Terminal.Color(Swatch.DbBlood);
                    for (int n = 0; n < actor.Health; n++)
                    {
                        layer.Put(2 + n, drawY, hpSymbol);
                    }

                    Terminal.Color(Swatch.DbMetal);
                    for (int n = actor.Health; n < actor.MaxHealth; n++)
                    {
                        layer.Put(2 + n, drawY, hpSymbol);
                    }

                    drawY++;
                }
            }
        }
    }
}
