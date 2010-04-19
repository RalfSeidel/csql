#include "stdafx.h"
#include "Context.h"
#include "Token.h"
#include "Macro.h"
#include "Expression.h"
#include "TestBase.h"

namespace sqtpp {
namespace test {

[TestClass]
public ref class ExpressionTest : public TestBase
{
public:
	/**
	** @brief Evaluate trivial expressions.
	*/
	[TestMethod]
	void evalutateTrivialsTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		// Decimals
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		// Hexadecimals
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0x0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0x1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0xF" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 15 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0x10" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 16 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0xFF" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 255 );

		// Octals
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"00" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"01" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"010" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 8 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"077" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 63 );
	}

	/**
	** @brief Evaluate trivial expressions.
	*/
	[TestMethod]
	void unaryPlusMinusTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"10" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -10 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0x10" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -16 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"010" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -8 );


		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"10" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == +10 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0x10" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == +16 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"010" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == +8 );
	}

	/**
	** @brief Test evaluation of all unary operators.
	*/
	[TestMethod]
	void unaryOperatorsTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		// Unary minus
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -1 );

		// Unary plus
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == +1 );

		// Not
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_NOT, CTX_DEFAULT, L"!" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_NOT, CTX_DEFAULT, L"!" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_NOT, CTX_DEFAULT, L"!" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0xFFFFFFFFFFFFFFFF" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		// complement
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_BIT_NOT, CTX_DEFAULT, L"~" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_BIT_NOT, CTX_DEFAULT, L"~" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -2 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_BIT_NOT, CTX_DEFAULT, L"~" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0xFFFFFFFFFFFFFFFF" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		// defined
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_DEFINED, CTX_DEFAULT, L"defined" ) );
		expressions.push_back( TokenExpression( TOK_IDENTIFIER, CTX_DEFAULT, L"MACRO" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );
	}

	/**
	** @brief Test all comparison operators.
	*/
	[TestMethod]
	void compareOperatorsTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		// ==
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_EQ, CTX_DEFAULT, L"==" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_EQ, CTX_DEFAULT, L"==" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_EQ, CTX_DEFAULT, L"==" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		// !=
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_NE, CTX_DEFAULT, L"!=" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_NE, CTX_DEFAULT, L"!=" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_NE, CTX_DEFAULT, L"!=" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		// <=
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LE, CTX_DEFAULT, L"<=" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LE, CTX_DEFAULT, L"<=" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_LE, CTX_DEFAULT, L"<=" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );


		// >=
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_GE, CTX_DEFAULT, L">=" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_GE, CTX_DEFAULT, L">=" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_GE, CTX_DEFAULT, L">=" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		// <
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LT, CTX_DEFAULT, L"<" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LT, CTX_DEFAULT, L"<" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_LT, CTX_DEFAULT, L"<" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		// >
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_GT, CTX_DEFAULT, L">" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_GT, CTX_DEFAULT, L">" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_GT, CTX_DEFAULT, L">" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0);
	}

	/**
	** @brief Test all boolean operators (&&, ||, ^^).
	*/
	[TestMethod]
	void booleanOperatorsTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		// &&
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_AND, CTX_DEFAULT, L"&&" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_AND, CTX_DEFAULT, L"&&" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_AND, CTX_DEFAULT, L"&&" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		// ||
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_OR, CTX_DEFAULT, L"||" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_OR, CTX_DEFAULT, L"||" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_OR, CTX_DEFAULT, L"||" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_OR, CTX_DEFAULT, L"||" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		// ^^
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_XOR, CTX_DEFAULT, L"^^" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_XOR, CTX_DEFAULT, L"^^" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_XOR, CTX_DEFAULT, L"^^" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_XOR, CTX_DEFAULT, L"^^" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );
	}

	/**
	** @brief Test all boolean operators (&, |, ^).
	*/
	[TestMethod]
	void bitwiseOperatorsTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		// &
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_AND, CTX_DEFAULT, L"&" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_AND, CTX_DEFAULT, L"&" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"3" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_AND, CTX_DEFAULT, L"&" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 2 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_AND, CTX_DEFAULT, L"&" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_AND, CTX_DEFAULT, L"&" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		// |
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_OR, CTX_DEFAULT, L"|" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 3 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_OR, CTX_DEFAULT, L"|" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_OR, CTX_DEFAULT, L"|" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_OR, CTX_DEFAULT, L"|" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		// ^
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_XOR, CTX_DEFAULT, L"^" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 3 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_XOR, CTX_DEFAULT, L"^" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_XOR, CTX_DEFAULT, L"^" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		expressions.push_back( TokenExpression( TOK_OP_BIT_XOR, CTX_DEFAULT, L"^" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );
	}


	/**
	** @brief Evaluate trivial expressions with an unary not (!).
	*/
	[TestMethod]
	void logicalNotTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_NOT, CTX_DEFAULT, L"!" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_NOT, CTX_DEFAULT, L"!" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_NOT, CTX_DEFAULT, L"!" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_NOT, CTX_DEFAULT, L"!" ) );
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_NOT, CTX_DEFAULT, L"!" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_LOGICAL_NOT, CTX_DEFAULT, L"!" ) );
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );
	}

	/**
	** @brief Evaluate trivial expressions with an unary not/complement (~).
	*/
	[TestMethod]
	void complementTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_BIT_NOT, CTX_DEFAULT, L"~" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"0" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_BIT_NOT, CTX_DEFAULT, L"~" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -2 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_BIT_NOT, CTX_DEFAULT, L"~" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -3 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_BIT_NOT, CTX_DEFAULT, L"~" ) );
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_OP_BIT_NOT, CTX_DEFAULT, L"~" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == -2 );
	}


	/**
	** @brief Evaluate simple calcuation expressions.
	*/
	[TestMethod]
	void simpleCalculationTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 2 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 4 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_DIVIDE, CTX_DEFAULT, L"/" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"10" ) );
		expressions.push_back( TokenExpression( TOK_OP_DIVIDE, CTX_DEFAULT, L"/" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"5" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 2 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"10" ) );
		expressions.push_back( TokenExpression( TOK_OP_DIVIDE, CTX_DEFAULT, L"/" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"3" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 3 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_MODULUS, CTX_DEFAULT, L"%" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );
	}


	/**
	** @brief Evaluate bit shifting operations.
	*/
	[TestMethod]
	void shiftingTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LSHIFT, CTX_DEFAULT, L"<<" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 2 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_LSHIFT, CTX_DEFAULT, L"<<" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"10" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1024 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1024" ) );
		expressions.push_back( TokenExpression( TOK_OP_RSHIFT, CTX_DEFAULT, L">>" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 512 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1024" ) );
		expressions.push_back( TokenExpression( TOK_OP_RSHIFT, CTX_DEFAULT, L">>" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"10" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1024" ) );
		expressions.push_back( TokenExpression( TOK_OP_RSHIFT, CTX_DEFAULT, L">>" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"11" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );
	}


	/**
	** @brief Test operator precedence.
	*/
	[TestMethod]
	void precedenceTest()
	{
		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		// 2 + 2 * 2
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 6 );

		// 2 - 2 / 2
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_DIVIDE, CTX_DEFAULT, L"/" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		// 1 + 4 * 3 / 2 + 3 (10)
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"4" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"3" ) );
		expressions.push_back( TokenExpression( TOK_OP_DIVIDE, CTX_DEFAULT, L"/" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"3" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 10 );

		// 2 * 2 == 16 / 4 (1)
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_EQ, CTX_DEFAULT, L"==" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"16" ) );
		expressions.push_back( TokenExpression( TOK_OP_DIVIDE, CTX_DEFAULT, L"/" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"4" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );

		// 2 * 2 == 1 + 1 * 2 (0)
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_EQ, CTX_DEFAULT, L"==" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );
	}

	/**
	** @brief Test parenthesis.
	*/
	[TestMethod]
	void parenthesisTest()
	{

		TokenExpressions  expressions;
		Expression        evaluator;
		Expression::Value value;

		// (2 + 2) * 2
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_LEFT_PARENTHESIS, CTX_DEFAULT, L"(" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_RIGHT_PARENTHESIS, CTX_DEFAULT, L")" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 8 );

		// (2 - 2) / 2
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_LEFT_PARENTHESIS, CTX_DEFAULT, L"(" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_MINUS, CTX_DEFAULT, L"-" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_RIGHT_PARENTHESIS, CTX_DEFAULT, L")" ) );
		expressions.push_back( TokenExpression( TOK_OP_DIVIDE, CTX_DEFAULT, L"/" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		// (1 + 4) * 3 / (2 + 3) (3)
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_LEFT_PARENTHESIS, CTX_DEFAULT, L"(" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"4" ) );
		expressions.push_back( TokenExpression( TOK_RIGHT_PARENTHESIS, CTX_DEFAULT, L")" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"3" ) );
		expressions.push_back( TokenExpression( TOK_OP_DIVIDE, CTX_DEFAULT, L"/" ) );
		expressions.push_back( TokenExpression( TOK_LEFT_PARENTHESIS, CTX_DEFAULT, L"(" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"3" ) );
		expressions.push_back( TokenExpression( TOK_RIGHT_PARENTHESIS, CTX_DEFAULT, L")" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 3 );

		// 2 * (2 == 16) / 4 (0)
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_LEFT_PARENTHESIS, CTX_DEFAULT, L"(" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_EQ, CTX_DEFAULT, L"==" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"16" ) );
		expressions.push_back( TokenExpression( TOK_RIGHT_PARENTHESIS, CTX_DEFAULT, L")" ) );
		expressions.push_back( TokenExpression( TOK_OP_DIVIDE, CTX_DEFAULT, L"/" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"4" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 0 );

		// 2 * 2 == (1 + 1) * 2 (1)
		expressions.clear();
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		expressions.push_back( TokenExpression( TOK_OP_EQ, CTX_DEFAULT, L"==" ) );
		expressions.push_back( TokenExpression( TOK_LEFT_PARENTHESIS, CTX_DEFAULT, L"(" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_OP_PLUS, CTX_DEFAULT, L"+" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"1" ) );
		expressions.push_back( TokenExpression( TOK_RIGHT_PARENTHESIS, CTX_DEFAULT, L")" ) );
		expressions.push_back( TokenExpression( TOK_OP_MULTIPLY, CTX_DEFAULT, L"*" ) );
		expressions.push_back( TokenExpression( TOK_NUMBER, CTX_DEFAULT, L"2" ) );
		evaluator.build( expressions );
		value = evaluator.evaluate();
		Assert::IsTrue( value.getInteger() == 1 );
	}

}; // class ExpressionTest

} // namespace test
} // namespace sqtpp
