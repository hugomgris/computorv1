namespace Computorv1.IO
{
	using Computorv1.Core;

	/// <summary>
	/// Handles formatting and displaying of results
	/// </summary>
	public class OutputHandler
	{
		public void DisplayResult(SolutionResult result, bool showGraph = false)
		{
			if (!string.IsNullOrEmpty(result.ErrorMessage))
			{
				Console.WriteLine($"Error: {result.ErrorMessage}");
				return;
			}

			DisplayReducedForm(result.ReducedForm);
			
			Console.WriteLine($"Polynomial degree: {result.Degree}");

			// Display intermediate steps based on the type
			DisplayIntermediateSteps(result);

			if (result.Type == SolutionType.QuadraticRealSolutions || 
				result.Type == SolutionType.QuadraticComplexSolutions)
			{
				Console.WriteLine($"Discriminant: {result.Discriminant}");
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

			// Display graphical representation for polynomial degrees 1 and 2
			if (showGraph && result.Degree >= 1 && result.Degree <= 2 && result.Type != SolutionType.UnsolvableDegree)
			{
				DisplayGraphicalRepresentation(result);
			}
		}

		private void DisplayGraphicalRepresentation(SolutionResult result)
		{
			// Find approximate roots numerically
			var approximateRoots = CustomMath.FindApproximateRoots(result.ReducedForm);
			
			if (approximateRoots.Count > 0)
			{
				Console.WriteLine("\nNumerical root verification:");
				foreach (var root in approximateRoots)
				{
					double value = CustomMath.EvaluatePolynomial(result.ReducedForm, root);
					Console.WriteLine($"f({FormatSolution(root)}) ≈ {FormatSolution(value)}");
				}
			}

			// Draw the graph
			CustomMath.DrawPolynomialGraph(result.ReducedForm);
			
			// Show the analytical solutions on the graph
			if (result.RealSolutions.Count > 0)
			{
				Console.WriteLine("\nAnalytical solutions marked with coordinates:");
				foreach (var solution in result.RealSolutions)
				{
					double yValue = CustomMath.EvaluatePolynomial(result.ReducedForm, solution);
					Console.WriteLine($"Root: ({FormatSolution(solution)}, {FormatSolution(yValue)})");
				}
			}
		}

		private void DisplayReducedForm(Polynomial polynomial)
		{
			Console.Write("Reduced form: ");
			
			var terms = polynomial.GetTerms().OrderBy(t => t.Key).ToList();
			var termStrings = new List<string>();
			
			foreach (var term in terms)
			{
				if (CustomMath.ft_Abs(term.Value) < 1e-10)
					continue;
				
				string sign = term.Value >= 0 ? "+" : "-";
				double absCoeff = CustomMath.ft_Abs(term.Value);
				
				string coeffStr;
				if (CustomMath.ft_Abs(absCoeff - 1.0) < 1e-10 && term.Key > 0)
				{
					coeffStr = "";
				}
				else
				{
					var fraction = ConvertToFraction(absCoeff);
					if (fraction.HasValue && IsInterestingFraction(fraction.Value.numerator, fraction.Value.denominator))
					{
						if (fraction.Value.denominator == 1)
							coeffStr = fraction.Value.numerator.ToString();
						else
							coeffStr = $"{fraction.Value.numerator}/{fraction.Value.denominator}";
					}
					else
					{
						coeffStr = absCoeff.ToString();
					}
				}
				
				string termStr;
				if (term.Key == 0)
				{
					termStr = coeffStr;
				}
				else
				{
					string varPart = term.Key == 1 ? "X" : $"X^{term.Key}";
					termStr = string.IsNullOrEmpty(coeffStr) ? varPart : $"{coeffStr} * {varPart}";
				}
				
				if (termStrings.Count == 0)
				{
					termStrings.Add(term.Value >= 0 ? termStr : $"-{termStr}");
				}
				else
				{
					termStrings.Add($" {sign} {termStr}");
				}
			}
			
			if (termStrings.Count == 0)
			{
				Console.WriteLine("0 = 0");
			}
			else
			{
				Console.WriteLine(string.Join("", termStrings) + " = 0");
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
				int numerator = CustomMath.ft_Round(value * denominator);
				
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

		private void DisplayIntermediateSteps(SolutionResult result)
		{
			Console.WriteLine("\nSolving steps:");
			
			switch (result.Type)
			{
				case SolutionType.NoSolution:
					Console.WriteLine("Step 1: Check if the equation is consistent");
					Console.WriteLine($"Step 2: We have 0 * X^0 = {result.ReducedForm.GetCoefficient(0)}");
					Console.WriteLine("Step 3: Since 0 ≠ constant, there is no solution");
					break;

				case SolutionType.InfiniteSolutions:
					Console.WriteLine("Step 1: Check the equation structure");
					Console.WriteLine("Step 2: We have 0 * X^0 = 0 (or 0 = 0)");
					Console.WriteLine("Step 3: This is always true, so any real number is a solution");
					break;

				case SolutionType.LinearSolution:
					DisplayLinearSteps(result);
					break;

				case SolutionType.QuadraticRealSolutions:
				case SolutionType.QuadraticComplexSolutions:
					DisplayQuadraticSteps(result);
					break;

				case SolutionType.UnsolvableDegree:
					Console.WriteLine("Step 1: Identify the polynomial degree");
					Console.WriteLine($"Step 2: Degree is {result.Degree}, which is greater than 2");
					Console.WriteLine("Step 3: This program only solves polynomial equations up to degree 2");
					break;
			}
		}

		private void DisplayLinearSteps(SolutionResult result)
		{
			var a = result.ReducedForm.GetCoefficient(1);
			var b = result.ReducedForm.GetCoefficient(0);

			Console.WriteLine("Step 1: Identify this as a linear equation (ax + b = 0)");
			Console.WriteLine($"Step 2: We have {FormatCoefficient(a)} * X + ({FormatCoefficient(b)}) = 0");
			Console.WriteLine($"Step 3: Rearrange to isolate X: {FormatCoefficient(a)} * X = {FormatCoefficient(-b)}");
			Console.WriteLine($"Step 4: Divide both sides by {FormatCoefficient(a)}: X = {FormatCoefficient(-b)} / {FormatCoefficient(a)}");
			Console.WriteLine($"Step 5: Simplify: X = {FormatSolution(-b / a)}");
		}

		private void DisplayQuadraticSteps(SolutionResult result)
		{
			var a = result.ReducedForm.GetCoefficient(2);
			var b = result.ReducedForm.GetCoefficient(1);
			var c = result.ReducedForm.GetCoefficient(0);
			var discriminant = result.Discriminant!.Value;

			Console.WriteLine("Step 1: Identify this as a quadratic equation (ax² + bx + c = 0)");
			Console.WriteLine($"Step 2: Extract coefficients: a = {FormatCoefficient(a)}, b = {FormatCoefficient(b)}, c = {FormatCoefficient(c)}");
			Console.WriteLine("Step 3: Use the quadratic formula: x = (-b ± √(b² - 4ac)) / (2a)");
			Console.WriteLine($"Step 4: Calculate discriminant: Δ = b² - 4ac");
			Console.WriteLine($"Step 5: Δ = ({FormatCoefficient(b)})² - 4({FormatCoefficient(a)})({FormatCoefficient(c)})");
			Console.WriteLine($"Step 6: Δ = {FormatCoefficient(b * b)} - {FormatCoefficient(4 * a * c)} = {FormatCoefficient(discriminant)}");

			if (discriminant > 0)
			{
				var sqrtD = CustomMath.ft_Sqrt(discriminant);
				Console.WriteLine($"Step 7: Since Δ > 0, we have two real solutions");
				Console.WriteLine($"Step 8: √Δ = √{FormatCoefficient(discriminant)} = {FormatCoefficient(sqrtD)}");
				Console.WriteLine($"Step 9: x₁ = (-b + √Δ) / (2a) = ({FormatCoefficient(-b)} + {FormatCoefficient(sqrtD)}) / {FormatCoefficient(2 * a)}");
				Console.WriteLine($"Step 10: x₁ = {FormatCoefficient(-b + sqrtD)} / {FormatCoefficient(2 * a)} = {FormatSolution((-b + sqrtD) / (2 * a))}");
				Console.WriteLine($"Step 11: x₂ = (-b - √Δ) / (2a) = ({FormatCoefficient(-b)} - {FormatCoefficient(sqrtD)}) / {FormatCoefficient(2 * a)}");
				Console.WriteLine($"Step 12: x₂ = {FormatCoefficient(-b - sqrtD)} / {FormatCoefficient(2 * a)} = {FormatSolution((-b - sqrtD) / (2 * a))}");
			}
			else if (CustomMath.ft_Abs(discriminant) < 1e-10)
			{
				Console.WriteLine($"Step 7: Since Δ = 0, we have one repeated real solution");
				Console.WriteLine($"Step 8: x = -b / (2a) = {FormatCoefficient(-b)} / {FormatCoefficient(2 * a)} = {FormatSolution(-b / (2 * a))}");
			}
			else
			{
				var sqrtAbsD = CustomMath.ft_Sqrt(-discriminant);
				Console.WriteLine($"Step 7: Since Δ < 0, we have two complex solutions");
				Console.WriteLine($"Step 8: √(-Δ) = √{FormatCoefficient(-discriminant)} = {FormatCoefficient(sqrtAbsD)}");
				Console.WriteLine($"Step 9: x = (-b ± i√(-Δ)) / (2a)");
				Console.WriteLine($"Step 10: x₁ = ({FormatCoefficient(-b)} + {FormatCoefficient(sqrtAbsD)}i) / {FormatCoefficient(2 * a)}");
				Console.WriteLine($"Step 11: x₁ = {FormatSolution(-b / (2 * a))} + {FormatComplexPart(sqrtAbsD / (2 * a))}");
				Console.WriteLine($"Step 12: x₂ = ({FormatCoefficient(-b)} - {FormatCoefficient(sqrtAbsD)}i) / {FormatCoefficient(2 * a)}");
				Console.WriteLine($"Step 13: x₂ = {FormatSolution(-b / (2 * a))} - {FormatComplexPart(sqrtAbsD / (2 * a))}");
			}
		}

		private string FormatCoefficient(double value)
		{
			if (CustomMath.ft_Abs(value) < 1e-10)
				return "0";
			if (CustomMath.ft_Abs(value - 1.0) < 1e-10)
				return "1";
			if (CustomMath.ft_Abs(value + 1.0) < 1e-10)
				return "-1";

			var fraction = ConvertToFraction(value);
			
			if (fraction.HasValue && IsInterestingFraction(fraction.Value.numerator, fraction.Value.denominator))
			{
				if (fraction.Value.denominator == 1)
					return fraction.Value.numerator.ToString();
				else
					return $"{fraction.Value.numerator}/{fraction.Value.denominator}";
			}
			
			return value.ToString();
		}
	}
}