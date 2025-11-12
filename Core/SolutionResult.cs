namespace Computorv1.Core
{
	public enum SolutionType
	{
		NoSolution,
		InfiniteSolutions,
		LinearSolution,
		QuadraticRealSolutions,
		QuadraticComplexSolutions,
		UnsolvableDegree
	}

	/// <summary>
	/// Represents the result of solving a polynomial equation
	/// </summary>
	public class SolutionResult
	{
		public Polynomial ReducedForm { get; set; } = new Polynomial();
		public int Degree { get; set; }
		public SolutionType Type { get; set; }
		public List<double> RealSolutions { get; set; } = new List<double>();
		public List<(double Real, double Imaginary)> ComplexSolutions { get; set; } = new List<(double, double)>();
		public double? Discriminant { get; set; }
		public string? ErrorMessage { get; set; }
	}
}
