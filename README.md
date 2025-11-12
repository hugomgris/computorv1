# <h1 align="center">COMPUTORV1</h1>

<p align="center">
   <b>A comprehensive polynomial equation solver implementing mathematical fundamentals from parsing to advanced visualization. This project demonstrates core computational mathematics concepts including equation parsing, polynomial evaluation, custom mathematical functions, and graphical representation.</b><br>
</p>

<p align="center">
    <img alt="C#" src="https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white" />
    <img alt=".NET" src="https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white" />
    <img alt="Math" src="https://img.shields.io/badge/Mathematics-FF6B6B?style=for-the-badge&logo=mathworks&logoColor=white" />
    <img alt="Polynomial Algebra" src="https://img.shields.io/badge/Polynomial%20Algebra-4ECDC4?style=for-the-badge" />
</p>
<p align="center">
    <img alt="GitHub code size in bytes" src="https://img.shields.io/github/languages/code-size/hugomgris/computorv1?color=lightblue" />
    <img alt="Feature Count" src="https://img.shields.io/badge/Features-7-brightgreen" />
    <img alt="Standard" src="https://img.shields.io/badge/.NET-8.0-blue" />
</p>

## Table of Contents

1. [Project Overview](#project-overview)
2. [Mathematical Foundations](#mathematical-foundations)
3. [Feature Breakdown](#feature-breakdown)
4. [Implementation Architecture](#implementation-architecture)
5. [Building and Running](#building-and-running)
6. [Theoretical Deep Dive](#theoretical-deep-dive)

---

### **Core Features Implemented**

- **Polynomial Parsing**: Both mandatory and free-form equation formats
- **Linear Equations**: First-degree polynomial solving with exact solutions
- **Quadratic Equations**: Second-degree solving with discriminant analysis
- **Complex Numbers**: Support for imaginary solutions when discriminant < 0
- **Fraction Display**: Irreducible fraction representation of rational solutions
- **Step-by-Step Solving**: Detailed intermediate calculation display
- **Graphical Visualization**: ASCII art polynomial plotting and root finding

### **Learning Objectives**

By studying this implementation, you will understand:
- How to parse mathematical expressions using regular expressions
- The mathematical theory behind polynomial equation solving
- Implementation of custom mathematical functions (sqrt, fraction reduction)
- Complex number arithmetic and representation
- Error handling in mathematical software
- ASCII-based graphical visualization techniques

---

## Mathematical Foundations

### **Polynomial Equation Theory**

The project implements solving for polynomial equations of the form:
```
aₙxⁿ + aₙ₋₁xⁿ⁻¹ + ... + a₁x + a₀ = 0
```

#### **Degree Classification**
```
Degree 0: Constant equation (a₀ = 0)
Degree 1: Linear equation (ax + b = 0)  
Degree 2: Quadratic equation (ax² + bx + c = 0)
Degree > 2: Higher-order polynomials (unsolvable by this program)
```

#### **Solution Methods**
```
Linear: x = -b/a (where a ≠ 0)
Quadratic: x = (-b ± √(b² - 4ac)) / 2a
```

#### **Discriminant Analysis**
```
Δ = b² - 4ac
Δ > 0: Two distinct real solutions
Δ = 0: One repeated real solution  
Δ < 0: Two complex conjugate solutions
```

#### **Complex Number Representation**
```
For Δ < 0: x = -b/(2a) ± i√|Δ|/(2a)
Standard form: a + bi where i² = -1
```

---

## Feature Breakdown

### **Feature 1: Polynomial Parsing**

**Objective**: Parse mathematical expressions from string input to internal representation.

**Supported Formats**:
- **Mandatory**: `5 * X^0 + 4 * X^1 - 9.3 * X^2 = 1 * X^0`
- **Free Form**: `5 + 4X - 9.3X^2 = 1` or `X^2 - 3X + 2 = 0`

**Algorithm**: Regex-based parsing with multiple pattern matching.

```csharp
public class PolynomialParser
{
    private static readonly string[] StandardPatterns = {
        @"([+-]?)\s*(\d*\.?\d*)\s*\*\s*X\s*\^\s*(\d+)",  // 5 * X^2
        @"([+-]?)\s*X\s*\^\s*(\d+)",                      // X^2
        @"([+-]?)\s*(\d*\.?\d*)\s*\*?\s*X\b",            // 5*X or 5X
        @"([+-]?)\s*X\b",                                 // X
        @"([+-]?)\s*(\d+\.?\d*)"                          // Constants
    };

    public static Polynomial ParseSide(string side)
    {
        var terms = new Dictionary<int, double>();
        
        foreach (var pattern in StandardPatterns)
        {
            var matches = Regex.Matches(side, pattern);
            foreach (Match match in matches)
            {
                var term = ExtractTerm(match);
                if (terms.ContainsKey(term.Power))
                    terms[term.Power] += term.Coefficient;
                else
                    terms[term.Power] = term.Coefficient;
            }
        }
        
        return new Polynomial(terms);
    }
}
```

**Pattern Recognition Examples**:
```
"5 * X^2" → Coefficient: 5, Power: 2
"X^2" → Coefficient: 1, Power: 2
"-3X" → Coefficient: -3, Power: 1
"7" → Coefficient: 7, Power: 0
```

---

### **Feature 2: Linear Equation Solving**

**Objective**: Solve first-degree polynomials of the form `ax + b = 0`.

**Algorithm**: Direct algebraic manipulation with fraction reduction.

```csharp
public static SolutionResult SolveDegree1(Polynomial polynomial)
{
    double a = polynomial.GetCoefficient(1);
    double b = polynomial.GetCoefficient(0);
    
    if (CustomMath.ft_Abs(a) < EPSILON)
    {
        return HandleDegenerate(b);
    }
    
    double solution = -b / a;
    
    return new SolutionResult
    {
        Type = SolutionType.Linear,
        Solutions = new List<string> { FormatAsFraction(solution) }
    };
}

private static string FormatAsFraction(double value)
{
    if (CustomMath.ft_Abs(value - CustomMath.ft_Round(value)) < EPSILON)
        return CustomMath.ft_Round(value).ToString();
    
    return FindBestFraction(value);
}
```

**Edge Cases Handled**:
- **Infinite Solutions**: `0x + 0 = 0` → "Any real number is a solution"
- **No Solution**: `0x + c = 0` where c ≠ 0 → "No solution"

---

### **Feature 3: Quadratic Equation Solving**

**Objective**: Solve second-degree polynomials using the quadratic formula.

**Algorithm**: Discriminant-based classification with custom square root.

```csharp
public static SolutionResult SolveDegree2(Polynomial polynomial)
{
    double a = polynomial.GetCoefficient(2);
    double b = polynomial.GetCoefficient(1);
    double c = polynomial.GetCoefficient(0);
    
    double discriminant = b * b - 4 * a * c;
    
    if (CustomMath.ft_Abs(discriminant) < EPSILON)
    {
        // Single solution
        double solution = -b / (2 * a);
        return new SolutionResult
        {
            Type = SolutionType.SingleReal,
            Discriminant = discriminant,
            Solutions = new List<string> { FormatAsFraction(solution) }
        };
    }
    else if (discriminant > 0)
    {
        // Two real solutions
        double sqrtDisc = CustomMath.ft_Sqrt(discriminant);
        double sol1 = (-b + sqrtDisc) / (2 * a);
        double sol2 = (-b - sqrtDisc) / (2 * a);
        
        return new SolutionResult
        {
            Type = SolutionType.TwoReal,
            Discriminant = discriminant,
            Solutions = new List<string> { 
                FormatAsFraction(sol1), 
                FormatAsFraction(sol2) 
            }
        };
    }
    else
    {
        // Complex solutions
        double realPart = -b / (2 * a);
        double imaginaryPart = CustomMath.ft_Sqrt(-discriminant) / (2 * a);
        
        return new SolutionResult
        {
            Type = SolutionType.Complex,
            Discriminant = discriminant,
            Solutions = new List<string> {
                FormatComplexFraction(realPart, imaginaryPart),
                FormatComplexFraction(realPart, -imaginaryPart)
            }
        };
    }
}
```

**Mathematical Verification**:
- For `x² - 4 = 0`: Solutions are `x = ±2`
- For `x² + 1 = 0`: Solutions are `x = ±i`
- For `x² - 2x + 1 = 0`: Solution is `x = 1` (repeated root)

---

### **Feature 4: Custom Mathematical Functions**

**Objective**: Implement mathematical operations without using built-in Math library.

**Key Functions Implemented**:

```csharp
public class CustomMath
{
    // Newton's method for square root
    public static double ft_Sqrt(double number)
    {
        if (number < 0) throw new ArgumentException("Square root of negative number");
        if (number == 0) return 0;
        
        double guess = number / 2.0;
        const double precision = 0.000001;
        
        while (true)
        {
            double newGuess = (guess + number / guess) / 2.0;
            if (ft_Abs(guess - newGuess) < precision)
                return newGuess;
            guess = newGuess;
        }
    }
    
    // Custom absolute value
    public static double ft_Abs(double x)
    {
        return x < 0 ? -x : x;
    }
    
    // Custom rounding
    public static int ft_Round(double x)
    {
        return x >= 0 ? (int)(x + 0.5) : (int)(x - 0.5);
    }
}
```

**Newton's Method Convergence**:
For √n, iterating `x_{k+1} = (x_k + n/x_k)/2` converges quadratically.

---

### **Feature 5: Fraction Display**

**Objective**: Display rational solutions as irreducible fractions.

**Algorithm**: Greatest Common Divisor (GCD) reduction using Euclidean algorithm.

```csharp
private static string FindBestFraction(double value)
{
    const double tolerance = 0.000001;
    const int maxDenominator = 10000;
    
    bool isNegative = value < 0;
    if (isNegative) value = -value;
    
    for (int denominator = 1; denominator <= maxDenominator; denominator++)
    {
        int numerator = (int)(value * denominator + 0.5);
        
        if (ft_Abs(value - (double)numerator / denominator) < tolerance)
        {
            int gcd = CalculateGCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;
            
            if (denominator == 1)
                return isNegative ? $"-{numerator}" : numerator.ToString();
            else
                return isNegative ? $"-{numerator}/{denominator}" : $"{numerator}/{denominator}";
        }
    }
    
    return (isNegative ? -value : value).ToString("F6");
}

private static int CalculateGCD(int a, int b)
{
    while (b != 0)
    {
        int temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}
```

**Examples**:
- `0.5` → `1/2`
- `-0.25` → `-1/4`
- `1.333...` → `4/3`

---

### **Feature 6: Step-by-Step Solution Display**

**Objective**: Show intermediate calculation steps for educational value.

**Implementation**: Structured step tracking with mathematical explanations.

```csharp
public void DisplayIntermediateSteps(SolutionResult result)
{
    if (result.Type == SolutionType.Linear)
    {
        Console.WriteLine("Solving steps:");
        Console.WriteLine("Step 1: Identify this as a linear equation (ax + b = 0)");
        Console.WriteLine($"Step 2: We have {GetLinearForm()} = 0");
        Console.WriteLine("Step 3: Rearrange to isolate X");
        Console.WriteLine("Step 4: Divide both sides by coefficient of X");
        Console.WriteLine($"Step 5: Solution: X = {result.Solutions[0]}");
    }
    else if (result.Type == SolutionType.TwoReal)
    {
        Console.WriteLine("Solving steps:");
        Console.WriteLine("Step 1: Apply quadratic formula: x = (-b ± √(b² - 4ac)) / 2a");
        Console.WriteLine($"Step 2: Calculate discriminant: b² - 4ac = {result.Discriminant}");
        Console.WriteLine($"Step 3: √{result.Discriminant} = {CustomMath.ft_Sqrt(result.Discriminant)}");
        Console.WriteLine("Step 4: Apply ± to get two solutions");
    }
}
```

**Educational Value**: Demonstrates mathematical reasoning process step-by-step.

---

### **Feature 7: Graphical Visualization**

**Objective**: Provide ASCII-based polynomial plotting and root visualization.

**Algorithm**: Discrete sampling with coordinate mapping and root approximation.

```csharp
public static void DrawPolynomialGraph(Polynomial polynomial, double xMin, double xMax)
{
    const int width = 80;
    const int height = 25;
    char[,] grid = new char[height, width];
    
    // Initialize grid
    for (int i = 0; i < height; i++)
        for (int j = 0; j < width; j++)
            grid[i, j] = ' ';
    
    // Draw axes
    int centerY = height / 2;
    int centerX = width / 2;
    
    // Draw polynomial curve
    for (int screenX = 0; screenX < width; screenX++)
    {
        double realX = xMin + (xMax - xMin) * screenX / width;
        double y = polynomial.EvaluateAt(realX);
        
        int screenY = (int)(centerY - y * height / (xMax - xMin));
        
        if (screenY >= 0 && screenY < height)
        {
            grid[screenY, screenX] = '*';
        }
    }
    
    // Mark approximate roots
    var roots = FindApproximateRoots(polynomial, xMin, xMax);
    foreach (var root in roots)
    {
        int rootX = (int)((root - xMin) / (xMax - xMin) * width);
        if (rootX >= 0 && rootX < width)
            grid[centerY, rootX] = 'X';
    }
    
    // Display grid
    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
            Console.Write(grid[i, j]);
        Console.WriteLine();
    }
}

private static List<double> FindApproximateRoots(Polynomial poly, double min, double max)
{
    var roots = new List<double>();
    const double step = 0.1;
    
    for (double x = min; x < max - step; x += step)
    {
        double y1 = poly.EvaluateAt(x);
        double y2 = poly.EvaluateAt(x + step);
        
        if (y1 * y2 < 0) // Sign change indicates root
        {
            double root = FindRootByBisection(poly, x, x + step);
            roots.Add(root);
        }
    }
    
    return roots;
}
```

**Graph Features**:
- Polynomial curve plotting with `*` characters
- Root marking with `X` characters  
- Coordinate axes display
- Automatic scaling based on domain

---

## Implementation Architecture

### **Core Design Principles**

1. **Separation of Concerns**: Input/Output handling separate from mathematical logic
2. **Custom Mathematics**: All math operations implemented from scratch
3. **Error Resilience**: Comprehensive input validation and error handling
4. **Educational Value**: Clear code structure for learning purposes
5. **Extensibility**: Modular design allows easy feature addition

### **Class Structure**

```csharp
namespace Computorv1
{
    // Core mathematical classes
    namespace Core
    {
        public class Polynomial { }           // Polynomial representation
        public class Term { }                 // Individual polynomial terms
        public class PolynomialParser { }     // String to polynomial conversion
        public class PolynomialSolver { }     // Main solving algorithms
        public class SolutionResult { }       // Result encapsulation
        public class CustomMath { }           // Mathematical utilities
    }
    
    // Input/Output handling
    namespace IO
    {
        public class InputHandler { }         // Input validation
        public class OutputHandler { }        // Result formatting and display
    }
    
    // Main program
    public class Program { }                  // Entry point and CLI handling
}
```

### **Data Flow Architecture**

```
Command Line Input → InputHandler → PolynomialParser → Polynomial
                                         ↓
ASCII Graph ← OutputHandler ← SolutionResult ← PolynomialSolver
```

### **File Organization**

```
computorv1/
├── Program.cs                    # Entry point and CLI interface
├── computorv1.csproj            # .NET project configuration
├── README.md                    # This documentation
├── run_tests.sh                 # Automated testing script
├── test_cases.md                # Comprehensive test documentation
├── TESTING_CHECKLIST.md         # Manual testing checklist
├── header_banner.txt            # ASCII art banner
├── Core/                        # Mathematical logic
│   ├── Polynomial.cs            # Polynomial representation
│   ├── Term.cs                  # Term data structure
│   ├── PolynomialParser.cs      # Expression parsing
│   ├── PolynomialSolver.cs      # Solution algorithms
│   ├── SolutionResult.cs        # Result encapsulation
│   └── CustomMath.cs            # Mathematical utilities
├── IO/                          # Input/Output handling
│   ├── InputHandler.cs          # Input validation
│   └── OutputHandler.cs         # Result formatting
├── bin/                         # Compiled binaries
└── obj/                         # Build artifacts
```

---

## Building and Running

### **Prerequisites**

- **.NET SDK**: Version 8.0 or higher
- **Operating System**: Linux, macOS, or Windows
- **Shell**: Compatible with bash/zsh for test scripts

### **Building the Project**

```bash
# Clone the repository
git clone https://github.com/hugomgris/computorv1.git
cd computorv1

# Restore dependencies and build
dotnet restore
dotnet build

# Create executable (optional)
dotnet publish -c Release -o publish/
```

### **Running Examples**

```bash
# Basic usage with equation as argument
dotnet run "5 * X^0 + 4 * X^1 - 9.3 * X^2 = 1 * X^0"

# Free form input
dotnet run "X^2 - 4 = 0"

# With graphical output
dotnet run --graph "X^2 - 4 = 0"

# Interactive mode (read from STDIN)
dotnet run
# Then enter: X^2 + 2X + 1 = 0

# Graph flag with interactive mode  
dotnet run --graph
# Then enter equation
```

### **Testing Framework**

```bash
# Run automated test suite
chmod +x run_tests.sh
./run_tests.sh

# Run with visual verification
./run_tests.sh --visual

# Run individual test categories
dotnet run "X^2 - 4 = 0"                    # Basic quadratic
dotnet run "invalid equation"                # Error handling
dotnet run --graph "2X + 4 = 0"            # Graphical output
```

### **Expected Output Examples**

#### Linear Equation:
```
$ dotnet run "3X + 6 = 0"

 ██████╗ ██████╗ ███╗   ███╗██████╗ ██╗   ██╗████████╗ ██████╗ ██████╗ ██╗   ██╗██╗
██╔════╝██╔═══██╗████╗ ████║██╔══██╗██║   ██║╚══██╔══╝██╔═══██╗██╔══██╗██║   ██║╚═╝
██║     ██║   ██║██╔████╔██║██████╔╝██║   ██║   ██║   ██║   ██║██████╔╝██║   ██║   
██║     ██║   ██║██║╚██╔╝██║██╔═══╝ ██║   ██║   ██║   ██║   ██║██╔══██╗╚██╗ ██╔╝██╗
╚██████╗╚██████╔╝██║ ╚═╝ ██║██║     ╚██████╔╝   ██║   ╚██████╔╝██║  ██║ ╚████╔╝ ╚═╝
 ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚═╝      ╚═════╝    ╚═╝    ╚═════╝ ╚═╝  ╚═╝  ╚═══╝     

Reduced form: 6 * X^0 + 3 * X^1 = 0
Polynomial degree: 1
The solution is:
-2
```

#### Quadratic Equation:
```
$ dotnet run --graph "X^2 - 4 = 0"

Reduced form: -4 * X^0 + 0 * X^1 + 1 * X^2 = 0
Polynomial degree: 2
Discriminant is strictly positive, the two solutions are:
2
-2

                                       *
                                     * | *
                                   *   |   *
                                 *     |     *
                               *       |       *
                             *         |         *
                           *           |           *
                         *             |             *
                       *               |               *
                     *                 |                 *
                   *                   |                   *
                 *                     |                     *
               *                       |                       *
             *                         |                         *
X-----------*---------------------------+---------------------------*-----------X
           *                           |                           *
         *                             |                             *
       *                               |                               *
     *                                 |                                 *
   *                                   |                                   *
 *                                     |                                     *
                                       |
                                       |
                                       |
                                       |
```

#### Complex Solutions:
```
$ dotnet run "X^2 + 1 = 0"

Reduced form: 1 * X^0 + 0 * X^1 + 1 * X^2 = 0
Polynomial degree: 2
Discriminant is strictly negative, the two complex solutions are:
0 + i
0 - i
```

---

## Theoretical Deep Dive

### **Computational Complexity Analysis**

| Operation | Algorithm | Time Complexity | Space Complexity |
|-----------|-----------|----------------|------------------|
| Parsing | Regex Matching | O(n × m) | O(n) |
| Linear Solving | Direct Formula | O(1) | O(1) |
| Quadratic Solving | Quadratic Formula | O(1) | O(1) |
| Custom Square Root | Newton's Method | O(log p) | O(1) |
| Fraction Reduction | Euclidean GCD | O(log min(a,b)) | O(1) |
| Graph Plotting | Discrete Sampling | O(w × h) | O(w × h) |
| Root Finding | Bisection Method | O(log(1/ε)) | O(1) |

*Where: n = input length, m = pattern length, p = precision digits, w = width, h = height, ε = tolerance*

### **Numerical Analysis Considerations**

#### **Precision and Stability**
```csharp
const double EPSILON = 1e-9;  // Floating-point comparison tolerance

// Stable discriminant calculation avoiding catastrophic cancellation
double discriminant = b * b - 4.0 * a * c;

// Newton's method convergence criteria
while (ft_Abs(guess - newGuess) < precision) { /* ... */ }
```

#### **Edge Cases Handled**
- **Near-zero coefficients**: Proper degree determination
- **Floating-point precision**: Epsilon-based equality testing  
- **Overflow prevention**: Reasonable input bounds checking
- **Invalid operations**: Square root of negative numbers in real context

### **Mathematical Correctness Verification**

#### **Quadratic Formula Derivation**
```
Given: ax² + bx + c = 0
Complete the square: a(x² + (b/a)x) + c = 0
Add and subtract (b/2a)²: a(x + b/2a)² - b²/4a + c = 0
Solve for x: x = -b/2a ± √(b² - 4ac)/2a
```

#### **Newton's Method Convergence**
For f(x) = x² - n, finding √n:
```
x_{k+1} = x_k - f(x_k)/f'(x_k) = x_k - (x_k² - n)/(2x_k) = (x_k + n/x_k)/2
```
Converges quadratically for any x₀ > 0.

#### **Complex Number Arithmetic**
```
For z = a + bi and w = c + di:
Addition: (a + c) + (b + d)i
Multiplication: (ac - bd) + (ad + bc)i  
Magnitude: |z| = √(a² + b²)
```

### **Algorithm Optimizations Implemented**

1. **Regex Compilation**: Pre-compiled patterns for parsing efficiency
2. **Early Termination**: Degree checking before expensive calculations  
3. **Memory Efficiency**: Minimal object allocation in hot paths
4. **Precision Control**: Adaptive tolerance based on input magnitude
5. **Caching**: Coefficient lookups cached in Dictionary structure

### **Error Analysis**

#### **Sources of Numerical Error**
- **Floating-point representation**: Limited precision in binary representation
- **Arithmetic operations**: Rounding errors accumulate through calculations
- **Algorithmic error**: Newton's method iteration error decreases quadratically
- **Input parsing**: String-to-double conversion precision limits

#### **Error Mitigation Strategies**
```csharp
// Relative error tolerance
bool IsZero(double value, double relativeTolerance = 1e-9)
{
    return ft_Abs(value) < ft_Max(relativeTolerance, ft_Abs(value) * relativeTolerance);
}

// Discriminant calculation with explicit promotion
double discriminant = (double)b * b - 4.0 * (double)a * (double)c;
```

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
