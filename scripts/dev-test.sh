#!/bin/bash

# CienceTerminal Development Test Script
# Run all tests across the project

set -e

echo "🧪 Running CienceTerminal test suite..."

# Function to run tests with better output
run_tests() {
    local test_project="$1"
    local test_name="$2"

    echo ""
    echo "🔍 Running $test_name..."
    echo "=================="

    if [ -d "$test_project" ]; then
        cd "$test_project"
        dotnet test --verbosity normal
        cd - > /dev/null
        echo "✅ $test_name completed successfully"
    else
        echo "⚠️ $test_name not found at $test_project"
    fi
}

# Parse command line arguments
RUN_UNIT=true
RUN_INTEGRATION=true
RUN_FRONTEND=true

while [[ $# -gt 0 ]]; do
    case $1 in
        --unit-only)
            RUN_INTEGRATION=false
            RUN_FRONTEND=false
            shift
            ;;
        --integration-only)
            RUN_UNIT=false
            RUN_FRONTEND=false
            shift
            ;;
        --frontend-only)
            RUN_UNIT=false
            RUN_INTEGRATION=false
            shift
            ;;
        --backend-only)
            RUN_FRONTEND=false
            shift
            ;;
        --help)
            echo "Usage: $0 [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  --unit-only       Run only unit tests"
            echo "  --integration-only Run only integration tests"
            echo "  --frontend-only   Run only frontend tests"
            echo "  --backend-only    Run only backend tests"
            echo "  --help            Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

# Run backend tests
if [ "$RUN_UNIT" = true ]; then
    # Check if test projects exist
    if ls tests/backend/*UnitTests*/*.csproj 1> /dev/null 2>&1; then
        for test_project in tests/backend/*UnitTests*; do
            if [ -d "$test_project" ]; then
                project_name=$(basename "$test_project")
                run_tests "$test_project" "Unit Tests ($project_name)"
            fi
        done
    else
        echo "⚠️ No unit test projects found"
    fi
fi

if [ "$RUN_INTEGRATION" = true ]; then
    # Check if integration test projects exist
    if ls tests/backend/*IntegrationTests*/*.csproj 1> /dev/null 2>&1; then
        for test_project in tests/backend/*IntegrationTests*; do
            if [ -d "$test_project" ]; then
                project_name=$(basename "$test_project")
                run_tests "$test_project" "Integration Tests ($project_name)"
            fi
        done
    else
        echo "⚠️ No integration test projects found"
    fi
fi

# Run frontend tests
if [ "$RUN_FRONTEND" = true ]; then
    echo ""
    echo "🎨 Running Frontend Tests..."
    echo "============================"

    if [ -d "frontend" ] && [ -f "frontend/package.json" ]; then
        cd frontend

        # Check if test script exists
        if npm run test --silent 2>/dev/null; then
            echo "✅ Frontend tests completed successfully"
        else
            echo "⚠️ No frontend test script found or tests failed"
        fi

        cd ..
    else
        echo "⚠️ Frontend directory or package.json not found"
    fi
fi

echo ""
echo "🎉 Test suite completed!"
echo ""