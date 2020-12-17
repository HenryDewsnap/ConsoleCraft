using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//--------//
//TERRARIA//
//--------// 


namespace Game
{
	class Global
	{
		//Render And Display Settings
		public static int [] NonSolid = {0,5}; //Ammount of non solid blocks
		public static int Frames = 0;
		public static int FramesPerSecond = 0;
		public static int MidPoint = 13; //Manually Set To MidPoint/2 - 1
		public static string Pixel = "[";
		public static string PixelGap = "]";
		public static string PlayerChar = "()";
		public static int FrameDelay = 100; //Reccomended Speed
		public static int MapSize = 25;
		public static int Width = 1000; //Make Value Bigger if PC = Beefy
		public static int WaterDepth = 15; //PUT AMMOUNT HERE [Currently Disabled]
		public static int DirtThickness = 3; //<-- PUT YOUR NUMBER HERE
		public static int MaxHigh = 20;
		public static int MaxLow = 5;
		public static int [,] MapData = new int [MapSize * Width, MapSize];

		//Functions And Game Rules
		public static bool Run = true;
		public static Random rnd = new Random(); //Use Case = Global.rnd.Next(1,10);

		//Player Settings
		public static int InventorySize = 5;

		public static string [] DataName = {"Health", "Hunger", "Thirst"};
		public static int [] PlayerData = {10, 10, 10};

		public static int [] InventoryName = new int [InventorySize];
		public static int [] InventoryCount = new int [InventorySize]; 

		public static int PlayerX = (MapSize * Width) / 2;
		public static int PlayerY = 4;
		
		public static int Health = 10;
		public static int Hunger = 10;
		
	}

	class Program
	{
		
		//Master Sub-Routine
		public static void Main()
		{ 
			
			Render(Global.MapSize, Global.DirtThickness, Global.MaxHigh, Global.MaxLow, Global.rnd.Next(Global.MaxLow, Global.MaxHigh), Global.Width, Global.WaterDepth);

			Console.CursorVisible = false;
			//Initialising Thread
			Thread t = new Thread(new ThreadStart(GameLoop));

			//Starting Thread
			t.Start();
		}

		//Game Loop That Times The Actions Made By The Client
		public static void GameLoop()
		{
			
			Thread FPS = new Thread(new ThreadStart(FPSCount));

			FPS.Start();

			Console.Clear();

			ConsoleKeyInfo Input;

			while (Global.Run)
			{	
				while (!Console.KeyAvailable)
				{
					Gravity();
					
					DisplayUpdate();

					Thread.Sleep(Global.FrameDelay);
				}
				UserInput();
				
				
			}
		}

	

		//Graphical Display Feature
		public static void DisplayUpdate()
		{
			Console.SetCursorPosition(0, 0);
			Display();
		}

		public static void Display()
		{
			for (int y = 0 ; y < Global.MapSize ; y++)
			{
				for (int x = 0 ; x < Global.MapSize ; x++)
				{

					//To reset the colour to stop smudging
					try
					{
						if (Global.MapData[x + Global.PlayerX ,y] == 1)
						{
							Console.ForegroundColor = ConsoleColor.Green;  
						}					
						if (Global.MapData[x + Global.PlayerX ,y] == 2)
						{
							Console.ForegroundColor = ConsoleColor.Red;  
						}
						if (Global.MapData[x + Global.PlayerX ,y] == 3)
						{
							Console.ForegroundColor = ConsoleColor.DarkGray; 
						}
						if (Global.MapData[x + Global.PlayerX ,y] == 4)
						{
							Console.ForegroundColor = ConsoleColor.DarkMagenta;
						}
						if (Global.MapData[x + Global.PlayerX ,y] == 5)
						{
							Console.ForegroundColor = ConsoleColor.Blue;
						}

						Console.Write($"{Global.Pixel}{Global.PixelGap}");

						if (x == Global.MidPoint)
						{
							if (y == Global.PlayerY)
							{

								Console.SetCursorPosition(x*2, y);
								Console.ForegroundColor = ConsoleColor.Cyan; 
								Console.Write($"{Global.PlayerChar}");
							
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

			//Game Data Display
			Console.WriteLine($"\nGame Data:");
			Console.WriteLine($" - Frames Per Second -> {Global.FramesPerSecond}");
			Console.WriteLine($" - Player X Coordinate -> {Global.PlayerX}");
			Console.WriteLine($" - Player Y Coordinate -> {Global.MapSize - Global.PlayerY}");

			Global.Frames += 1;
		}
		
		//Map Data Rendering and Preperation
		public static void Render(int Size, int DirtAmmount, int MaxHeight, int MinHeight, int Height, int Width, int WaterHeightLevel)
		{
			for (int x = 0;x < Size * Width;x++)
			{
				for (int y = 0;y < Size;y++)
				{
					if (Height >= MaxHeight)
					{
						Height -= 1;
					}
					if (Height <= MinHeight)
					{
						Height += 1;
					}
					else{
						if (y < Height){
							if (y >= WaterHeightLevel)
							{
								Global.MapData[x, y] = 5; //water
							}
						}
						if (Height == y)
						{
							Global.MapData[x, y] = 1; //grass
						}
						if (Height + 1 == y){
							for (int i = 0; i <= DirtAmmount; i++)
							{
								Global.MapData[x, y + i] = 2; //dirt
							}
						}
						else{
							if (y > Height + DirtAmmount){
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
				int HeightDiff = Global.rnd.Next(0,3);
				if (HeightDiff == 2)
				{
					Height += 1;
				}
				if (HeightDiff == 1)
				{
					Height -= 1;
				}
			}
		}

		//User Input And Player Managment
		public static void UserInput()
		{
			//So you dont got into hash-tag land
			if (Global.PlayerX > Global.MapSize * Global.Width - (Global.MapSize / 2) - 3) 
			{
				Global.PlayerX -= 1;
			}
			if (Global.PlayerX < 0 - (Global.MapSize / 2))
			{
				Global.PlayerX += 1;
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
								}
							}
							return;
						}
						Global.PlayerX += 1;
						return;


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


		//Gravity Management
		public static void Gravity()
		{
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

		public static void FPSCount()
		{
			while (Global.Run)
			{
				Global.FramesPerSecond = Global.Frames;	

				Global.Frames = 0;

				Thread.Sleep(1000);
			}
		}
		
		public static void BlockDestroy(string Dir)
		{
			switch (Dir){
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

		public static bool IsSolid(int Item)
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
		
		public static void WaterFill(int BrokenBlockX, int BrokenBlockY)
		{
			try
			{
				for (int x = -1; x <= 1; x++)
				{
					for (int y = 0; y <= 1; y++)
					{
						if (Global.MapData[BrokenBlockX-x, BrokenBlockY-y] == 5)
						{
							Global.MapData[BrokenBlockX,BrokenBlockY] = 5;
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
}
