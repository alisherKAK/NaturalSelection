using System;
using System.Collections.Generic;

namespace Evo
{
    public class Game
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        private int _generation = 0;

        private int foodCount;
        private int poisonCount;

        private float spawnCof = 0.2f;

        private List<Bot> _bots = new List<Bot>();
        private int _botsCount = 64;

        private List<Food> _foods = new List<Food>();
        private List<Poison> _poisons = new List<Poison>();
        private List<Wall> _walls = new List<Wall>();

        public Game()
        {
            X = 10;
            Y = 5;
            W = 50;
            H = 30;

            Random random = new Random();

            #region WallInit
            for (int i = 0; i < W + 2; i++)
            {
                _walls.Add(new Wall()
                {
                    X = this.X,
                    Y = this.Y,
                    XPos = i,
                    YPos = 0
                });
            }

            for(int i = 0; i < W + 2; i++)
            {
                _walls.Add(new Wall()
                {
                    X = this.X,
                    Y = this.Y,
                    XPos = i,
                    YPos = H + 1
                });
            }

            for(int i = 0; i < H + 1; i++)
            {
                _walls.Add(new Wall()
                {
                    X = this.X,
                    Y = this.Y,
                    XPos = 0,
                    YPos = i
                });
            }

            for (int i = 0; i < H + 1; i++)
            {
                _walls.Add(new Wall()
                {
                    X = this.X,
                    Y = this.Y,
                    XPos = W + 1,
                    YPos = i
                });
            }

            for(int i = 1; i <= 9; i++)
            {
                _walls.Add(new Wall()
                {
                    X = this.X,
                    Y = this.Y,
                    XPos = 10,
                    YPos = i
                });
            }

            for (int i = 1; i <= 9; i++)
            {
                _walls.Add(new Wall()
                {
                    X = this.X,
                    Y = this.Y,
                    XPos = W - 20,
                    YPos = H - i
                });
            }
            #endregion

            #region BotInit
            for (int i = 0; i < _botsCount; i++)
            {
                int x = random.Next(1, W + 1);
                int y = random.Next(1, H + 1);
                while(!IsSpaceFree(x, y))
                {
                    x = random.Next(1, W + 1);
                    y = random.Next(1, H + 1);
                }

                _bots.Add(new Bot()
                {
                    X = this.X,
                    Y = this.Y,
                    XPos = x, 
                    YPos = y
                });
            }
            #endregion

            FoodInit();
            PoisonInit();

            foodCount = _foods.Count;
            poisonCount = _poisons.Count;
        }

        public void Start()
        {
            Food food;
            Poison poison;
            Wall wall;
            Bot bot;
            bool isStopGeneration = false;

            Random random = new Random();
            Print();

            while (true)
            {
                int moveCount = 0;
                for (int i = 0; true; i++)
                {
                    int commandExecuteCount = 0;
                    int unconditionalTransitionCount = 0;
                    while (true)
                    {
                        i %= _bots.Count;

                        PrintEmpty(_bots[i].XPos, _bots[i].YPos);

                        int command = _bots[i].GetCurrentCommand();
                        _bots[i].Move();

                        food = CheckFood(_bots[i].XPos, _bots[i].YPos);
                        poison = CheckPoison(_bots[i].XPos, _bots[i].YPos);
                        wall = CheckWall(_bots[i].XPos, _bots[i].YPos);
                        bot = CheckBot(_bots[i].XPos, _bots[i].YPos, i);

                        if (command >= 0 && command <= 7)
                        {
                            if (food != null)
                            {
                                PrintEmpty(food.XPos, food.YPos);
                                _foods.Remove(food);
                                _bots[i].Eat();

                                _bots[i].MoveCurrentCommand((int)TouchObjects.Food);
                                _bots[i].Damage();

                                CheckBotHealthPoint(_bots[i]);
                            }
                            else if (poison != null)
                            {
                                _bots[i].Back();
                                PrintEmpty(_bots[i].XPos, _bots[i].YPos);

                                PrintEmpty(poison.XPos, poison.YPos);
                                _poisons.Remove(poison);
                                _bots.Remove(_bots[i]);
                            }
                            else if (bot != null)
                            {
                                _bots[i].Back();
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Bot);
                                _bots[i].Damage();
                                CheckBotHealthPoint(_bots[i]);
                            }
                            else if (wall != null)
                            {
                                _bots[i].Back();
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Wall);
                                _bots[i].Damage();
                                CheckBotHealthPoint(_bots[i]);
                            }
                            else
                            {
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Empty);
                                _bots[i].Damage();
                                CheckBotHealthPoint(_bots[i]);
                            }

                            moveCount++;
                            break;
                        }
                        else if (command >= 8 && command <= 15)
                        {
                            if (food != null)
                            {
                                PrintEmpty(food.XPos, food.YPos);
                                _foods.Remove(food);

                                _bots[i].Eat();
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Food);
                            }
                            else if (poison != null)
                            {
                                var changeFood = new Food()
                                {
                                    X = this.X,
                                    Y = this.Y,
                                    XPos = poison.XPos,
                                    YPos = poison.YPos
                                };

                                _foods.Add(changeFood);
                                changeFood.Print();
                                _poisons.Remove(poison);
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Poison);
                            }
                            else if (wall != null)
                            {
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Wall);
                            }
                            else if (bot != null)
                            {
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Bot);
                            }
                            else
                            {
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Empty);
                            }

                            _bots[i].Back();
                            _bots[i].Damage();
                            CheckBotHealthPoint(_bots[i]);

                            break;
                        }
                        else if (command >= 16 && command <= 23)
                        {
                            if (food != null)
                            {
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Food);
                            }
                            else if (poison != null)
                            {
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Poison);
                            }
                            else if (bot != null)
                            {
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Bot);
                            }
                            else if (wall != null)
                            {
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Wall);
                            }
                            else
                            {
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Empty);
                            }

                            _bots[i].Back();
                            _bots[i].Damage();
                            CheckBotHealthPoint(_bots[i]);
                        }
                        else if (command >= 24 && command <= 31)
                        {
                            _bots[i].Back();
                            _bots[i].Rotate();
                            _bots[i].MoveCurrentCommand(random.Next(1, 7));
                            //_bots[i].Damage();
                            //CheckBotHealthPoint(_bots[i]);
                        }
                        else if (command >= 32 && command <= 63)
                        {
                            _bots[i].Back();
                            _bots[i].UnconditionalTransition();

                            unconditionalTransitionCount++;

                            if(unconditionalTransitionCount == 10)
                            {
                                _bots[i].Mutate();
                            }
                        }

                        commandExecuteCount++;

                        if(commandExecuteCount == 10)
                        {
                            break;
                        }

                        if (_bots.Count <= 8)
                        {
                            isStopGeneration = true;
                            break;
                        }
                    }

                    if(_foods.Count <= foodCount*0.9)
                    {
                        for(int j = 0; j < random.Next(1, (int)(foodCount*0.1)); j++)
                        {
                            GenerateFood().Print();
                        }
                    }

                    if(_poisons.Count <= poisonCount*0.8)
                    {
                        for (int j = 0; j < random.Next(1, (int)(poisonCount * 0.1)); j++)
                        {
                            GeneratePoison().Print();
                        }
                    }

                    if (isStopGeneration)
                    {
                        break;
                    }
                }

                int botsCount = _bots.Count;
                for (int i = 0; i < botsCount; i++)
                {
                    for(int j = 0; j < _botsCount / botsCount; j++)
                    {
                        int x = random.Next(1, W + 1);
                        int y = random.Next(1, H + 1);
                        while (!IsSpaceFree(x, y))
                        {
                            x = random.Next(1, W + 1);
                            y = random.Next(1, H + 1);
                        }

                        var newBot = _bots[i].GetClone();
                        newBot.X = this.X;
                        newBot.Y = this.Y;
                        newBot.XPos = x;
                        newBot.YPos = y;

                        if(j == 7)
                        {
                            newBot.Mutate();
                        }

                        _bots.Add(newBot);
                        newBot.Print();
                    }
                    PrintEmpty(_bots[i].XPos, _bots[i].YPos);
                }

                _bots.RemoveRange(0, 8);

                 foreach(var foodClear in _foods)
                {
                    PrintEmpty(foodClear.XPos, foodClear.YPos);
                }

                foreach (var poisonClear in _poisons)
                {
                    PrintEmpty(poisonClear.XPos, poisonClear.YPos);
                }

                _foods.Clear();
                _poisons.Clear();

                FoodInit();
                PoisonInit();

                PrintFood();
                PrintPoison();

                _generation++;
                isStopGeneration = false;

                Console.SetCursorPosition(X + W + 5, Y);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Generation: {_generation}");

                //Console.SetCursorPosition(X + W + 5, Y + 5);
                //Console.WriteLine($"Interrations Count: {moveCount}");
            }
        }

        public void Print()
        {
            foreach(var wall in _walls)
            {
                wall.Print();
            }

            foreach(var bot in _bots)
            {
                bot.Print();
            }

            foreach(var food in _foods)
            {
                food.Print();
            }

            foreach(var poison in _poisons)
            {
                poison.Print();
            }

            Console.SetCursorPosition(X + W + 5, Y);
            Console.WriteLine($"Generation: {_generation}");
        }

        public void PrintEmpty(int x, int y)
        {
            Console.SetCursorPosition(X + x, Y + y);
            Console.Write(" ");
        }

        private Food GenerateFood()
        {
            Random random = new Random();
            int x = random.Next(1, W + 1);
            int y = random.Next(1, H + 1);
            while(!IsSpaceFree(x, y))
            {
                x = random.Next(1, W + 1);
                y = random.Next(1, H + 1);
            }

            var food = new Food()
            {
                X = this.X,
                Y = this.Y,
                XPos = x,
                YPos = y
            };

            _foods.Add(food);

            return food;
        }

        private Poison GeneratePoison()
        {
            Random random = new Random();
            int x = random.Next(1, W + 1);
            int y = random.Next(1, H + 1);
            while (!IsSpaceFree(x, y))
            {
                x = random.Next(1, W + 1);
                y = random.Next(1, H + 1);
            }

            var poison = new Poison()
            {
                X = this.X,
                Y = this.Y,
                XPos = x,
                YPos = y
            };

            _poisons.Add(poison);

            return poison;
        }

        private bool IsSpaceFree(int x, int y)
        {
            foreach(var bot in _bots)
            {
                if(bot.XPos == x && bot.YPos == y)
                {
                    return false;
                }
            }
            
            foreach(var food in _foods)
            {
                if(food.XPos == x && food.YPos == y)
                {
                    return false;
                }
            }

            foreach(var poison in _poisons)
            {
                if(poison.XPos == x && poison.YPos == y)
                {
                    return false;
                }
            }

            foreach(var wall in _walls)
            {
                if(wall.XPos == x && wall.YPos == y)
                {
                    return false;
                }
            }

            return true;
        }

        private Food CheckFood(int x, int y)
        {
            foreach(var food in _foods)
            {
                if(food.XPos == x && food.YPos == y)
                {
                    return food;
                }
            }

            return null;
        }

        private Wall CheckWall(int x, int y)
        {
            foreach (var wall in _walls)
            {
                if (wall.XPos == x && wall.YPos == y)
                {
                    return wall;
                }
            }

            return null;
        }

        private Poison CheckPoison(int x, int y)
        {
            foreach (var poison in _poisons)
            {
                if (poison.XPos == x && poison.YPos == y)
                {
                    return poison;
                }
            }

            return null;
        }

        private Bot CheckBot(int x, int y, int index)
        {
            for(int i = 0; i < _bots.Count; i++)
            {
                if(i != index)
                {
                    if (_bots[i].XPos == x && _bots[i].YPos == y)
                    {
                        return _bots[i];
                    }
                }
            }

            return null;
        }

        private void CheckBotHealthPoint(Bot bot)
        {
            if (!bot.IsAlive())
            {
                PrintEmpty(bot.XPos, bot.YPos);
                _bots.Remove(bot);
                return;
            }
            
            PrintEmpty(bot.XPos, bot.YPos);
            bot.Print();
        }

        private void FoodInit()
        {
            #region FoodInit
            for (int i = 0; i < (W * H - 64) * spawnCof; i++)
            {
                GenerateFood();
            }
            #endregion
        }

        private void PoisonInit()
        {
            #region PoisonInit
            for (int i = 0; i < (W * H - 64) * spawnCof; i++)
            {
                GeneratePoison();
            }
            #endregion
        }

        private void PrintFood()
        {
            foreach(var food in _foods)
            {
                food.Print();
            }
        }

        private void PrintPoison()
        {
            foreach(var poison in _poisons)
            {
                poison.Print();
            }
        }
    }
}
