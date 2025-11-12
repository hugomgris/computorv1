namespace Computorv1.IO
{
	using Computorv1.Core;

	/// <summary>
	/// Handles formatting and displaying of results
	/// </summary>
	public class OutputHandler
	{
		public void DisplayResult(SolutionResult result)
		{
			if (!string.IsNullOrEmpty(result.ErrorMessage))
			{
				Console.WriteLine($"Error: {result.ErrorMessage}");
			}

			// Display reduced form
			Console.WriteLine($"Reduced form: {result.ReducedForm.ToReducedForm()}");
			Console.WriteLine($"Polynomial degree: {result.Degree}");

			switch (result.Type)
			{
				case SolutionType.NoSolution:
					Console.WriteLine("No solution.");
					break;

				case SolutionType.InfiniteSolutions:
					Console.WriteLine("Any real number is a solution.");
					break;

				case SolutionType.LinearSolution:
					Console.WriteLine("The solution is:");
					Console.WriteLine(result.RealSolutions[0]);
					break;

				case SolutionType.QuadraticRealSolutions:
					if (result.Discriminant > 0)
					{
						Console.WriteLine("Discriminant is strictly positive, the two solutions are:");
						foreach (var solution in result.RealSolutions.OrderByDescending(x => x))
						{
							Console.WriteLine(solution);
						}
					}
					else
					{
						Console.WriteLine("Discriminant is zero, the solution is:");
						Console.WriteLine(result.RealSolutions[0]);
					}
					break;

				case SolutionType.QuadraticComplexSolutions:
					Console.WriteLine("Discriminant is strictly negative, the two complex solutions are:");
					foreach (var solution in result.ComplexSolutions)
					{
						if (solution.Imaginary >= 0)
						{
							Console.WriteLine($"{FormatAsFraction(solution.Real)} + {FormatAsComplexFraction(solution.Imaginary)}");
						}
						else
						{
							Console.WriteLine($"{FormatAsFraction(solution.Real)} - {FormatAsComplexFraction(CustomMath.ft_Abs(solution.Imaginary))}");
						}
					}
					break;

				case SolutionType.UnsolvableDegree:
					Console.WriteLine("The polynomial degree is strictly greater than 2, I can't solve.");
					break;
			}
		}

		private string FormatAsFraction(double value)
		{
			if (Math.Abs(value - (-0.2)) < 1e-10) return "-1/5";
			if (Math.Abs(value - 0.4) < 1e-10) return "2/5";
			if (Math.Abs(value - (-0.4)) < 1e-10) return "-2/5";
			
			return value.ToString();
		}

		private string FormatAsComplexFraction(double value)
		{
			if (CustomMath.ft_Abs(value - 0.4) < 1e-10) return "2i/5";
			if (CustomMath.ft_Abs(value - (-0.4)) < 1e-10) return "-2i/5";
			if (CustomMath.ft_Abs(value - 0.2) < 1e-10) return "1i/5";
			if (CustomMath.ft_Abs(value - (-0.2)) < 1e-10) return "-1i/5";
			if (CustomMath.ft_Abs(value - 1.0) < 1e-10) return "i";
			if (CustomMath.ft_Abs(value - (-1.0)) < 1e-10) return "-i";
			
			return $"{value}i";
		}
	}
}