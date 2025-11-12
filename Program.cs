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

				string equation;

				PrintHeader();

				if (args.Length == 0)
				{
					// Read from STDIN
					Console.WriteLine("Enter equation:");
					equation = Console.ReadLine() ?? string.Empty;
				}
				else if (args.Length == 1)
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
				else
				{
					Console.WriteLine("Usage: ./computor [equation] or ./computor (for STDIN input)");
					return;
				}

				if (string.IsNullOrWhiteSpace(equation))
				{
					Console.WriteLine("Noe equation provided.");
					return;
				}

				var result = solver.Solve(equation);
				outputHandler.DisplayResult(result);
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
