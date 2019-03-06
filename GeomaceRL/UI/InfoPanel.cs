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

            int drawY = 0;
            foreach (Actor.Actor actor in Game.MapHandler.Units.Values)
            {
                if (Game.MapHandler.Field[actor.Pos].IsVisible)
                {
                    Terminal.Layer(layer.Z + 3);
                    Terminal.Color(Colors.Text);
                    layer.Print(drawY, $"{actor.Symbol}:{actor.Health}/{actor.MaxHealth}");

                    double hpFrac = (double)actor.Health / actor.MaxHealth;
                    double width = hpFrac * (layer.Width - 3);
                    int intWidth = (int)width;
                    double fracWidth = width - intWidth;

                    Terminal.Color(Swatch.DbOldBlood);
                    Terminal.Layer(layer.Z);
                    for (int i = 2; i < layer.Width - 1; i++)
                    {
                        layer.Put(i, drawY, '█');
                    }

                    Terminal.Color(Swatch.DbBlood);
                    Terminal.Layer(layer.Z + 1);
                    Terminal.PutExt(
                        layer.X + intWidth + 1,
                        layer.Y + drawY,
                        (int)(fracWidth * Terminal.State(Terminal.TK_CELL_WIDTH)),
                        0, '█');

                    Terminal.Layer(layer.Z + 2);
                    for (int i = 0; i < intWidth; i++)
                    {
                        layer.Put(i + 2, drawY, '█');
                    }

                    Terminal.Color("black");
                    layer.Put(1, drawY, '█');
                    drawY++;
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
