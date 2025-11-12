namespace Computorv1.Core
{
	/// <summary>
	/// Main solver for polynomial equations
	/// </summary>
	public class PolynomialSolver
	{
		private readonly PolynomialParser _parser;

		public PolynomialSolver()
		{
			_parser = new PolynomialParser();
		}

		public SolutionResult Solve(string equation)
		{
			try
			{
				var polynomial = _parser.ParseEquation(equation);
				var result = new SolutionResult
				{
					ReducedForm = polynomial,
					Degree = polynomial.GetDegree()
				};

				return SolvePolynomial(result);
			}
			catch (Exception ex)
			{
				return new SolutionResult
				{
					Type = SolutionType.NoSolution,
					ErrorMessage = ex.Message
				};
			}
		}

		private SolutionResult SolvePolynomial(SolutionResult result)
		{
			var polynomial = result.ReducedForm;

			switch (result.Degree)
			{
				case 0:
					return SolveDegree0(result);
				case 1:
					return SolveDegree1(result);
				case 2:
					return SolveDegree2(result);
				default:
					result.Type = SolutionType.UnsolvableDegree;
					return result;
			}
		}

		private SolutionResult SolveDegree0(SolutionResult result)
		{
			var constant = result.ReducedForm.GetCoefficient(0);

			if (CustomMath.ft_Abs(constant) < 1e-10)
			{
				result.Type = SolutionType.InfiniteSolutions;
			}
			else
			{
				result.Type = SolutionType.NoSolution;
			}

			return result;
		}

		private SolutionResult SolveDegree1(SolutionResult result)
		{
			var a = result.ReducedForm.GetCoefficient(1);
			var b = result.ReducedForm.GetCoefficient(0);

			result.Type = SolutionType.LinearSolution;
			result.RealSolutions.Add(-b / a);

			return result;
		}

		private SolutionResult SolveDegree2(SolutionResult result)
		{
			var a = result.ReducedForm.GetCoefficient(2);
			var b = result.ReducedForm.GetCoefficient(1);
			var c = result.ReducedForm.GetCoefficient(0);

			var discriminant = b * b - 4 * a * c;
			result.Discriminant = discriminant;

			if (discriminant > 0)
			{
				result.Type = SolutionType.QuadraticRealSolutions;
				var sqrtD = CustomMath.ft_Sqrt(discriminant);
				result.RealSolutions.Add((-b + sqrtD) / (2 * a));
				result.RealSolutions.Add((-b - sqrtD) / (2 * a));
			}
			else if (CustomMath.ft_Abs(discriminant) < 1e-10)
			{
				result.Type = SolutionType.QuadraticRealSolutions;
				result.RealSolutions.Add(-b / (2 * a));
			}
			else
			{
				result.Type = SolutionType.QuadraticComplexSolutions;
				var realPart = -b / (2 * a);
				var imaginaryPart = CustomMath.ft_Sqrt(-discriminant) / (2 * a);
				result.ComplexSolutions.Add((realPart, imaginaryPart));
				result.ComplexSolutions.Add((realPart, -imaginaryPart));
			}

			return result;
		}
	}
}