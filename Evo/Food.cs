using System;

namespace Evo
{
    public class Food : GameObject
    {
        public override void Print()
        {
            Console.SetCursorPosition(X + XPos, Y + YPos);
            Console.Write("@");
        }
    }
}
