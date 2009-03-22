#ifndef SQTPP_EXPRESSION_TEST_H
#define SQTPP_EXPRESSION_TEST_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Test.h"

namespace sqtpp {
namespace test {

/**
** @brief Tests for the class sqtpp.Expression
*/
class ExpressionTest : public Test
{
public:
	ExpressionTest();
	~ExpressionTest() throw() {}

	/// Execute all tests.
	static void run();


	/// Evaluate trivial expressions.
	void evalutateTrivialsTest();

	/// Evaluate trivial expressions with an unary +/-.
	void unaryPlusMinusTest();

	/// Test all unary operators.
	void unaryOperatorsTest();

	/// Test all comparison operators.
	void compareOperatorsTest();

	/// Test all boolean operators (&&, ||, ^^).
	void booleanOperatorsTest();

	/// Test all boolean operators (&, |, ^).
	void bitwiseOperatorsTest();

	/// Evaluate trivial expressions with an unary not (!).
	void logicalNotTest();

	/// Evaluate trivial expressions with an unary not/complement (~).
	void complementTest();

	/// Evaluate trivial expressions with an unary not (!).
	void simpleCalculationTest();

	/// Evaluate trivial expressions with an unary not (!).
	void shiftingTest();

	/// Test operator precedence.
	void precedenceTest();

	/// Test parenthesis
	void parenthesisTest();
};



} // namespace test
} // namespace sqtpp



#endif // SQTPP_EXPRESSION_TEST_H
