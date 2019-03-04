using BearLib;
using System.Collections.Generic;

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
                TopLeftChar = '╔', // 201
                BottomLeftChar = '╚', // 200
                TopChar = '═', // 205
                BottomChar = '═',
                LeftChar = '║' // 186
            });
            //layer.Print(-1, $"{Constants.HEADER_LEFT}SCAN" +
            //    $"[color=white]{Constants.HEADER_SEP}DATA{Constants.HEADER_RIGHT}",
            //    System.Drawing.ContentAlignment.TopRight);

            // draw info
            Terminal.Color(Colors.Text);
            int drawX = 0, drawY = 0;
            var manaAmount = new Dictionary<Element, int>()
            {
                [Element.Fire] = 0,
                [Element.Earth] = 0,
                [Element.Metal] = 0,
                [Element.Water] = 0,
                [Element.Wood] = 0
            };
            
            foreach ((int x, int y) in Game.MapHandler.GetPointsInRadius(Game.Player.Pos, 1))
            {
                (Element elem, int amount) = Game.MapHandler.Mana[x, y];

                if (!Game.MapHandler.Field[x, y].IsWall)
                {
                    manaAmount[elem] += amount;

                    Terminal.Color(elem.Color());
                    layer.PrintMana(drawX, drawY++, $"{amount}");
                }
                else
                {
                    layer.PrintMana(drawX, drawY++, $" ");
                }

                if (drawY >= 3)
                {
                    drawX++;
                    drawY = 0;
                }
            }

            drawY += 5;

            foreach ((Element elem, int amount) in manaAmount)
            {
                Terminal.Color(elem.Color());
                layer.PrintMana(0, drawY++, $"{amount}");
            }
        }
    }
}
