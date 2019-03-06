using BearLib;

namespace GeomaceRL.UI
{
    internal static class InfoPanel
    {
        public static void Draw(LayerInfo layer)
        {
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopLeftChar = '┌',
                BottomLeftChar = '└',
                TopChar = '─', // 196
                BottomChar = '─',
                LeftChar = '│' // 179
            });

            Terminal.Color(Colors.Text);
            int drawY = 0;
            foreach (Actor.Actor actor in Game.MapHandler.Units.Values)
            {
                if (Game.MapHandler.Field[actor.Pos].IsVisible)
                {
                    layer.Print(drawY++, $"{actor.Symbol}:{actor.Health}/{actor.MaxHealth}");
                }
            }

            int drawX = 1;
            int sectionTop = layer.Height - 9;
            drawY = sectionTop;
            layer.Print(sectionTop - 1, "Mana");

            foreach ((int x, int y) in Game.MapHandler.GetPointsInRadius(Game.Player.Pos, 1))
            {
                (Element elem, int amount) = Game.MapHandler.Mana[x, y];

                if (!Game.MapHandler.Field[x, y].IsWall)
                {
                    Terminal.Color(elem.Color());
                    layer.PrintMana(drawX, drawY++, $"{amount}");
                }
                else
                {
                    layer.PrintMana(drawX, drawY++, " ");
                }

                if (drawY >= sectionTop + 3)
                {
                    drawX++;
                    drawY = sectionTop;
                }
            }

            drawY += 4;
            foreach ((Element elem, int amount) in Game.Player.Mana)
            {
                Terminal.Color(elem.Color());
                layer.PrintMana(0, drawY++, $"{elem.Abbrev()}:{amount}");
            }
        }
    }
}
