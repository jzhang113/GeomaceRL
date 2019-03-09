namespace GeomaceRL.Actor
{
    internal class Pillar : Actor
    {
        public Pillar(in Loc pos) : base(pos, 1, Colors.Wall, 'o')
        {
            Name = "Pillar";
            BlocksLight = true;
        }
    }
}
