namespace Computorv1.IO
{
	/// <summary>
	/// Handles input validation and preprocessing
	/// </summary>
	public class InputHandler
	{
		public string retakeRawInput(string input)
		{
			string equation;
			Console.WriteLine();
			Console.Write("Enter equation:\n> ");
			equation = Console.ReadLine() ?? string.Empty;
			return equation;
		}
		
		public string ProcessInput(string input)
		{
			return input.Trim();
		}

		public (bool IsValid, string ErrorMessage) ValidateInput(string input)
		{
			// 1. Check for null/empty
			if (string.IsNullOrWhiteSpace(input))
				return (false, "No equation provided");

			// 2. Check for exactly one equals sign
			int equalsCount = input.Count(c => c == '=');
			if (equalsCount == 0)
				return (false, "Missing equals sign (=)");
			if (equalsCount > 1)
				return (false, "Too many equals signs - equation must have exactly one '='");

			// 3. Check vocabulary (invalid characters)
			var invalidChar = GetFirstInvalidCharacter(input);
			if (invalidChar != null)
				return (false, $"Invalid character '{invalidChar}' found");

			// 4. Check basic syntax structure
			var syntaxCheck = CheckSyntax(input);
			if (!syntaxCheck.IsValid)
				return (false, syntaxCheck.ErrorMessage);

			return (true, string.Empty);
		}

		public bool CheckVocabulary(string input)
		{
			return input.All(c => IsValidCharacter(c));
		}

		private char? GetFirstInvalidCharacter(string input)
		{
			foreach (char c in input)
			{
				if (!IsValidCharacter(c))
					return c;
			}
			return null;
		}

		private (bool IsValid, string ErrorMessage) CheckSyntax(string input)
		{
			var parts = input.Split('=');
			string leftSide = parts[0].Trim();
			string rightSide = parts[1].Trim();

			if (string.IsNullOrWhiteSpace(leftSide))
				return (false, "Left side of equation is empty");
			if (string.IsNullOrWhiteSpace(rightSide))
				return (false, "Right side of equation is empty");

			var leftCheck = CheckSideSyntax(leftSide, "left");
			if (!leftCheck.IsValid) return leftCheck;

			var rightCheck = CheckSideSyntax(rightSide, "right");
			if (!rightCheck.IsValid) return rightCheck;

			return (true, string.Empty);
		}

		private (bool IsValid, string ErrorMessage) CheckSideSyntax(string side, string sideName)
		{
			string clean = side.Replace(" ", "");

			if (HasConsecutiveOperators(clean))
				return (false, $"Consecutive operators found on {sideName} side");

			if (clean.StartsWith("*") || clean.StartsWith("/") || clean.StartsWith("^"))
				return (false, $"Equation {sideName} side starts with invalid operator");

			if (clean.EndsWith("*") || clean.EndsWith("/") || clean.EndsWith("^") || clean.EndsWith("+"))
				return (false, $"Equation {sideName} side ends with incomplete operator");

			if (clean.Contains("X^") || clean.Contains("x^"))
			{
				var powerCheck = CheckPowerFormat(clean);
				if (!powerCheck.IsValid)
					return (false, $"{powerCheck.ErrorMessage} on {sideName} side");
			}

			if (!HasValidDecimals(clean))
				return (false, $"Invalid decimal format on {sideName} side");

			return (true, string.Empty);
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

		private bool HasValidDecimals(string input)
		{
			var parts = input.Split(new char[] { '+', '-', '*', '/', '^', 'x', 'X' }, 
								StringSplitOptions.RemoveEmptyEntries);
			
			foreach (var part in parts)
			{
				if (part.Contains('.'))
				{
					if (!System.Text.RegularExpressions.Regex.IsMatch(part, @"^\d*\.\d+$") && 
						!System.Text.RegularExpressions.Regex.IsMatch(part, @"^\d+\.\d*$"))
						return false;
				}
			}
			return true;
		}

		private (bool IsValid, string ErrorMessage) CheckPowerFormat(string input)
		{
			var explicitPowerPattern = @"[xX]\^([^+\-*/=\s]+)";
			var matches = System.Text.RegularExpressions.Regex.Matches(input, explicitPowerPattern);
			
			foreach (System.Text.RegularExpressions.Match match in matches)
			{
				string powerStr = match.Groups[1].Value;
				
				if (string.IsNullOrEmpty(powerStr))
					return (false, "Missing power after ^");
				
				if (powerStr.Contains('.'))
					return (false, $"Decimal powers not allowed: X^{powerStr}");
				
				if (!powerStr.All(char.IsDigit))
					return (false, $"Invalid power format: X^{powerStr}");
				
				if (!int.TryParse(powerStr, out int power) || power > 50)
					return (false, $"Invalid power: X^{powerStr}");
			}
			
			return (true, string.Empty);
		}

		private bool IsValidCharacter(char c)
		{
			return char.IsDigit(c) ||
				char.IsWhiteSpace(c) ||
				c == '.' ||
				c == 'x' || c == 'X' ||
				"+-*/^=()".Contains(c);
		}
	}
}