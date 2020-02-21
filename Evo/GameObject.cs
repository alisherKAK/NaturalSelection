namespace Evo
{
    public abstract class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }

        public abstract void Print();
    }
}
