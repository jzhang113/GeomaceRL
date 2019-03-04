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
            //layer.Print(-1, $"{Constants.HEADER_LEFT}SCAN" +
            //    $"[color=white]{Constants.HEADER_SEP}DATA{Constants.HEADER_RIGHT}",
            //    System.Drawing.ContentAlignment.TopRight);

            // draw info
            Terminal.Color(Colors.Text);

            int drawX = 0, drawY = 0;
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

                if (drawY >= 3)
                {
                    drawX++;
                    drawY = 0;
                }
            }

            drawY = 5;
            foreach ((Element elem, int amount) in Game.Player.Mana)
            {
                Terminal.Color(elem.Color());
                layer.PrintMana(0, drawY++, $"{amount}");
            }
        }
    }
}
