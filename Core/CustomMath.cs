namespace Computorv1.Core
{
	/// <summary>
	/// Replication of Math.abs and Math.Sqrt because subject reasons
	/// </summary>
	public static class CustomMath
	{
		public static double ft_Abs(double x)
		{
			return x < 0 ? -x : x;
		}

		public static double ft_Sqrt(double nb)
		{
			if (nb < 0)
			{
				nb = -nb;
			}
			
			if (nb == 0 || nb == 1)
				return nb;
			
			double guess = nb / 2.0;
			double prevGuess = 0;
			
			while (ft_Abs(guess - prevGuess) > 1e-10)
			{
				prevGuess = guess;
				guess = (guess + nb / guess) / 2.0;
			}
			
			return guess;
		}

		public static int ft_Round(double value)
		{
			return (int)(value + 0.5);
		}

		/// <summary>
		/// Creates a simple text-based graph of a polynomial function
		/// </summary>
		public static void DrawPolynomialGraph(Polynomial polynomial, double xMin = -10, double xMax = 10, int width = 60, int height = 20)
		{
			Console.WriteLine("\nGraphical representation:");
			Console.WriteLine(new string('=', width + 10));

			double[] yValues = new double[width];
			double xStep = (xMax - xMin) / (width - 1);
			
			double yMin = double.MaxValue;
			double yMax = double.MinValue;
			
			for (int i = 0; i < width; i++)
			{
				double x = xMin + i * xStep;
				double y = EvaluatePolynomial(polynomial, x);
				yValues[i] = y;
				
				if (!double.IsInfinity(y) && !double.IsNaN(y))
				{
					yMin = y < yMin ? y : yMin;
					yMax = y > yMax ? y : yMax;
				}
			}
			
			double yRange = yMax - yMin;
			yMin -= yRange * 0.1;
			yMax += yRange * 0.1;
			yRange = yMax - yMin;
			
			for (int row = 0; row < height; row++)
			{
				double currentY = yMax - (row * yRange) / (height - 1);
				
				Console.Write($"{currentY,6:F1} |");
				
				for (int col = 0; col < width; col++)
				{
					double x = xMin + col * xStep;
					double y = yValues[col];

					bool showCurve = ft_Abs(y - currentY) < (yRange / (height - 1)) / 2;

					bool isXAxis = ft_Abs(currentY) < (yRange / (height - 1)) / 2;

					bool isYAxis = ft_Abs(x) < xStep / 2;
					
					if (showCurve)
					{
						Console.Write("*");
					}
					else if (isXAxis && isYAxis)
					{
						Console.Write("+");
					}
					else if (isXAxis)
					{
						Console.Write("-");
					}
					else if (isYAxis)
					{
						Console.Write("|");
					}
					else
					{
						Console.Write(" ");
					}
				}
				Console.WriteLine();
			}
			
			Console.Write("       ");
			for (int i = 0; i < width; i += 10)
			{
				double x = xMin + i * xStep;
				Console.Write($"{x,10:F1}");
			}
			Console.WriteLine();
			Console.WriteLine(new string('=', width + 10));
		}
		
		/// <summary>
		/// Evaluates a polynomial at a given x value
		/// </summary>
		public static double EvaluatePolynomial(Polynomial polynomial, double x)
		{
			double result = 0;
			
			foreach (var term in polynomial.GetTerms())
			{
				int power = term.Key;
				double coefficient = term.Value;
				
				double termValue = coefficient;
				
				if (power > 0)
				{
					double xPower = 1;
					for (int i = 0; i < power; i++)
					{
						xPower *= x;
					}
					termValue *= xPower;
				}
				
				result += termValue;
			}
			
			return result;
		}

		/// <summary>
		/// Finds approximate roots using simple numerical method
		/// </summary>
		public static List<double> FindApproximateRoots(Polynomial polynomial, double xMin = -10, double xMax = 10, double step = 0.1)
		{
			var roots = new List<double>();
			
			double prevY = EvaluatePolynomial(polynomial, xMin);
			
			for (double x = xMin + step; x <= xMax; x += step)
			{
				double currentY = EvaluatePolynomial(polynomial, x);
				
				if ((prevY > 0 && currentY < 0) || (prevY < 0 && currentY > 0))
				{
					double root = FindRootByBisection(polynomial, x - step, x, 1e-6);
					
					bool isNewRoot = true;
					foreach (var existingRoot in roots)
					{
						if (ft_Abs(root - existingRoot) < 0.01)
						{
							isNewRoot = false;
							break;
						}
					}
					
					if (isNewRoot)
					{
						roots.Add(root);
					}
				}
				
				prevY = currentY;
			}
			
			return roots;
		}
		
		private static double FindRootByBisection(Polynomial polynomial, double a, double b, double tolerance)
		{
			int maxIterations = 50;
			
			for (int i = 0; i < maxIterations; i++)
			{
				double mid = (a + b) / 2.0;
				double midValue = EvaluatePolynomial(polynomial, mid);
				
				if (ft_Abs(midValue) < tolerance || ft_Abs(b - a) < tolerance)
				{
					return mid;
				}
				
				double aValue = EvaluatePolynomial(polynomial, a);
				
				if ((aValue > 0 && midValue > 0) || (aValue < 0 && midValue < 0))
				{
					a = mid;
				}
				else
				{
					b = mid;
				}
			}
			
			return (a + b) / 2.0;
		}
	}
}