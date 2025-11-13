# Computorv1 Comprehensive Test Cases

This document contains systematic test cases for all implemented features of Computorv1.

## Mandatory Tests (Subject Examples)

### Test 1: Quadratic with Real Solutions
```bash
./computor "5 * X^0 + 4 * X^1 - 9.3 * X^2 = 1 * X^0"
```
**Expected Output:**
```
Reduced form: 4 * X^0 + 4 * X^1 - 9.3 * X^2 = 0
Polynomial degree: 2
Discriminant is strictly positive, the two solutions are:
0.905239
-0.475131
```

### Test 2: Linear Equation
```bash
./computor "5 * X^0 + 4 * X^1 = 4 * X^0"
```
**Expected Output:**
```
Reduced form: 1 * X^0 + 4 * X^1 = 0
Polynomial degree: 1
The solution is:
-1/4
```

### Test 3: Degree > 2 (Unsolvable)
```bash
./computor "8 * X^0 - 6 * X^1 + 0 * X^2 - 5.6 * X^3 = 3 * X^0"
```
**Expected Output:**
```
Reduced form: 5 * X^0 - 6 * X^1 + 0 * X^2 - 5.6 * X^3 = 0
Polynomial degree: 3
The polynomial degree is strictly greater than 2, I can't solve.
```

### Test 4: Infinite Solutions
```bash
./computor "6 * X^0 = 6 * X^0"
```
**Expected Output:**
```
Reduced form: 0 * X^0 = 0
Any real number is a solution.
```

### Test 5: No Solution
```bash
./computor "10 * X^0 = 15 * X^0"
```
**Expected Output:**
```
Reduced form: -5 * X^0 = 0
No solution.
```

### Test 6: Complex Solutions
```bash
./computor "1 * X^0 + 2 * X^1 + 5 * X^2 = 0"
```
**Expected Output:**
```
Reduced form: 1 * X^0 + 2 * X^1 + 5 * X^2 = 0
Polynomial degree: 2
Discriminant is strictly negative, the two complex solutions are:
-1/5 + 2i/5
-1/5 - 2i/5
```

## Free Form Entry Tests (Bonus)

### Test 7: Free Form Quadratic
```bash
./computor "5 + 4 * X + X^2 = X^2"
```
**Expected Output:**
```
Reduced form: 5 * X^0 + 4 * X^1 = 0
Polynomial degree: 1
The solution is:
-5/4
```

### Test 8: Free Form without Spaces
```bash
./computor "X^2-3X+2=0"
```
**Expected Output:**
```
Reduced form: 2 * X^0 - 3 * X^1 + 1 * X^2 = 0
Polynomial degree: 2
Discriminant is strictly positive, the two solutions are:
2
1
```

### Test 9: Mixed Format
```bash
./computor "2X + 3 = X^2"
```
**Expected Output:**
```
Reduced form: 3 * X^0 + 2 * X^1 - 1 * X^2 = 0
Polynomial degree: 2
[Complex or real solutions depending on discriminant]
```

## Error Handling Tests

### Test 10: Invalid Characters
```bash
./computor "5 @ X^2 = 3"
./computor "5 * Y^2 = 3"
./computor "5 * X^2 = 3#"
```
**Expected Output:**
```
Error: Invalid character '@' found
Error: Invalid character 'Y' found
Error: Invalid character '#' found
```

### Test 11: Syntax Errors
```bash
./computor "5 ** X^2 = 3"
./computor "= 3"
./computor "5 * X^2 ="
./computor "5 * X^ = 3"
./computor "5 * X^2.5 = 3"
```
**Expected Output:**
```
Error: Consecutive operators found on left side
Error: Left side of equation is empty
Error: Right side of equation is empty
Error: Missing power after ^
Error: Decimal powers not allowed: X^2.5
```

### Test 12: Multiple Equals Signs
```bash
./computor "5 * X^2 = 3 = 4"
./computor "5 = 3 = X^2"
```
**Expected Output:**
```
Error: Too many equals signs - equation must have exactly one '='
```

## Fraction Display Tests (Bonus)

### Test 13: Simple Fractions
```bash
./computor "2 * X^0 + 4 * X^1 = 0"
./computor "3 * X^0 + 6 * X^1 = 0"
./computor "1 * X^0 + 2 * X^1 = 0"
```
**Expected Output:**
```
The solution is: -1/2
The solution is: -1/2
The solution is: -1/2
```

### Test 14: Complex Fractions
```bash
./computor "5 * X^0 + 4 * X^1 + 20 * X^2 = 0"
```
**Expected Output:**
```
Discriminant is strictly negative, the two complex solutions are:
-1/10 + 3i/10
-1/10 - 3i/10
```

## Intermediate Steps Tests (Bonus)

### Test 15: Linear Steps
```bash
./computor "3 * X^1 + 6 * X^0 = 0"
```
**Expected Output should include:**
```
Solving steps:
Step 1: Identify this as a linear equation (ax + b = 0)
Step 2: We have 3 * X + (6) = 0
Step 3: Rearrange to isolate X: 3 * X = -6
Step 4: Divide both sides by 3: X = -6 / 3
Step 5: Simplify: X = -2
```

### Test 16: Quadratic Steps
```bash
./computor "1 * X^0 + 4 * X^1 + 4 * X^2 = 0"
```
**Expected Output should include discriminant calculation steps**

## Graphical Visualization Tests (Bonus)

### Test 17: Linear Graph
```bash
./computor --graph "2 * X^1 + 4 * X^0 = 0"
./computor -g "2X + 4 = 0"
```
**Expected: ASCII graph showing straight line crossing x-axis at x = -2**

### Test 18: Quadratic Graph
```bash
./computor --graph "X^2 - 4 = 0"
./computor -g "1 * X^2 + 0 * X^1 - 4 * X^0 = 0"
```
**Expected: ASCII graph showing parabola with roots at x = ±2**

### Test 19: Complex Quadratic Graph
```bash
./computor --graph "X^2 + 1 = 0"
```
**Expected: ASCII graph showing parabola that doesn't cross x-axis**

## Edge Cases

### Test 20: Zero Coefficients
```bash
./computor "0 * X^2 + 3 * X^1 + 0 * X^0 = 0"
./computor "0 * X^1 + 5 * X^0 = 0"
```

### Test 21: Very Small Numbers
```bash
./computor "0.000001 * X^1 + 0.000001 * X^0 = 0"
```

### Test 22: Large Numbers
```bash
./computor "1000000 * X^2 + 2000000 * X^1 + 1000000 = 0"
```

### Test 23: Negative Powers (Should Fail)
```bash
./computor "5 * X^-1 = 3"
```
**Expected: Error about negative powers**

### Test 24: High Powers (Should Fail)
```bash
./computor "5 * X^100 = 3"
```
**Expected: Error about power too large**

## Standard Input Tests

### Test 25: STDIN Input
```bash
./computor
# Then enter: X^2 - 4 = 0
```

### Test 26: STDIN with Graph
```bash
./computor --graph
# Then enter: X^2 - 4 = 0
```

## Perfect Square Discriminant

### Test 27: Zero Discriminant
```bash
./computor "1 * X^2 + 2 * X^1 + 1 * X^0 = 0"
./computor "4 * X^2 + 4 * X^1 + 1 * X^0 = 0"
```
**Expected: Single repeated solution**

## Real-World Examples

### Test 28: Physics Equations
```bash
./computor "16 * X^0 + 0 * X^1 - 4.9 * X^2 = 0"  # Free fall
./computor "X^2 + X - 6 = 0"                       # Factoring practice
./computor "2X^2 - 8X + 6 = 0"                     # Common quadratic
```

## Performance Tests

### Test 29: Precision Tests
```bash
./computor "X^2 - 2 = 0"  # Should give ±√2 ≈ ±1.414...
./computor "X^2 - 3 = 0"  # Should give ±√3 ≈ ±1.732...
```

## Combination Tests

### Test 30: All Features Combined
```bash
./computor --graph "0.5X^2 + 1.5X - 2 = 0"
```
**Expected:**
- Free form parsing
- Fraction display
- Intermediate steps
- Graphical representation
- Numerical verification

## Stress Tests

### Test 31: Very Long Equations
```bash
./computor "1*X^0 + 1*X^1 + 1*X^2 = 1*X^0 + 1*X^1 + 1*X^2"
```

### Test 32: Many Spaces
```bash
./computor "   5   *   X  ^  2   +   3   *  X  ^  1   =   0   "
```

## Expected Failures (Should Handle Gracefully)

### Test 33: Invalid Equations
```bash
./computor ""
./computor "   "
./computor "abc"
./computor "X^2.5 = 0"
./computor "X^-1 = 0"
```

---

## How to Run All Tests

Create a test script:
```bash
#!/bin/bash
# Save as run_tests.sh

echo "Running Computorv1 Test Suite..."
echo "=================================="

# Add each test case here with expected vs actual output comparison
# Example:
echo "Test 1: Basic quadratic"
./computor "X^2 - 4 = 0"
echo ""

echo "Test 2: With graphing"
./computor --graph "X^2 - 4 = 0"
echo ""

# Continue for all test cases...