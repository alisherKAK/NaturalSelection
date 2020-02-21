using System;

namespace Evo
{
    public class Wall : GameObject
    {
        public override void Print()
        {
            Console.SetCursorPosition(X + XPos, Y + YPos);
            Console.Write("#");
        }
    }
}
