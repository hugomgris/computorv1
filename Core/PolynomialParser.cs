using System.Text.RegularExpressions;

namespace Computorv1.Core
{
	/// <summary>
	/// Parses polynomial equations from string input supporting both mandatory and free-form formats
	/// with comprehensive error validation and specific error messages
	/// </summary>
	public class PolynomialParser
	{
		private static readonly HashSet<char> ValidCharacters = new HashSet<char>
		{
			'X', 'x', '+', '-', '*', '^', '=', '.', ' ',
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
		};

		private static readonly string[] StandardPatterns = new string[]
		{
			@"([+-]?)\s*(\d*\.?\d*)\s*\*\s*X\s*\^\s*(\d+)",  // 5 * X^2 or -3.2 * X^1
			@"([+-]?)\s*X\s*\^\s*(\d+)",                      // X^2 or -X^3
			@"([+-]?)\s*(\d*\.?\d*)\s*\*?\s*X\b",            // 5*X or 5X or -2X
			@"([+-]?)\s*X\b",                                 // X or -X
			@"([+-]?)\s*(\d+\.?\d*)"                          // Constants like 5 or -3.2
		};

		private static readonly string[] FreeFormPatterns = new string[]
		{
			@"([+-]?)(\d*\.?\d*)X\^(\d+)",     // 5X^2 or -3.2X^1
			@"([+-]?)X\^(\d+)",                // X^2 or -X^3  
			@"([+-]?)(\d*\.?\d*)X\b",          // 5X or -2X
			@"([+-]?)X\b",                     // X or -X
			@"([+-]?)(\d+\.?\d*)"              // Constants
		};

		public Polynomial ParseEquation(string equation)
		{
			ValidateEquationFormat(equation);
			
			var parts = equation.Split('=');
			if (parts.Length != 2)
				throw new ArgumentException("Invalid equation format: must contain exactly one '=' sign");

			ValidateSide(parts[0].Trim(), "left");
			ValidateSide(parts[1].Trim(), "right");

			var leftSide = ParseSide(parts[0].Trim());
			var rightSide = ParseSide(parts[1].Trim());

			var result = new Polynomial();

			foreach (var term in leftSide.GetTerms())
			{
				result.AddTerm(term.Key, term.Value);
			}

			foreach (var term in rightSide.GetTerms())
			{
				result.AddTerm(term.Key, -term.Value);
			}

			return result;
		}

		private void ValidateEquationFormat(string equation)
		{
			if (string.IsNullOrWhiteSpace(equation))
				throw new ArgumentException("Empty equation provided");

			// Check for invalid characters
			foreach (char c in equation)
			{
				if (!ValidCharacters.Contains(c))
				{
					throw new ArgumentException($"Invalid character '{c}' found in equation. Only mathematical symbols and X variable are allowed.");
				}
			}

			// Check for multiple equals signs
			int equalsCount = equation.Count(c => c == '=');
			if (equalsCount == 0)
				throw new ArgumentException("Missing equals sign (=) in equation");
			if (equalsCount > 1)
				throw new ArgumentException("Too many equals signs - equation must have exactly one '='");

			// Check for invalid variable names (only X/x allowed)
			// Look for letters that are not X or x
			var invalidVariables = System.Text.RegularExpressions.Regex.Matches(equation, @"[A-WYZa-wyz]");
			if (invalidVariables.Count > 0)
			{
				var firstInvalid = invalidVariables[0].Value;
				throw new ArgumentException($"Invalid variable '{firstInvalid}' found. Only 'X' or 'x' is allowed as variable.");
			}
		}

		private void ValidateSide(string side, string sideName)
		{
			if (string.IsNullOrWhiteSpace(side))
				throw new ArgumentException($"{char.ToUpper(sideName[0])}{sideName.Substring(1)} side of equation is empty");

			string cleanSide = side.Replace(" ", "");

			if (HasConsecutiveOperators(cleanSide))
				throw new ArgumentException($"Consecutive operators found on {sideName} side");

			if (cleanSide.StartsWith("*") || cleanSide.StartsWith("/") || cleanSide.StartsWith("^"))
				throw new ArgumentException($"Equation {sideName} side starts with invalid operator '{cleanSide[0]}'");

			if (cleanSide.EndsWith("*") || cleanSide.EndsWith("/") || cleanSide.EndsWith("^") || cleanSide.EndsWith("+") || cleanSide.EndsWith("-"))
				throw new ArgumentException($"Equation {sideName} side ends with incomplete operator '{cleanSide[cleanSide.Length - 1]}'");

			ValidatePowers(cleanSide, sideName);

			ValidateDecimals(cleanSide, sideName);
		}

		private bool HasConsecutiveOperators(string input)
		{
			string operators = "+-*/^";
			for (int i = 0; i < input.Length - 1; i++)
			{
				if (operators.Contains(input[i]) && operators.Contains(input[i + 1]))
				{
					if ((input[i] == '+' || input[i] == '-') && 
						(input[i + 1] == '+' || input[i + 1] == '-'))
						continue;
					return true;
				}
			}
			return false;
		}

		private void ValidatePowers(string input, string sideName)
		{
			var powerPattern = @"[xX]\^([^+\-*/=\s]+)";
			var matches = System.Text.RegularExpressions.Regex.Matches(input, powerPattern);

			foreach (System.Text.RegularExpressions.Match match in matches)
			{
				string powerStr = match.Groups[1].Value;

				if (string.IsNullOrEmpty(powerStr))
					throw new ArgumentException($"Missing power after ^ on {sideName} side");

				if (powerStr.Contains('.'))
					throw new ArgumentException($"Decimal powers not allowed: X^{powerStr} on {sideName} side");

				if (powerStr.StartsWith("-"))
					throw new ArgumentException($"Negative powers not allowed: X^{powerStr} on {sideName} side");

				if (!powerStr.All(char.IsDigit))
					throw new ArgumentException($"Invalid power format: X^{powerStr} on {sideName} side. Powers must be non-negative integers.");

				if (int.TryParse(powerStr, out int power) && power > 100)
					throw new ArgumentException($"Power too large: X^{powerStr} on {sideName} side. Maximum power is 100.");
			}
		}

		private void ValidateDecimals(string input, string sideName)
		{
			var decimalPattern = @"\d*\.\d*";
			var matches = System.Text.RegularExpressions.Regex.Matches(input, decimalPattern);

			foreach (System.Text.RegularExpressions.Match match in matches)
			{
				string decimalStr = match.Value;

				if (decimalStr == "." || decimalStr.StartsWith(".") || decimalStr.EndsWith("."))
				{
					if (decimalStr.Replace(".", "").Length == 0)
						throw new ArgumentException($"Invalid decimal format '{decimalStr}' on {sideName} side");
				}

				if (decimalStr.Count(c => c == '.') > 1)
					throw new ArgumentException($"Invalid decimal format '{decimalStr}' on {sideName} side. Multiple decimal points found.");
			}
		}

		private Polynomial ParseSide(string side)
		{
			var polynomial = new Polynomial();
			
			// Normalize: convert to uppercase, clean up spaces
			string normalizedSide = side.Replace('x', 'X').Replace(" ", "");
			
			// Add leading + if no sign
			if (!normalizedSide.StartsWith("+") && !normalizedSide.StartsWith("-"))
				normalizedSide = "+" + normalizedSide;
			
			// Split by + and - while keeping the operators
			var parts = SplitPreservingOperators(normalizedSide);
			
			foreach (string part in parts)
			{
				if (string.IsNullOrWhiteSpace(part)) continue;
				
				var term = ParseTerm(part.Trim());
				polynomial.AddTerm(term.Power, term.Coefficient);
			}
			
			return polynomial;
		}
		
		private List<string> SplitPreservingOperators(string expression)
		{
			var parts = new List<string>();
			int start = 0;
			
			for (int i = 1; i < expression.Length; i++)
			{
				if (expression[i] == '+' || expression[i] == '-')
				{
					parts.Add(expression.Substring(start, i - start));
					start = i;
				}
			}
			
			// Add the last part
			if (start < expression.Length)
				parts.Add(expression.Substring(start));
			
			return parts;
		}
		
		private Term ParseTerm(string term)
		{
			// Clean up the term
			term = term.Trim();
			
			// Determine sign
			double sign = 1;
			if (term.StartsWith("+"))
			{
				term = term.Substring(1);
			}
			else if (term.StartsWith("-"))
			{
				sign = -1;
				term = term.Substring(1);
			}
			
			// Check what type of term this is
			if (!term.Contains("X"))
			{
				// Constant term
				double coeff = string.IsNullOrEmpty(term) ? 1 : double.Parse(term);
				return new Term(sign * coeff, 0);
			}
			else if (term.Contains("X^"))
			{
				// X with explicit power
				var parts = term.Split('^');
				if (parts.Length != 2)
					throw new ArgumentException($"Invalid power format in term: {term}");
				
				string coeffPart = parts[0];
				string powerPart = parts[1];
				
				// Remove X from coefficient part
				coeffPart = coeffPart.Replace("X", "").Replace("*", "");
				
				double coeff = string.IsNullOrEmpty(coeffPart) ? 1 : double.Parse(coeffPart);
				int power = int.Parse(powerPart);
				
				return new Term(sign * coeff, power);
			}
			else
			{
				// X with implicit power 1
				string coeffPart = term.Replace("X", "").Replace("*", "");
				double coeff = string.IsNullOrEmpty(coeffPart) ? 1 : double.Parse(coeffPart);
				
				return new Term(sign * coeff, 1);
			}
		}

		private bool TryParseStandardFormat(string side, Polynomial polynomial)
		{
			string remaining = side;
			bool foundAnyMatch = false;
			
			// Process patterns in order of specificity to avoid conflicts
			foreach (string pattern in StandardPatterns)
			{
				var matches = Regex.Matches(remaining, pattern);
				
				foreach (Match match in matches)
				{
					try
					{
						foundAnyMatch = true;
						var term = ExtractStandardTerm(match, pattern);
						polynomial.AddTerm(term.Power, term.Coefficient);
						// Remove the matched text to prevent double-matching
						int index = remaining.IndexOf(match.Value);
						if (index >= 0)
						{
							remaining = remaining.Remove(index, match.Value.Length);
						}
					}
					catch (Exception ex)
					{
						throw new ArgumentException($"Error parsing term '{match.Value}': {ex.Message}");
					}
				}
			}

			// Check if we consumed the entire input (allowing for +/- signs)
			remaining = remaining.Replace("+", "").Replace("-", "").Replace(" ", "");
			bool consumedAll = string.IsNullOrWhiteSpace(remaining);
			
			if (foundAnyMatch && !consumedAll)
			{
				throw new ArgumentException($"Unrecognized parts in equation: '{remaining}'. Please check the format.");
			}
			
			return foundAnyMatch && consumedAll;
		}

		private Polynomial ParseFreeForm(string side)
		{
			var polynomial = new Polynomial();
			
			// Normalize the input (convert to uppercase X)
			side = NormalizeFreeFormInput(side);
			string remaining = side;
			
			// Process patterns in order of specificity
			foreach (string pattern in FreeFormPatterns)
			{
				var matches = Regex.Matches(remaining, pattern);
				
				foreach (Match match in matches)
				{
					try
					{
						var term = ExtractFreeFormTerm(match, pattern);
						polynomial.AddTerm(term.Power, term.Coefficient);
						// Remove the matched text to prevent double-matching
						int index = remaining.IndexOf(match.Value);
						if (index >= 0)
						{
							remaining = remaining.Remove(index, match.Value.Length);
						}
					}
					catch (Exception ex)
					{
						throw new ArgumentException($"Error parsing term '{match.Value}' in free-form expression: {ex.Message}");
					}
				}
			}
			
			// Check for unparsed content
			string remainingContent = remaining.Replace(" ", "").Replace("+", "").Replace("-", "");
			if (!string.IsNullOrWhiteSpace(remainingContent))
			{
				throw new ArgumentException($"Unable to parse parts of expression: '{remainingContent}' in '{side}'. Please verify the format.");
			}

			return polynomial;
		}

		private string NormalizeFreeFormInput(string input)
		{
			input = input.Replace('x', 'X');
			
			if (!input.StartsWith("+") && !input.StartsWith("-"))
				input = "+" + input;
			
			input = Regex.Replace(input, @"([+-])", " $1");
			input = Regex.Replace(input, @"\s+", " ").Trim();
			
			return input;
		}

		private Term ExtractStandardTerm(Match match, string pattern)
		{
			if (pattern.Contains(@"X\s*\^\s*(\d+)") && pattern.Contains(@"(\d*\.?\d*)"))
			{
				string sign = match.Groups[1].Value;
				string coeffStr = match.Groups[2].Value;
				string powerStr = match.Groups[3].Value;
				
				double coeff = ParseCoefficient(sign + coeffStr);
				int power = ParsePower(powerStr);
				
				return new Term(coeff, power);
			}
			else if (pattern.Contains(@"X\s*\^\s*(\d+)"))
			{
				string sign = match.Groups[1].Value;
				string powerStr = match.Groups[2].Value;
				
				double coeff = string.IsNullOrEmpty(sign) || sign == "+" ? 1 : -1;
				int power = ParsePower(powerStr);
				
				return new Term(coeff, power);
			}
			else if (pattern.Contains(@"\*?\s*X\b"))
			{
				string sign = match.Groups[1].Value;
				string coeffStr = match.Groups[2].Value;
				
				double coeff = ParseCoefficient(sign + coeffStr);
				
				return new Term(coeff, 1);
			}
			else if (pattern.Contains(@"X\b"))
			{
				string sign = match.Groups[1].Value;
				double coeff = string.IsNullOrEmpty(sign) || sign == "+" ? 1 : -1;
				
				return new Term(coeff, 1);
			}
			else
			{
				string sign = match.Groups[1].Value;
				string coeffStr = match.Groups[2].Value;
				
				double coeff = ParseCoefficient(sign + coeffStr);
				
				return new Term(coeff, 0);
			}
		}

		private Term ExtractFreeFormTerm(Match match, string pattern)
		{
			if (pattern.Contains("X^"))
			{
				if (pattern.Contains(@"(\d*\.?\d*)X"))
				{
					string sign = match.Groups[1].Value;
					string coeffStr = match.Groups[2].Value;
					string powerStr = match.Groups[3].Value;
					
					double coeff = ParseCoefficient(sign + coeffStr);
					int power = ParsePower(powerStr);
					
					return new Term(coeff, power);
				}
				else
				{
					string sign = match.Groups[1].Value;
					string powerStr = match.Groups[2].Value;
					
					double coeff = string.IsNullOrEmpty(sign) || sign == "+" ? 1 : -1;
					int power = ParsePower(powerStr);
					
					return new Term(coeff, power);
				}
			}
			else if (pattern.Contains("X"))
			{
				if (pattern.Contains(@"(\d*\.?\d*)X"))
				{
					string sign = match.Groups[1].Value;
					string coeffStr = match.Groups[2].Value;
					
					double coeff = ParseCoefficient(sign + coeffStr);
					
					return new Term(coeff, 1);
				}
				else
				{
					string sign = match.Groups[1].Value;
					double coeff = string.IsNullOrEmpty(sign) || sign == "+" ? 1 : -1;
					
					return new Term(coeff, 1);
				}
			}
			else
			{
				string sign = match.Groups[1].Value;
				string coeffStr = match.Groups[2].Value;
				
				double coeff = ParseCoefficient(sign + coeffStr);
				
				return new Term(coeff, 0);
			}
		}

		private double ParseCoefficient(string coeffStr)
		{
			if (string.IsNullOrEmpty(coeffStr) || coeffStr == "+")
				return 1;
			if (coeffStr == "-")
				return -1;
			
			coeffStr = coeffStr.Trim();
			if (coeffStr.EndsWith("*"))
				coeffStr = coeffStr.TrimEnd('*');
			
			if (double.TryParse(coeffStr, out double result))
			{
				if (double.IsInfinity(result) || double.IsNaN(result))
					throw new ArgumentException($"Invalid coefficient value: {coeffStr}");
				
				return result;
			}
			
			throw new ArgumentException($"Unable to parse coefficient: '{coeffStr}'. Expected a number.");
		}

		private int ParsePower(string powerStr)
		{
			if (string.IsNullOrEmpty(powerStr))
				throw new ArgumentException("Missing power value after ^");
			
			if (!int.TryParse(powerStr, out int power))
				throw new ArgumentException($"Invalid power: '{powerStr}'. Powers must be integers.");
			
			if (power < 0)
				throw new ArgumentException($"Negative powers not allowed: {power}");
			
			if (power > 100)
				throw new ArgumentException($"Power too large: {power}. Maximum allowed power is 100.");
			
			return power;
		}
	}
}
