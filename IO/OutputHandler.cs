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
				return;
			}

			switch (result.Type)
			{
				case SolutionType.NoSolution:
					Console.WriteLine("No solution exists.");
					break;

				case SolutionType.InfiniteSolutions:
					Console.WriteLine("Infinite solutions (any real number).");
					break;

				case SolutionType.LinearSolution:
					Console.WriteLine("The solution is:");
					Console.WriteLine(FormatSolution(result.RealSolutions[0]));
					break;

				case SolutionType.QuadraticRealSolutions:
					if (result.Discriminant > 0)
					{
						Console.WriteLine("Discriminant is strictly positive, the two solutions are:");
						foreach (var solution in result.RealSolutions.OrderByDescending(x => x))
						{
							Console.WriteLine(FormatSolution(solution));
						}
					}
					else
					{
						Console.WriteLine("Discriminant is zero, the solution is:");
						Console.WriteLine(FormatSolution(result.RealSolutions[0]));
					}
					break;

				case SolutionType.QuadraticComplexSolutions:
					Console.WriteLine("Discriminant is strictly negative, the two complex solutions are:");
					foreach (var solution in result.ComplexSolutions)
					{
						if (solution.Imaginary >= 0)
						{
							Console.WriteLine($"{FormatSolution(solution.Real)} + {FormatComplexPart(solution.Imaginary)}");
						}
						else
						{
							Console.WriteLine($"{FormatSolution(solution.Real)} - {FormatComplexPart(CustomMath.ft_Abs(solution.Imaginary))}");
						}
					}
					break;

				case SolutionType.UnsolvableDegree:
					Console.WriteLine("The polynomial degree is strictly greater than 2, I can't solve.");
					break;
			}
		}

		private string FormatSolution(double value)
		{
			// Handle special cases
			if (CustomMath.ft_Abs(value) < 1e-10)
				return "0";

			// Try to convert to fraction
			var fraction = ConvertToFraction(value);
			
			if (fraction.HasValue && IsInterestingFraction(fraction.Value.numerator, fraction.Value.denominator))
			{
				if (fraction.Value.denominator == 1)
					return fraction.Value.numerator.ToString();
				else
					return $"{fraction.Value.numerator}/{fraction.Value.denominator}";
			}
			
			// Fall back to decimal
			return value.ToString();
		}

		private string FormatComplexPart(double imaginaryValue)
		{
			// Special case for coefficient of 1
			if (CustomMath.ft_Abs(imaginaryValue - 1.0) < 1e-10)
				return "i";

			// Try fraction format for imaginary part
			var fraction = ConvertToFraction(imaginaryValue);
			
			if (fraction.HasValue && IsInterestingFraction(fraction.Value.numerator, fraction.Value.denominator))
			{
				if (fraction.Value.denominator == 1)
					return $"{fraction.Value.numerator}i";
				else
					return $"{fraction.Value.numerator}i/{fraction.Value.denominator}";
			}
			
			return $"{imaginaryValue}i";
		}

		private (int numerator, int denominator)? ConvertToFraction(double value)
		{
			const int maxDenominator = 50;
			const double tolerance = 1e-10;

			bool isNegative = value < 0;
			value = CustomMath.ft_Abs(value);

			for (int denominator = 1; denominator <= maxDenominator; denominator++)
			{
				int numerator = ft_Round(value * denominator);
				
				if (CustomMath.ft_Abs(value - (double)numerator / denominator) < tolerance)
				{
					int gcd = GreatestCommonDivisor(numerator >= 0 ? numerator : -numerator, denominator);
					numerator /= gcd;
					denominator /= gcd;
					
					if (isNegative)
						numerator = -numerator;
						
					return (numerator, denominator);
				}
			}

			return null;
		}

		private bool IsInterestingFraction(int numerator, int denominator)
		{
			return denominator <= 20 && denominator > 0;
		}

		private int GreatestCommonDivisor(int a, int b)
		{
			while (b != 0)
			{
				int temp = b;
				b = a % b;
				a = temp;
			}
			return a;
		}
	}
}