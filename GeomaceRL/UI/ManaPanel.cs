using BearLib;

namespace GeomaceRL.UI
{
    internal static class ManaPanel
    {
        public static void Draw(LayerInfo layer)
        {
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopLeftChar = '├',
                BottomLeftChar = '└',
                TopChar = '─', // 196
                BottomChar = '─',
                LeftChar = '│' // 179
            });

            layer.Print(-1, $"{Constants.HEADER_LEFT}[color=white]Mana[/color]{Constants.HEADER_RIGHT}",
                System.Drawing.ContentAlignment.TopCenter);

            const int top = 1;
            double startX = (layer.Width - 3.0) / 2.0;

            int drawX = (int)startX;
            int offset = (int)((startX - drawX) * Terminal.State(Terminal.TK_CELL_WIDTH));
            int drawY = top;
            Terminal.Font("text");

            foreach ((int x, int y) in Game.MapHandler.GetPointsInRadius(Game.Player.Pos, 1))
            {
                (Element elem, int amount) = Game.MapHandler.Mana[x, y];

                if (!Game.MapHandler.Field[x, y].IsWall)
                {
                    Terminal.Color(elem.Color());
                    Terminal.PutExt(layer.X + drawX, layer.Y + drawY, offset, 0, amount + '0');
                }
                else
                {
                    Terminal.Color(Colors.Wall);
                    Terminal.PutExt(layer.X + drawX, layer.Y + drawY, offset, 0, '#');
                }

                if (++drawY >= top + 3)
                {
                    drawX++;
                    drawY = top;
                }
            }

            Terminal.Font("");
            drawY += 4;
            drawX = 0;
            int i = 0;
            foreach ((Element elem, int amount) in Game.Player.Mana)
            {
                Terminal.Color(elem.Color());
                layer.PrintMana(drawX, drawY, $"{amount}{elem.Abbrev()}");

                drawX += 6;
                if (++i == 2)
                {
                    drawY++;
                    drawX = 0;
                }
            }
        }
    }
}
