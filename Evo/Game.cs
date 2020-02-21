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

        private List<Bot> _bots = new List<Bot>();
        private int _botsCount = 64;

        private List<Food> _foods = new List<Food>();
        private List<Poison> _poisons = new List<Poison>();
        private List<Wall> _walls = new List<Wall>();

        public Game()
        {
            X = 10;
            Y = 5;
            W = 40;
            H = 20;

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
            #endregion

            #region BotInit
            for(int i = 0; i < _botsCount; i++)
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

            #region FoodInit
            for(int i = 0; i < (W*H - 64) * 0.2; i++)
            {
                GenerateFood();
            }
            #endregion

            #region PoisonInit
            for (int i = 0; i < (W * H - 64) * 0.2; i++)
            {
                GeneratePoison();
            }
            #endregion
        }

        public void Start()
        {
            Food food;
            Poison poison;
            Wall wall;
            Bot bot;
            bool isStopGeneration = false;

            Random random = new Random();

            while (true)
            {
                for (int i = 0; true; i++)
                {
                    int commandExecuteCount = 0;
                    while (true)
                    {
                        Print();
                        i %= _bots.Count;

                        int command = _bots[i].GetCurrentCommand();
                        _bots[i].Move();

                        food = CheckFood(_bots[i].XPos, _bots[i].YPos);
                        poison = CheckPoison(_bots[i].XPos, _bots[i].YPos);
                        wall = CheckWall(_bots[i].XPos, _bots[i].YPos);
                        bot = CheckBot(_bots[i].XPos, _bots[i].YPos, i);

                        if (command >= 32 && command <= 63)
                        {
                            if (food != null)
                            {
                                _foods.Remove(food);
                                _bots[i].Eat();

                                _bots[i].MoveCurrentCommand((int)TouchObjects.Food);
                                _bots[i].Damage();

                                CheckBotHealthPoint(_bots[i]);
                            }
                            else if (poison != null)
                            {
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

                            break;
                        }
                        else if (command >= 8 && command <= 15)
                        {
                            if (food != null)
                            {
                                _bots[i].Eat();
                                _bots[i].MoveCurrentCommand((int)TouchObjects.Food);
                            }
                            else if (poison != null)
                            {
                                _foods.Add(new Food()
                                {
                                    X = this.X,
                                    Y = this.Y,
                                    XPos = poison.XPos,
                                    YPos = poison.YPos
                                });
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
                            _bots[i].MoveCurrentCommand(1);
                        }
                        else if (command >= 0 && command <= 7)
                        {
                            _bots[i].Back();
                            _bots[i].UnconditionalTransition();
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
                        Console.Clear();
                    }

                    if(_foods.Count <= 20)
                    {
                        for(int j = 0; j < 10; j++)
                        {
                            GenerateFood();
                        }
                    }

                    if(_poisons.Count <= 20)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            GeneratePoison();
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
                    for(int j = 0; j < _botsCount / botsCount - 1; j++)
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

                        if(j == 6)
                        {
                            newBot.Mutate();
                        }

                        _bots.Add(newBot);
                    }
                }

                _foods.Clear();
                _poisons.Clear();

                FoodInit();
                PoisonInit();

                _generation++;
                isStopGeneration = false;
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

        private void GenerateFood()
        {
            Random random = new Random();
            int x = random.Next(1, W + 1);
            int y = random.Next(1, H + 1);
            while(!IsSpaceFree(x, y))
            {
                x = random.Next(1, W + 1);
                y = random.Next(1, H + 1);
            }

            _foods.Add(new Food()
            {
                X = this.X,
                Y = this.Y,
                XPos = x,
                YPos = y
            });
        }

        private void GeneratePoison()
        {
            Random random = new Random();
            int x = random.Next(1, W + 1);
            int y = random.Next(1, H + 1);
            while (!IsSpaceFree(x, y))
            {
                x = random.Next(1, W + 1);
                y = random.Next(1, H + 1);
            }

            _poisons.Add(new Poison()
            {
                X = this.X,
                Y = this.Y,
                XPos = x,
                YPos = y
            });
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
            if(!bot.IsAlive())
            {
                _bots.Remove(bot);
            }
        }

        private void FoodInit()
        {
            #region FoodInit
            for (int i = 0; i < (W * H - 64) * 0.2; i++)
            {
                GenerateFood();
            }
            #endregion
        }

        private void PoisonInit()
        {
            #region PoisonInit
            for (int i = 0; i < (W * H - 64) * 0.2; i++)
            {
                GeneratePoison();
            }
            #endregion
        }
    }
}
