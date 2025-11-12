namespace Computorv1.Core
{
	/// <summary>
	/// Represents a polynomial equation
	/// </summary>
	public class Polynomial
	{
		private Dictionary<int, double> _terms;

		public Polynomial()
		{
			_terms = new Dictionary<int, double>();
		}

		public void AddTerm(int power, double coefficient)
		{
			if (_terms.ContainsKey(power))
				_terms[power] += coefficient;
			else
				_terms[power] = coefficient;
		}

		public double GetCoefficient(int power)
		{
			return _terms.TryGetValue(power, out double coefficient) ? coefficient : 0;
		}

		public int GetDegree()
		{
			return _terms.Where(kvp => Math.Abs(kvp.Value) > 1e-10)
						.Select(kvp => kvp.Key)
						.DefaultIfEmpty(0)
						.Max();
		}

		public Dictionary<int, double> GetTerms()
		{
			return new Dictionary<int, double>(_terms);
		}

		public string ToReducedForm()
		{
			var sortedTerms = _terms.Where(kvp => Math.Abs(kvp.Value) > 1e-10)
									.OrderBy(kvp => kvp.Key)
									.ToList();

			if (!sortedTerms.Any())
				return "0 * X^0 = 0";

			var terms = new List<string>();
			foreach (var term in sortedTerms)
			{
				terms.Add($"{term.Value} * X^{term.Key}");
			}

			return string.Join(" + ", terms).Replace(" + -", " - ") + " = 0";
		}
	}
}
