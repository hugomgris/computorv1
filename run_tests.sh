#!/bin/bash

# Computorv1 Automated Test Suite
# Run with: chmod +x run_tests.sh && ./run_tests.sh

echo "Computorv1 Automated Test Suite"
echo "================# Test 14: Empty# Test 16: Multiple equals signs
run_test "Multiple equals signs" \
    'dotnet run "5 * X^2 = 3 = 4"' \
    "Error:" \
    true side
run_test "Empty left side" \
    'dotnet run "= 3"' \
    "Error:" \
    true============="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Test counters
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0

# Function to run a test
run_test() {
    local test_name="$1"
    local command="$2"
    local expected_pattern="$3"
    local should_fail="$4"
    
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    
    echo -e "${BLUE}Test $TOTAL_TESTS: $test_name${NC}"
    echo "Command: $command"
    
    # Run the command and capture output
    output=$(eval "$command" 2>&1)
    exit_code=$?
    
    # Check if test should fail
    if [ "$should_fail" = "true" ]; then
        if [ $exit_code -ne 0 ] || echo "$output" | grep -q "Error:"; then
            echo -e "${GREEN}✓ PASSED${NC} (Expected failure)"
            PASSED_TESTS=$((PASSED_TESTS + 1))
        else
            echo -e "${RED}✗ FAILED${NC} (Should have failed but didn't)"
            echo "Output: $output"
            FAILED_TESTS=$((FAILED_TESTS + 1))
        fi
    else
        # Check if expected pattern exists in output  
        # Use grep -F -- for literal string matching to handle patterns starting with -
        if echo "$output" | grep -F -- "$expected_pattern" >/dev/null 2>&1; then
            echo -e "${GREEN}✓ PASSED${NC}"
            PASSED_TESTS=$((PASSED_TESTS + 1))
        else
            echo -e "${RED}✗ FAILED${NC}"
            echo "Expected pattern: $expected_pattern"
            echo "Actual output:"
            echo "$output"
            FAILED_TESTS=$((FAILED_TESTS + 1))
        fi
    fi
    echo ""
}

# Function to run a visual test (requires manual verification)
run_visual_test() {
    local test_name="$1"
    local command="$2"
    
    echo -e "${YELLOW}Visual Test: $test_name${NC}"
    echo "Command: $command"
    echo "Please verify output manually:"
    echo "-----------------------------"
    eval "$command"
    echo "-----------------------------"
    echo ""
}

echo "Building project..."
dotnet build > /dev/null 2>&1
if [ $? -ne 0 ]; then
    echo -e "${RED}Build failed! Please fix compilation errors first.${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Build successful${NC}"
echo ""

echo "Running Mandatory Tests..."
echo "=============================="

# Test 1: Basic quadratic with real solutions
run_test "Basic quadratic (subject example)" \
    'dotnet run "5 * X^0 + 4 * X^1 - 9.3 * X^2 = 1 * X^0"' \
    "Polynomial degree: 2"

# Test 2: Linear equation
run_test "Linear equation (subject example)" \
    'dotnet run "5 * X^0 + 4 * X^1 = 4 * X^0"' \
    "Polynomial degree: 1"

# Test 3: Degree > 2
run_test "Unsolvable degree (subject example)" \
    'dotnet run "8 * X^0 - 6 * X^1 + 0 * X^2 - 5.6 * X^3 = 3 * X^0"' \
    "degree is strictly greater than 2"

# Test 4: Infinite solutions
run_test "Infinite solutions (subject example)" \
    'dotnet run "6 * X^0 = 6 * X^0"' \
    "Infinite solutions"

# Test 5: No solution
run_test "No solution (subject example)" \
    'dotnet run "10 * X^0 = 15 * X^0"' \
    "No solution"

# Test 6: Complex solutions
run_test "Complex solutions (subject example)" \
    'dotnet run "1 * X^0 + 2 * X^1 + 5 * X^2 = 0"' \
    "Discriminant is strictly negative"

echo "Running Bonus Feature Tests..."
echo "================================="

# Test 7: Free form entry
run_test "Free form entry" \
    'dotnet run "5 + 4 * X + X^2 = X^2"' \
    "Polynomial degree: 1"

# Test 8: Free form without spaces
run_test "Free form without spaces" \
    'dotnet run "X^2-3X+2=0"' \
    "Polynomial degree: 2"

# Test 9: Fraction display
run_test "Fraction display" \
    'dotnet run "2 * X^0 + 4 * X^1 = 0"' \
    "-1/2"

# Test 10: Complex fractions
run_test "Complex fraction display" \
    'dotnet run "1 * X^0 + 2 * X^1 + 5 * X^2 = 0"' \
    "i/"

echo "Running Error Handling Tests..."
echo "=================================="

# Test 11: Invalid characters
run_test "Invalid character @" \
    'dotnet run "5 @ X^2 = 3"' \
    "Invalid character" \
    true

# Test 12: Invalid character Y
run_test "Invalid variable Y" \
    'dotnet run "5 * Y^2 = 3"' \
    "Invalid character" \
    true

# Test 13: Consecutive operators
run_test "Consecutive operators" \
    'dotnet run "5 ** X^2 = 3"' \
    "Error:" \
    true

# Test 14: Empty sides
run_test "Empty left side" \
    'dotnet run "= 3"' \
    "empty\|Error:" \
    true

# Test 15: Decimal powers
run_test "Decimal powers" \
    'dotnet run "5 * X^2.5 = 3"' \
    "Error:" \
    true

# Test 16: Multiple equals
run_test "Multiple equals signs" \
    'dotnet run "5 * X^2 = 3 = 4"' \
    "Too many equals\|Error:" \
    true

echo "Running Edge Case Tests..."
echo "============================="

# Test 17: Zero coefficients
run_test "Zero coefficients" \
    'dotnet run "0 * X^2 + 3 * X^1 + 0 * X^0 = 0"' \
    "Polynomial degree: 1"

# Test 18: Perfect square discriminant
run_test "Perfect square discriminant" \
    'dotnet run "1 * X^2 + 2 * X^1 + 1 * X^0 = 0"' \
    "Discriminant is zero"

# Test 19: Very simple linear
run_test "Simple linear" \
    'dotnet run "X = 5"' \
    "Polynomial degree: 1"

echo "Running Visual Tests (Manual Verification)..."
echo "================================================="

# Only run visual tests if --visual flag is provided
if [ "$1" = "--visual" ]; then
    run_visual_test "Linear graph" \
        'dotnet run --graph "2 * X^1 + 4 * X^0 = 0"'
    
    run_visual_test "Quadratic graph" \
        'dotnet run --graph "X^2 - 4 = 0"'
    
    run_visual_test "Complex quadratic graph" \
        'dotnet run --graph "X^2 + 1 = 0"'
else
    echo -e "${YELLOW}Skipping visual tests. Run with --visual flag to see graphs.${NC}"
fi

echo ""
echo "Test Results Summary"
echo "======================="
echo -e "Total Tests: ${BLUE}$TOTAL_TESTS${NC}"
echo -e "Passed: ${GREEN}$PASSED_TESTS${NC}"
echo -e "Failed: ${RED}$FAILED_TESTS${NC}"

if [ $FAILED_TESTS -eq 0 ]; then
    echo -e "${GREEN}All tests passed!${NC}"
    exit 0
else
    echo -e "${RED}Some tests failed.${NC}"
    exit 1
fi
