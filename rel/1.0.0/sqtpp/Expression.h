/**
** @file
** @author Ralf Seidel
** @brief Declaration of the expression parser/evaluator.
**
** © 2004-2006 by Heinrich und Seidel GbR Wuppertal.
*/
#ifndef SQTPP_EXPRESSION_H
#define SQTPP_EXPRESSION_H
#if _MSC_VER > 10
#pragma once
#endif

#include "Token.h"

namespace sqtpp {

class MacroSet;
class TokenExpression;
class TokenExpressions;

class Expression
{
public:
	/**
	** @brief the type of a conditional expression.
	*/
	enum Type 
	{
		/// Undefined / Initial value
		TYPE_UNDEFINED,
		/// An integer type (long long int).
		TYPE_INTEGER,
		// An identifier.
		TYPE_IDENTIFIER
		// An floating point type (sqtpp will use a double).
		//TYPE_FLOAT
	};

	/**
	** @brief The value of an expression.
	*/
	class Value
	{
	private:
		/// The type of the value.
		Expression::Type m_type;

		/// Zero.
		static const Value m_zero;

		/// The integer value.
		long long int m_integer;

		/// Identifier value.
		wstring       m_identifier;
	public:
		Value();
		Value( const Value&   that );
		Value( long long integer );
		Value( Expression::Type, const wstring& value );

		static const Value& getZero() throw()             { return m_zero; }
		Expression::Type    getType() const throw()       { return m_type; }
		long long int       getInteger() const throw()    { return m_integer; }
		const wstring&      getIdentifier() const throw() { return m_identifier; }

		void                evaluate( const Value& value,  Token opToken );
		void                evaluate( const Value& lValue, const Value& rValue, Token opToken );
		void                evaluateDefined( const Value& value, const MacroSet* pMacros );
	};

private:
	enum   NodeType;
	struct Node;

	/// The root node of the expression tree.
	Node* m_pRoot;

	/// Once evaluated the expression value.
	Value m_value;

	/// Evaluate the expression tree.
	const Value& evaluateNode( Node* ) const;

public:
	// Default constructor.
	Expression();
	// Initialising constructor.
	Expression( const TokenExpressions& expressions );
	// Destructor.
	~Expression();

	// Get the evaluated value.
	const Value& getValue() const;

	// Build up the expression tree 
	void build( const TokenExpressions& expressions );

	// Evaluate the expression tree.
	const Value& evaluate( const MacroSet* pMacros = NULL );
};


} // namespace


#endif // SQTPP_EXPRESSION_H
