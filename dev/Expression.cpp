#include "StdAfx.h"
#include <math.h>
#include "Token.h"
#include "Macro.h"
#include "Exceptions.h"
#include "Error.h"
#include "Expression.h"

namespace sqtpp {

// --------------------------------------------------------------------
// Expression::NodeType
// --------------------------------------------------------------------

/**
** @brief Type of a node in the expression tree.
*/
enum  Expression::NodeType
{
	/// Undefined / Initial value.
	NTP_UNDEFINED,
	/// A node holding an operator,
	NTP_OPERATOR,
	/// A raw value node.
	NTP_VALUE
};



// --------------------------------------------------------------------
// Expression::Node
// --------------------------------------------------------------------

/**
** @brief A node in the expression tree.
*/
struct Expression::Node
{
	NodeType m_type;
	Node*    m_pLeft;
	Node*    m_pRight;
	Node*    m_pParent;
	Token    m_token;
	Value    m_value;

	Node();
	Node( Node* pParent );
	~Node();

	const Value& evaluate( const MacroSet* pMacros );
};


Expression::Node::Node()
{
	memset( this, 0, sizeof( *this ) );
}

Expression::Node::~Node()
{
	delete m_pLeft;
	delete m_pRight;
}


/**
** @brief Evaluate the expression sub tree.
*/
const Expression::Value& Expression::Node::evaluate( const MacroSet* pMacros )
{
	const TokenInfo& tokenInfo = TokenInfo::getTokenInfo( m_token );
	

	switch ( this->m_type ) {
		case Expression::NTP_OPERATOR:

			if ( this->m_pLeft != NULL && this->m_pRight != NULL ) {
				assert( tokenInfo.isBinaryOperator() );
				const Value& lValue = this->m_pLeft->evaluate( pMacros );
				const Value& rValue = this->m_pRight->evaluate( pMacros );
				this->m_value.evaluate( lValue, rValue, m_token );
			} else if ( this->m_pRight != NULL ) {
				assert( tokenInfo.isUnaryOperator() || m_token == TOK_RIGHT_PARENTHESIS );
				const Value& rValue = this->m_pRight->evaluate( pMacros );

				if ( this->m_token == TOK_OP_DEFINED ) {
					this->m_value.evaluateDefined( rValue, pMacros );
				} else {
					this->m_value.evaluate( rValue, m_token );
				}
			} else {
				// Operator without operands!
				assert( false );
			}
			break;
		case Expression::NTP_VALUE:
			break;
		default:
			throw UnexpectedSwitchError();
	}
	return m_value;
}




// --------------------------------------------------------------------
// Expression::Value
// --------------------------------------------------------------------

/**
** @brief Zero constant value.
*/
const Expression::Value Expression::Value::m_zero;

/**
** @brief Default constructor.
*/
Expression::Value::Value()
{
	m_type    = Expression::TYPE_UNDEFINED;
	m_integer = 0L;
}

/**
** @brief Copy constructor.
*/
Expression::Value::Value( const Value& that )
{
	m_type       = that.m_type;
	m_integer    = that.m_integer;
	m_identifier = that.m_identifier;
}

/**
** @brief Initializing constructor.
*/
Expression::Value::Value( long long integer )
{
	m_type    = Expression::TYPE_INTEGER;
	m_integer = integer;
}


/**
** @brief Initializing constructor.
*/
Expression::Value::Value( Expression::Type type, const wstring& value )
{
	assert( type == Expression::TYPE_INTEGER || type == Expression::TYPE_IDENTIFIER );
	m_type    = type;
	m_integer = 0;

	if ( type == Expression::TYPE_IDENTIFIER ) {
		m_identifier = value;
	} else if ( value.length() > 0 ) {
		if ( value[0] == L'0' && value.length() > 1 ) {
			if ( value[1] == L'x' || value[1] == L'X'  ) {
				// Hexadecimal
				for ( size_t i = 2; i < value.length(); ++i ) {
					wchar_t ch    = value[i];
					int     digit = 0;

					if ( std::islower( ch, locale::classic() ) ) {
						ch = std::toupper( ch, locale::classic() );
					}
					assert( L'0' <= ch && ch <= L'9' || L'A' <= ch && ch <= L'F' );
					if ( L'0' <= ch && ch <= L'9' ) {
						digit = ch - L'0';
					} else if ( L'A' <= ch && ch <= L'F' ) {
						digit = ch - L'A' + 10;
					} else {
						throw RuntimeError( "Invalid hex digit" );
					}
					m_integer*= 0x10;
					m_integer+= digit;
				}
			} else {
				// Octal
				for ( size_t i = 1; i < value.length(); ++i ) {
					wchar_t ch    = value[i];
					int     digit = 0;

					assert( L'0' <= ch && ch <= L'7' );
					if ( L'0' <= ch && ch <= L'7' ) {
						digit = ch - L'0';
					} else {
						throw RuntimeError( "Invalid octal digit" );
					}
					m_integer*= 010;
					m_integer+= digit;
				}
			}
		} else {
			for ( size_t i = 0; i < value.length(); ++i ) {
				wchar_t ch    = value[i];
				int     digit = 0;

				assert( L'0' <= ch && ch <= L'9' );
				if ( L'0' <= ch && ch <= L'9' ) {
					digit = ch - L'0';
				} else {
					throw RuntimeError( "Invalid hex digit" );
				}
				m_integer*= 10;
				m_integer+= digit;
			}
		}
	}
}

/**
** @brief Evaluate binary operation.
**
** This methode will be called when evaluation expressions like
** @code
** 3 + 4
** @endcode
**
** or
**
** @code
** 3 | 4
** @endcode
** 
** @param lValue The left value.
** @param rValue The right value.
** @param opToken The operator token.
*/
void Expression::Value::evaluate( const Value& lValue, const Value& rValue, Token opToken )
{
	long long int lInteger = lValue.getInteger();
	long long int rInteger = rValue.getInteger();
	long long int result;

	const TokenInfo& tokenInfo = TokenInfo::getTokenInfo( opToken );
	if ( &lValue == &m_zero ) {
		assert( tokenInfo.isUnaryOperator() || opToken == TOK_RIGHT_PARENTHESIS );
	} else {
		assert( tokenInfo.isBinaryOperator() );
	}

	switch ( opToken ) {
	case TOK_OP_COMMA:
		result = rInteger;
		break;
	case TOK_OP_ASSIGN:
		assert( false );
		throw NotSupportedError();
		break;
	case TOK_OP_EQ:
		result = lInteger == rInteger;
		break;
	case TOK_OP_NE:
		result = lInteger != rInteger;
		break;
	case TOK_OP_LT:
		result = lInteger < rInteger;
		break;
	case TOK_OP_LE:
		result = lInteger <= rInteger;
		break;
	case TOK_OP_LSHIFT:
		result = lInteger << rInteger;
		break;
	case TOK_OP_GT:
		result = lInteger > rInteger;
		break;
	case TOK_OP_GE:
		result = lInteger >= rInteger;
		break;
	case TOK_OP_RSHIFT:
		result = lInteger >> rInteger;
		break;
	case TOK_OP_MINUS:
		result = lInteger - rInteger;
		break;
	case TOK_OP_UNARY_MINUS:
		result = -rInteger;
		break;
	case TOK_OP_PLUS:
		result = lInteger + rInteger;
		break;
	case TOK_OP_UNARY_PLUS:
		result = +rInteger;
		break;
	case TOK_OP_MULTIPLY:
		result = lInteger * rInteger;
		break;
	case TOK_OP_POWER:
		if ( lInteger == 0 || rInteger == 0 ) {
			result = 1;
		} else {
			// to be done.
			assert( false );
			throw NotSupportedError();
		} 
		break;
	case TOK_OP_DIVIDE:
		if ( rInteger == 0 ) {
			// Division by zero.
			throw error::C2124();
		}
		result = lInteger / rInteger;
		break;
	case TOK_OP_MODULUS:
		if ( rInteger == 0 ) {
			// Division by zero.
			throw error::C2124();
		}
		result = lInteger % rInteger;
		break;
	case TOK_OP_LOGICAL_NOT:
		result = !rInteger;
		break;
	case TOK_OP_LOGICAL_AND:
		result = lInteger && rInteger;
		break;
	case TOK_OP_LOGICAL_OR:
		result = lInteger || rInteger;
		break;
	case TOK_OP_LOGICAL_XOR:
		result = (lInteger != 0) ^ (rInteger != 0);
		break;
	case TOK_OP_BIT_AND:
		result = lInteger & rInteger;
		break;
	case TOK_OP_BIT_OR:
		result = lInteger | rInteger;
		break;
	case TOK_OP_BIT_XOR:
		result = lInteger ^ rInteger;
		break;
	case TOK_OP_BIT_NOT:
		result = ~rInteger;
		break;
	case TOK_RIGHT_PARENTHESIS:
		result = +rInteger;
		this->m_identifier = rValue.getIdentifier();
		break;
	default:
		assert( false );
		throw UnexpectedSwitchError();
	}

	this->m_type = rValue.getType();
	this->m_integer = result;
}


/**
** @brief Evaluate unary operation.
**
** The evaluation of an unary operations is handled in the binary
** operation evaluator. This methode is only implemented to make
** evaluation a bit clearer.
*/
void Expression::Value::evaluate( const Value& value, Token opToken )
{
	evaluate( m_zero, value, opToken );
}


/**
** @brief Evaluate the defined operator.
*/
void Expression::Value::evaluateDefined( const Value& value, const MacroSet* pMacros )
{
	if ( pMacros == NULL || value.getType() != Expression::TYPE_IDENTIFIER ) {
		*this = m_zero;
	} else {
		const wstring& identifier = value.getIdentifier();
		this->m_type    = Expression::TYPE_INTEGER;
		this->m_integer = pMacros->count( identifier ) != 0;
	}
}



// --------------------------------------------------------------------
// Expression
// --------------------------------------------------------------------
/**
** @brief Default contructor.
*/
Expression::Expression()
: m_pRoot( NULL )
{
}

/**
** @brief Initialising constructor. Immediatly parses and evaluates the given expression.
*/
Expression::Expression( const TokenExpressions& expressions )
: m_pRoot( NULL )
{
	build( expressions );
	evaluate();
}

Expression::~Expression()
{
	delete m_pRoot;
}


/**
** @brief Get the evaluated value.
*/
const Expression::Value& Expression::getValue() const 
{ 
	assert( m_value.getType() != Expression::TYPE_UNDEFINED );
	return m_value; 
}


/**
** @brief Build up the expression tree.
**
**          root: value 
**
**          root: unary operator
**                       right: value 
**
**          root: binary operator
**     left: value       right: value
**
*/
void Expression::build( const TokenExpressions& expressions )
{
	// Local functions
	struct InsertUnaryNode_ {
	// Insert a value or group
	// The value will be inserted at the most outer right operator node.
	void operator()( Node** ppNode, Node* pValue )
	{
		for (;;) {
			Node* pNode = *ppNode;
			if ( pNode == NULL ) {
				// The place has been found.
				*ppNode = pValue;
				if ( pValue->m_pParent != NULL ) {
					assert( pValue->m_pParent->m_pRight == pValue );
				}
				break;
			} else if ( pNode->m_type == NTP_OPERATOR ) {
				pValue->m_pParent = pNode;
				ppNode = &pNode->m_pRight;
			} else {
				// Right node already is a value. That means it has been populated in the 
				// previous step i.e. the next token expected is a (binary) operator.
				// Unexcpeted token found while parseing expression. Expected binary operator or end of expression.
				throw sqtpp::error::C4067B();
			}
		}
	}} insertUnaryNode;

	struct InsertBinaryNode_ {
	// Insert a operator.
	void operator()( Node** ppNode, Node* pOperator )
	{
		Token token                  = pOperator->m_token;
		const TokenInfo&  tokenInfo  = TokenInfo::getTokenInfo( token );
		const int         precedence = tokenInfo.getOperatorPrecedence();
		for ( ;; ) {
			Node* pNode = *ppNode;

			if ( pNode == NULL ) {
				// If iteration end at a null node the operator has to be unary.
				// Otherwise a value or group has to be inserted first.
				if ( !tokenInfo.isUnaryOperator() ) {
					// Invalid expression: missing left operand for operator {1}.
					throw sqtpp::error::C1017B( wstring() );
				}
				*ppNode = pOperator;
				if ( pOperator->m_pParent != NULL ) {
					assert( pOperator->m_pParent->m_pRight == NULL );
					pOperator->m_pParent->m_pRight = pOperator;
				}
				break;
			} else if ( pNode->m_type != NTP_OPERATOR ) {
				pNode->m_pParent   = pOperator;
				pOperator->m_pLeft = pNode;
				*ppNode = pOperator;
				break;
			} else {
				Token nodeToken           = pNode->m_token;
				const TokenInfo& nodeInfo = TokenInfo::getTokenInfo( nodeToken );
				const int        nodePred = nodeInfo.getOperatorPrecedence();
				if ( precedence >= nodePred ) {
					pNode->m_pParent   = pOperator;
					pOperator->m_pLeft = pNode;
					*ppNode            = pOperator;
					break;
				} else {
					ppNode = &pNode->m_pRight;
				}
			}
		}
	}} insertBinaryNode;


	// Methode body

	std::stack<Node*> nodeStack;

	delete m_pRoot;
	m_pRoot = NULL;
	try {
		bool bExpectOperator    = false;
		bool bIsDefinedOperator = false;

		if ( expressions.size() > 0 ) {
			// Check if this is a "#ifdef or #if defined()" expression
			// This check is only done to suppress the warning "found identifier in calculated expression"
			// Note that this check is not complete. It will find very simple cases only where the defined
			// operator is the first token in the expression list.
			const TokenExpression& tokenExpression = expressions[0];
			const Token            token           = tokenExpression.getToken();
			if ( token == TOK_OP_DEFINED ) {
				bIsDefinedOperator = true;
			}
		}

		for ( TokenExpressions::const_iterator  it = expressions.begin(); it != expressions.end(); ++it ) {
			const TokenExpression& tokenExpression = *it;
			const wstring&         tokenText       = tokenExpression.getText();
			Token                  token           = tokenExpression.getToken();
			const TokenInfo&       tokenInfo       = TokenInfo::getTokenInfo( token );
			bool   bIsOperator     = tokenInfo.isOperator();
			size_t nTextLength     = tokenText.length();
			Node*  pNode           = NULL;

			if ( bIsOperator ) {
				bool  bIsUnaryOperator  = tokenInfo.isUnaryOperator();
				bool  bIsBinaryOperator = tokenInfo.isBinaryOperator();

				bIsDefinedOperator = token == TOK_OP_DEFINED;

				if ( bExpectOperator ) {
					if ( bIsBinaryOperator ) {
						bExpectOperator = false;
					} else {
						// Unexcpeted token found while parseing expression. Expected binary operator or end of expression.
						throw error::C4067B();
					}
				} else {
					if ( token == TOK_OP_PLUS ) {
						token = TOK_OP_UNARY_PLUS;
						bIsUnaryOperator  = true;
						bIsBinaryOperator = false;
					} else if ( token == TOK_OP_MINUS ) {
						token = TOK_OP_UNARY_MINUS;
						bIsUnaryOperator  = true;
						bIsBinaryOperator = false;
					} else if ( !bIsUnaryOperator ) {
						// Invalid expression: missing left operand for operator {1}.
						throw error::C1017B( tokenExpression.getText() );
					} 
				}
				pNode = new Node();
				pNode->m_type      = NTP_OPERATOR;
				pNode->m_token     = token;
				if ( bIsBinaryOperator ) {
					insertBinaryNode( &m_pRoot, pNode );
				} else {
					insertUnaryNode( &m_pRoot, pNode );
				}
			} else {
				switch ( token ) {
				case TOK_SPACE:
				case TOK_BLOCK_COMMENT:
				case TOK_LINE_COMMENT:
				case TOK_NEW_LINE:
					// ignore
					break;
				case TOK_IDENTIFIER:
					if ( !bIsDefinedOperator ) {
						wclog << L"Warning: found identifier " << tokenExpression.getText() 
						      << L" in calculated expression." << endl;
					}

					if ( bExpectOperator ) {
						// Unexcpeted token found while parseing expression. Expected operator or end of expression.
						throw error::C4067B();
					}
					pNode = new Node();
					pNode->m_type    = Expression::NTP_VALUE;
					pNode->m_token   = token;
					//pNode->m_value   = Value( Expression::TYPE_IDENTIFIER, wstring() );
					pNode->m_value   = Value( Expression::TYPE_IDENTIFIER, tokenExpression.getText() );
					insertUnaryNode( &m_pRoot, pNode );
					bExpectOperator = true;
					break;
				case TOK_NUMBER:
					if ( bExpectOperator ) {
						// Unexcpeted token found while parseing expression. Expected operator or end of expression.
						throw error::C4067B();
					}
					pNode = new Node();
					pNode->m_type    = Expression::NTP_VALUE;
					pNode->m_token   = token;
					pNode->m_value   = Value( Expression::TYPE_INTEGER, tokenExpression.getText() );
					pNode->m_pParent = m_pRoot;
					insertUnaryNode( &m_pRoot, pNode );
					bExpectOperator = true;
					break;
				case TOK_LEFT_PARENTHESIS:
					nodeStack.push( m_pRoot );
					m_pRoot = NULL;
					break;
				case TOK_RIGHT_PARENTHESIS:
					if ( nodeStack.size() > 0 ) {
						pNode   = m_pRoot;
						if ( pNode == NULL ) {
							// Expression is empty.
							throw error::C1017C();
						}
						m_pRoot = nodeStack.top();
						nodeStack.pop();
						Node* pGroup = new Node();
						pGroup->m_type   = Expression::NTP_OPERATOR;
						pGroup->m_token  = token;
						pGroup->m_pRight = pNode;

						pNode->m_pParent = pGroup;
						insertUnaryNode( &m_pRoot, pGroup );
						bExpectOperator = true;
					} else {
						// Unmatched parenthesis.
						throw error::C1012();
					}
					break;
				case TOK_STRING:
					// Translate string to integer (Byte wise)
					if ( nTextLength > 6 ) {
						throw error::C2015( tokenExpression.getText() );
					} else {
						long long value = 0;
						for ( size_t i = 1; i < nTextLength - 1; ++i ) {
							wchar_t wc = tokenText[i];
							value = value << 16;
							value = value | (unsigned short)wc;
						}
						pNode = new Node();
						pNode->m_type    = Expression::NTP_VALUE;
						pNode->m_token   = token;
						pNode->m_value   = Value( value );
						pNode->m_pParent = m_pRoot;
						insertUnaryNode( &m_pRoot, pNode );
						bExpectOperator = true;
					}
					break;
				default:
 					if ( bExpectOperator ) {
						// Unexcpeted token found while parseing expression. Expected operator or end of expression.
						throw error::C4067B();
					} else {
						// Unexpected token {1} found while parseing expression.
						throw error::C4067( tokenExpression.getText() );
					}
				} // switch token
			}
		}

		if ( nodeStack.size() > 0 ) {
			// Unmatched parenthesis.
			throw error::C1012();
		}
	}
	catch ( ... ) {
		while ( nodeStack.size() > 0 ) {
			Node* pTopNode = nodeStack.top();
			delete pTopNode;
			nodeStack.pop();
		}
		throw;
	}
}


/**
** @brief Evaluate the expression tree.
*/
const Expression::Value& Expression::evaluate( const MacroSet* pMacros /* = NULL */ )
{
	if ( m_pRoot != NULL ) {
		m_value = m_pRoot->evaluate( pMacros );
	} else {
		m_value = Value();
	}

	return m_value;
}



} // namespace
