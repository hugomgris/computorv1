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

		private int ft_Round(double value)
		{
			return (int)(value + 0.5);
		}
	}
}