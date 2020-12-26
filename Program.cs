using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleCraft
{
	class Global
	{
		//Render And Display Settings
		public static int [] NonSolid = {0,5};//Ammount of non solid blocks
		public static int MidPoint = 12;//Manually Set To MidPoint/2 - 1
		public static string Pixel = "[]";
		public static string Player = "{}";
		public static int FrameDelay = 100;//Reccomended Speed
		public static int MapSize = 25;
		public static int Width = 1000;//Make Value Bigger if PC = Beefy
		public static int WaterDepth = 13;//PUT AMMOUNT HERE [Currently Disabled]
		public static int DirtDepth = 3;//<-- PUT YOUR NUMBER HERE
		public static int Highest = 18; //THIS BEING THE HIGHEST VALUE Y CAN BE THE HIGHEST POINT
		public static int Lowest = 10; //THIS BEING THE LOWEST THE Y VALUE CAN BE, NOT THE LOWEST POINT
		public static int [,] MapData = new int [MapSize * Width, MapSize];

		//Functions And Game Rules
		public static bool Run = true;
		public static Random rnd = new Random();//Use Case = Global.rnd.Next(1,10);

		//Player Settings
		public static int InventorySize = 5;
		public static string [] DataName = {"Health", "Hunger", "Thirst", "Oxygen"};
		public static int [] PlayerData = {10, 10, 10, 10};
		public static int [] InventoryName = new int [InventorySize];
		public static int [] InventoryCount = new int [InventorySize]; 
		public static int PlayerX = (MapSize * Width) / 2;
		public static int PlayerY = 4;
		public static int Health = 10;
		public static int Hunger = 10;
		
	}

	class GameControl
	{ 
		public static void Main()
		{
			Console.CursorVisible = false;//Makes the cursor invisible
			Init.Render();

			Thread GLoop = new Thread(new ThreadStart(GameLoop));

			Thread DLoop = new Thread(new ThreadStart(DisplayLoop));

			GLoop.Start();//Starts The Game Loop

			DLoop.Start();//Starts the Display Loop

		}

		public static void GameLoop()
		{
			while (Global.Run)
			{
				
				while (Console.KeyAvailable)
				{
					GameChecks.UserInput();

					GameChecks.Gravity();

					Thread.Sleep(Global.FrameDelay);
					
					break;
				}
				GameChecks.Gravity();
				
			}
		}


		public static void DisplayLoop()
		{
			while (Global.Run)
			{
				Display.Update(); //Will not be delayed to maximise FPS
			}
		}
	}





	class GameChecks //Gravity And Surrounding Checks and Input Checks
	{
		public static void Gravity() //Makes sure the player is standing on the ground.
		{
			if (Global.PlayerY >= Global.MapSize - 2)
			{
				Global.PlayerY -= 1;
			}

			if (Global.PlayerY <= 2)
			{
				Global.PlayerY += 1;
			}

			try
			{
				if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint, Global.PlayerY]) == true)
				{
					Global.PlayerY -= 1;
				}


				if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint, Global.PlayerY+1]) == false)
				{
					Global.PlayerY += 1;	
				}
			}
			catch
			{
				return;
			}
		}




		//User Input And Player Managment
		public static void UserInput()
		{
			//So you dont got into hash-tag land
			if (Global.PlayerX > Global.MapSize * Global.Width - (Global.MapSize / 2) - 3) 
			{
				Global.PlayerX -= 1;
				return;
			}
			if (Global.PlayerX < 0 - (Global.MapSize / 2))
			{
				Global.PlayerX += 1;
				return;
			}
			else
			{
				switch (Console.ReadKey(true).Key)
				{
					//PLAYER X COORD MANAGER

					case ConsoleKey.LeftArrow:
						if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint - 1,Global.PlayerY]) == true)
						{
							if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint - 1,Global.PlayerY-1]) == false)
							{
								if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint,Global.PlayerY-1]) == false)
								{
									Global.PlayerX -= 1;
									return;	
								}
							}
							return;
						}
						Global.PlayerX -= 1;
						return;

					case ConsoleKey.RightArrow:
						if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint + 1,Global.PlayerY]) == true)
						{
							if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint + 1,Global.PlayerY-1]) == false)
							{
								if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint,Global.PlayerY-1]) == false)
								{
									Global.PlayerX += 1;	
									return;
								}
							}
							return;
						}
						Global.PlayerX += 1;
						return;

					//Block Destruction Manager Router

					case ConsoleKey.W:
						BlockDestroy("w");
						return;

					case ConsoleKey.A:
						BlockDestroy("a");
						return;

					case ConsoleKey.S:
						BlockDestroy("s");
						return;

					case ConsoleKey.D:
						BlockDestroy("d");
						return;

					case ConsoleKey.Q:
						BlockDestroy("q");
						return;
						
					case ConsoleKey.E:
						BlockDestroy("e");
						return;
				}
			}
		}
			


		public static void BlockDestroy(string Dir)
		{
			switch (Dir)
			{
				case "s":

					if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint,Global.PlayerY+1]) == false)
					{
						break;
					}

					InventoryManager(Global.MapData[Global.PlayerX+Global.MidPoint,Global.PlayerY+1], 1);

					Global.MapData[Global.PlayerX+Global.MidPoint,Global.PlayerY+1] = 0;

					WaterFill(Global.PlayerX+Global.MidPoint,Global.PlayerY+1);
					
					return;
				case "w":

					if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint,Global.PlayerY-1]) == false)
					{
						break;
					}

					InventoryManager(Global.MapData[Global.PlayerX+Global.MidPoint,Global.PlayerY-1], 1);

					Global.MapData[Global.PlayerX+Global.MidPoint,Global.PlayerY-1] = 0;
					
					WaterFill(Global.PlayerX+Global.MidPoint,Global.PlayerY-1);

					return;
				case "a":

					if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint-1,Global.PlayerY]) == false)
					{
						break;
					}

					InventoryManager(Global.MapData[Global.PlayerX+Global.MidPoint-1,Global.PlayerY], 1);

					Global.MapData[Global.PlayerX+Global.MidPoint-1,Global.PlayerY] = 0;
					
					WaterFill(Global.PlayerX+Global.MidPoint-1,Global.PlayerY);

					return;
				case "d":

					if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint+1,Global.PlayerY]) == false)
					{
						break;
					}

					InventoryManager(Global.MapData[Global.PlayerX+Global.MidPoint+1,Global.PlayerY], 1);

					Global.MapData[Global.PlayerX+Global.MidPoint+1,Global.PlayerY] = 0;

					WaterFill(Global.PlayerX+Global.MidPoint+1,Global.PlayerY);
					
					return;
				case "q":

					if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint-1,Global.PlayerY-1]) == false)
					{
						break;
					}

					InventoryManager(Global.MapData[Global.PlayerX+Global.MidPoint-1,Global.PlayerY-1], 1);

					Global.MapData[Global.PlayerX+Global.MidPoint-1,Global.PlayerY-1] = 0;

					WaterFill(Global.PlayerX+Global.MidPoint-1,Global.PlayerY-1);

					return;
				case "e":

					if (IsSolid(Global.MapData[Global.PlayerX+Global.MidPoint+1,Global.PlayerY-1]) == false)
					{
						break;
					}

					InventoryManager(Global.MapData[Global.PlayerX+Global.MidPoint+1,Global.PlayerY-1], 1);

					Global.MapData[Global.PlayerX+Global.MidPoint+1,Global.PlayerY-1] = 0;

					WaterFill(Global.PlayerX+Global.MidPoint+1,Global.PlayerY-1);

					return;
			}
		}




		public static void InventoryManager(int Item, int Ammount)
		{
			if (IsSolid(Item) == false)
			{
				return;
			}

			for (int i = 0; i < Global.InventoryCount.Length; i++)
			{
				if (Global.InventoryName[i] == Item)
				{
					Global.InventoryCount[i] += Ammount;
					return;
				}
			}
			for (int i = 0; i < Global.InventoryCount.Length; i++)
			{
				if (Global.InventoryName[i] == 0)
				{
					Global.InventoryName[i] = Item;
					Global.InventoryCount[i] = Ammount;
					return;
				}
			}
		}
		





		public static bool IsSolid(int Item) //Returns whether a block is solid or not.
		{
			if (Item == 0) //So you cant dig air?!?!
			{
				return false;
			}
			if (Item == 5) //So you cant dig water lel
			{
				return false;
			}
			else
			{
				return true;
			}
		}







		public static void WaterFill(int BlockX, int BlockY)
		{
			try
			{
				for (int x = -1; x <= 1; x++)
				{
					for (int y = -1; y <= 0; y++)
					{
						if (Global.MapData[BlockX+x, BlockY+y] == 5)
						{
							Global.MapData[BlockX,BlockY] = 5;
						}
					}
				}
			}
			catch
			{
				return;
			}
		}
	}

















	class Display //Will have threads dedicated to rendering
	{
		public static void Update()
		{
			Console.SetCursorPosition(0, 0);
			Print();
		}

		public static void Print()
		{
			for (int y = 0 ; y < Global.MapSize ; y++)
			{
				for (int x = 0 ; x < Global.MapSize ; x++)
				{

					//To reset the colour to stop smudging
					try
					{
						switch (Global.MapData[x + Global.PlayerX, y])
						{
							case 1:
							Console.ForegroundColor = ConsoleColor.Green;  
							break;
						
							case 2:
							Console.ForegroundColor = ConsoleColor.Red;  
							break;
						
							case 3:
							Console.ForegroundColor = ConsoleColor.DarkGray;  
							break;
						
							case 4:
							Console.ForegroundColor = ConsoleColor.DarkMagenta;  
							break;
						
							case 5:
							Console.ForegroundColor = ConsoleColor.Blue;  
							break;
						
							case 6:
							Console.ForegroundColor = ConsoleColor.Magenta;  
							break;
						
							case 7:
							Console.ForegroundColor = ConsoleColor.DarkGreen;  
							break;
						}
						

						Console.Write($"{Global.Pixel}");

						if (x == Global.MidPoint)
						{
							if (y == Global.PlayerY)
							{

								Console.SetCursorPosition(x*2, y);
								Console.ForegroundColor = ConsoleColor.Cyan; 
								Console.Write($"{Global.Player}");
							
							}
						}

						Console.ResetColor();
					}
					catch
					{
						Console.Write("# ");
					}
				}
				Console.WriteLine();
			}

			//User Info Display

			Console.WriteLine("User Data:");
			for (int i=0;i<Global.DataName.Length;i++)
			{
				Console.Write($" - {Global.DataName[i]} -> {Global.PlayerData[i]}\n");
			}

			//Inventory Display
			Console.WriteLine("\nInventory:");
			for (int i=0;i<Global.InventoryName.Length;i++)
			{
				Console.Write($" - Slot {i+1} - Item: {Global.InventoryName[i]} - Ammount: {Global.InventoryCount[i]}\n");
			}
		}
	}













	class Init //Game Rendering and necessary tasks to ensure it works.
	{
		public static void Render()
		{
			int Height = Global.rnd.Next(Global.Lowest, Global.Highest);
			for (int x = 0;x < Global.MapSize * Global.Width;x++)
			{
				for (int y = 0;y < Global.MapSize;y++)
				{
					if (Height >= Global.Highest)
					{
						Height -= 1;
					}
					if (Height <= Global.Lowest)
					{
						Height += 1;
					}
					else
					{
						if (y < Height)
						{
							if (y >= Global.WaterDepth)
							{
								Global.MapData[x, y] = 5; //water
							}
						}
						if (Height == y)
						{
							Global.MapData[x, y] = 1; //grass
						}
						if (Height + 1 == y){
							for (int i = 0; i <= Global.DirtDepth; i++)
							{
								Global.MapData[x, y + i] = 2; //dirt
							}
						}
						else
						{
							if (y > Height + Global.DirtDepth)
							{
								if (Global.rnd.Next(0, 20) == 1)
								{
									Global.MapData[x, y] = 4; //ore
								}
								else
								{
									Global.MapData[x, y] = 3; //stone
								}
							}
						}
					}
				}
				int HeightDiff = Global.rnd.Next(0,4);
				if (HeightDiff == 2)
				{
					Height += 1;
				}
				if (HeightDiff == 1)
				{
					Height -= 1;
				}
			}

			TreePlacement();
		}








		public static void TreePlacement()
		{
		int x = 0;

			while (x < Global.MapSize * Global.Width)
			{
				bool OnGround = false;

				int y = 0;

				x += Global.rnd.Next(5,25);

				while (!OnGround)
				{
					try
					{
						if (GameChecks.IsSolid(Global.MapData[x,y+1]) == true)
						{    
							for (int i = 0; i < Global.rnd.Next(5,7); i++)
							{
								if (y <= 2)
								{
									Bush(x,y);
									break;
								}
								Global.MapData[x,y] = 6;
								y -= 1;
							}
							OnGround = true;
							Bush(x,y);
							break;
						}
						else
						{
							y += 1;
						}

					}
					catch
					{
						break;
					}
				}
			}
		}
		public static void Bush(int x, int y)
		{
			for (int a = -1; a <= 1; a++)
			{
				for (int b = -1; b <= 1; b++)
				{
					Global.MapData[x+a,y+b] = 7;
				}
			} 
		}
	}
}
