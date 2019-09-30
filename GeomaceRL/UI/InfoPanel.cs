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
                TopLeftChar = '├',
                BottomLeftChar = '└',
                TopChar = '─', // 196
                BottomChar = '─',
                LeftChar = '│' // 179
            });


        }
    }
}
