 using Computorv1.Core;
 using Computorv1.IO;

 namespace Computorv1
 {
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var solver = new PolynomialSolver();
				var inputHandler = new InputHandler();
				var outputHandler = new OutputHandler();

				string equation = "";
				bool showGraph = false;

				PrintHeader();

				// Parse command line arguments
				if (args.Length == 0)
				{
					// Read from STDIN
					Console.WriteLine("Enter equation:");
					equation = Console.ReadLine() ?? string.Empty;
				}
				else if (args.Length == 1)
				{
					// Check for graph flag or equation
					if (args[0] == "--graph" || args[0] == "-g")
					{
						Console.WriteLine("Enter equation:");
						equation = Console.ReadLine() ?? string.Empty;
						showGraph = true;
					}
					else
					{
						var validation = inputHandler.ValidateInput(args[0]);
						if (validation.IsValid)
						{
							equation = args[0];
						}
						else
						{
							Console.WriteLine($"Error: {validation.ErrorMessage}");
							return;
						}
					}
				}
				else if (args.Length == 2)
				{
					// Check for equation + graph flag
					if (args[0] == "--graph" || args[0] == "-g")
					{
						var validation = inputHandler.ValidateInput(args[1]);
						if (validation.IsValid)
						{
							equation = args[1];
							showGraph = true;
						}
						else
						{
							Console.WriteLine($"Error: {validation.ErrorMessage}");
							return;
						}
					}
					else if (args[1] == "--graph" || args[1] == "-g")
					{
						var validation = inputHandler.ValidateInput(args[0]);
						if (validation.IsValid)
						{
							equation = args[0];
							showGraph = true;
						}
						else
						{
							Console.WriteLine($"Error: {validation.ErrorMessage}");
							return;
						}
					}
					else
					{
						Console.WriteLine("Usage: ./computor [--graph/-g] [equation] or ./computor (for STDIN input)");
						return;
					}
				}
				else
				{
					Console.WriteLine("Usage: ./computor [--graph/-g] [equation] or ./computor (for STDIN input)");
					return;
				}

				if (string.IsNullOrWhiteSpace(equation))
				{
					Console.WriteLine("No equation provided.");
					return;
				}

				var result = solver.Solve(equation);
				outputHandler.DisplayResult(result, showGraph);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {ex.Message}");
			}
		}

		static public void PrintHeader()
		{
			Console.WriteLine();
			try
			{
				string[] lines = File.ReadAllLines("header_banner.txt");
				foreach (string line in lines)
				{
					Console.WriteLine(line);
				}
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Header banner file not found");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error reading header: {ex.Message}");
			}
		}
	}
 }
