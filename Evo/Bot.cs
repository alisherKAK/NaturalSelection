using System;

namespace Evo
{
    public class Bot : GameObject
    {
        private int[] _commands;
        private int _commandsCount = 64;
        private int _currentCommmand;

        private Directions _direction;

        private int _healthPoint;
        private int _maxHealthPoint;

        public Bot()
        {
            _commands = new int[_commandsCount];
            _healthPoint = 20;
            _maxHealthPoint = 90;
            _currentCommmand = 0;

            _direction = Directions.Up;

            Random random = new Random();
            for(int i = 0; i < _commandsCount; i++)
            {
                _commands[i] = random.Next(0, _commandsCount - 1);    
            }
        }

        // 0..7 сделать шаг
        // 8..15 схватить еду или преобразовать яд в еду
        // 16..23 посмотреть
        // 24..31 поворот
        // 32..63 безусловный переход

        //poison = 1, wall = 2, bot = 3, food = 4, empty = 5

        public void Move()
        {
            switch (_direction)
            {
                case Directions.LeftUp:
                    XPos--;
                    YPos--;
                    break;
                case Directions.Up:
                    YPos--;
                    break;
                case Directions.RightUp:
                    XPos++;
                    YPos--;
                    break;
                case Directions.Right:
                    XPos++;
                    break;
                case Directions.RightDown:
                    XPos++;
                    YPos++;
                    break;
                case Directions.Down:
                    YPos++;
                    break;
                case Directions.LeftDown:
                    XPos--;
                    YPos++;
                    break;
                case Directions.Left:
                    XPos--;
                    break;
            }
        }

        public void Back()
        {
            switch (_direction)
            {
                case Directions.LeftUp:
                    XPos++;
                    YPos++;
                    break;
                case Directions.Up:
                    YPos++;
                    break;
                case Directions.RightUp:
                    XPos--;
                    YPos++;
                    break;
                case Directions.Right:
                    XPos--;
                    break;
                case Directions.RightDown:
                    XPos--;
                    YPos--;
                    break;
                case Directions.Down:
                    YPos--;
                    break;
                case Directions.LeftDown:
                    XPos++;
                    YPos--;
                    break;
                case Directions.Left:
                    XPos++;
                    break;
            }
        }

        public void MoveCurrentCommand(int touchObject)
        {
            _currentCommmand += touchObject;
            _currentCommmand %= _commandsCount;
        }

        public void Rotate()
        {
            _direction = (Directions)(31 - _commands[_currentCommmand]);
        }

        public void UnconditionalTransition()
        {
            _currentCommmand += _commands[_currentCommmand];
            _currentCommmand %= _commandsCount;
        }

        public Bot GetClone()
        {
            var clone = new Bot();
            _commands.CopyTo(clone._commands, 0);

            return clone;
        }

        public void Mutate()
        {
            Random random = new Random();
            int randCommand = random.Next(0, _commandsCount - 1);

            _commands[randCommand] = random.Next(0, _commandsCount - 1);
        }

        public void Eat()
        {
            if(_healthPoint < _maxHealthPoint)
            {
                _healthPoint += 10;
            }
        }

        public void Damage()
        {
            _healthPoint--;
        }

        public bool IsAlive()
        {
            if(_healthPoint > 0)
            {
                return true;
            }

            return false;
        }

        public int GetCurrentCommand()
        {
            return _commands[_currentCommmand];
        }

        public override void Print()
        {
            Console.SetCursorPosition(X + XPos, Y + YPos);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("B");
        }
    }
}
