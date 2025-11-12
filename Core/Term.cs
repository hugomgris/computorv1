
namespace Computorv1.Core
{
	/// <summary>
	/// Represents a polynomial term (coefficient * X^power)
	/// </summary>
	public class Term
	{
		public double Coefficient { get; set; }
		public int Power { get; set; }

		public Term(double coefficient, int power)
		{
			Coefficient = coefficient;
			Power = power;
		}

		public override string ToString()
		{
			if (Power == 0)
				return $"{Coefficient} * X^0";
			else if (Power == 1)
				return $"{Coefficient} * X^1";
			else
				return $"{Coefficient} * X^{Power}";
		}
	}
}