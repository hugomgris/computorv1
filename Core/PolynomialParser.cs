using System.Text.RegularExpressions;

namespace Computorv1.Core
{
	/// <summary>
	/// Parses polynomial equations from string input
	/// </summary>
	public class PolynomialParser
	{
		public Polynomial ParseEquation(string equation)
		{
			var parts = equation.Split('=');
			if (parts.Length != 2)
				throw new ArgumentException("Invalid equation format: must contain exactly one '=' sign");

			var leftSide = ParseSide(parts[0].Trim());
			var rightSide = ParseSide(parts[1].Trim());

			var result = new Polynomial();

			foreach (var term in leftSide.GetTerms())
			{
				result.AddTerm(term.Key, term.Value);
			}

			foreach (var term in rightSide.GetTerms())
			{
				result.AddTerm(term.Key, -term.Value); // negative because of side switch
			}

			return result;
		}

		private Polynomial ParseSide(string side)
		{
			var polynomial = new Polynomial();
			
			side = side.Replace(" ", "");
			if (!side.StartsWith("+") && !side.StartsWith("-"))
				side = "+" + side;
			
			string remaining = side;
			
			var xPowerPattern = @"([+-]?\d*\.?\d*)\*?[xX]\^(\d+)";
			var xPowerMatches = Regex.Matches(remaining, xPowerPattern);
			
			foreach (Match match in xPowerMatches)
			{
				var coeffStr = match.Groups[1].Value;
				var powerStr = match.Groups[2].Value;
				
				double coeff = ParseCoefficient(coeffStr);
				int power = int.Parse(powerStr);
				
				polynomial.AddTerm(power, coeff);
				
				remaining = remaining.Replace(match.Value, "");
			}
			
			var xPattern = @"([+-]?\d*\.?\d*)\*?[xX]";
			var xMatches = Regex.Matches(remaining, xPattern);
			
			foreach (Match match in xMatches)
			{
				var coeffStr = match.Groups[1].Value;
				double coeff = ParseCoefficient(coeffStr);
				
				polynomial.AddTerm(1, coeff);
				
				remaining = remaining.Replace(match.Value, "");
			}
			
			var constantPattern = @"[+-]?\d+\.?\d*";
			var constantMatches = Regex.Matches(remaining, constantPattern);
			
			foreach (Match match in constantMatches)
			{
				if (!string.IsNullOrWhiteSpace(match.Value))
				{
					double coeff = double.Parse(match.Value);
					polynomial.AddTerm(0, coeff);
				}
			}
			
			return polynomial;
		}

		private double ParseCoefficient(string coeffStr)
		{
			if (string.IsNullOrEmpty(coeffStr) || coeffStr == "+")
				return 1;
			if (coeffStr == "-")
				return -1;
			
			return double.Parse(coeffStr);
		}
	}
}
