/**
** @file
** @author Ralf Seidel
** @brief Custom code page converter used for converting wide character strings
**        in the input and output files.
**
**
** © 2009 by Heinrich und Seidel GbR Wuppertal.
*/
namespace sqtpp {

typedef std::codecvt<wchar_t , char, mbstate_t> CodePageConverter;

/**
** @brief Converter for single byte coded string streams.
*/
class SbcsConverter : public CodePageConverter
{
private:
	typedef CodePageConverter base;

	unsigned int m_codePageId;

public:
	explicit SbcsConverter( unsigned int codePageId, size_t refCount = 0 );
	virtual ~SbcsConverter();


	unsigned int getCodePageId() const throw()
	{
		return m_codePageId;
	}

protected:
    virtual result do_in( mbstate_t& state
	                    , const char* pFrom , const char* pFromMax , const char*& pFromNext
						, wchar_t* pTo , wchar_t* pToMax, wchar_t*& pToNext ) const override;

    virtual result do_out( mbstate_t& state
	                     , const wchar_t* pFrom , const wchar_t* pFromMax, const wchar_t*& pFromNext 
						 , char* pTo, char* pToMax , char*& pToNext ) const override;

    virtual result do_unshift( mbstate_t& state, char* pTo , char* pToMax, char*& pToNext ) const override;

    virtual int do_length( mbstate_t& state, const char* pFrom, const char* pFromMax , size_t toLength ) const throw() override;

    virtual bool do_always_noconv() const throw() override;

    virtual int do_max_length() const throw() override;

    virtual int do_encoding() const throw() override;
};

/**
** @brief Converter for UTF7 streams.
*/
class Utf7Converter : public CodePageConverter
{
private:
	typedef CodePageConverter base;

public:
	explicit Utf7Converter( size_t refCount = 0 );
	virtual ~Utf7Converter();

	/// Check if the given character is the start byte of an UTF-7 character sequence.
	static bool is_start_byte( char byte );

protected:
    virtual result do_in( mbstate_t& state
	                    , const char* pFrom , const char* pFromMax , const char*& pFromNext
						, wchar_t* pTo , wchar_t* pToMax, wchar_t*& pToNext ) const override;

    virtual result do_out( mbstate_t& state
	                     , const wchar_t* pFrom , const wchar_t* pFromMax, const wchar_t*& pFromNext 
						 , char* pTo, char* pToMax , char*& pToNext ) const override;

    virtual result do_unshift( mbstate_t& state, char* pTo , char* pToMax, char*& pToNext ) const override;

    virtual int do_length( mbstate_t& state , const char* pFrom, const char* pFromMax , size_t toLength ) const throw() override;

    virtual bool do_always_noconv() const throw() override;

    virtual int do_max_length() const throw() override;

    virtual int do_encoding() const throw() override;
};


/**
** @brief Converter for UTF8 streams.
*/
class Utf8Converter : public CodePageConverter
{
private:
	typedef CodePageConverter base;

public:
	explicit Utf8Converter( size_t refCount = 0 );
	virtual ~Utf8Converter();

	/// Check if the given character is the start byte of an UTF-8 character sequence.
	static bool is_start_byte( char byte );

	/// Get the length of the sequence defined by the first byte of a UTF-8 character.
	static int sequence_length( char start );


protected:
    virtual result do_in( mbstate_t& state
	                    , const char* pFrom , const char* pFromMax , const char*& pFromNext
						, wchar_t* pTo , wchar_t* pToMax, wchar_t*& pToNext ) const override;

    virtual result do_out( mbstate_t& state
	                     , const wchar_t* pFrom , const wchar_t* pFromMax, const wchar_t*& pFromNext 
						 , char* pTo, char* pToMax , char*& pToNext ) const override;

    virtual result do_unshift( mbstate_t& state, char* pTo , char* pToMax, char*& pToNext ) const override;

    virtual int do_length( mbstate_t& state , const char* pFrom, const char* pFromMax , size_t toLength ) const throw() override;

    virtual bool do_always_noconv() const throw() override;

    virtual int do_max_length() const throw() override;

    virtual int do_encoding() const throw() override;
};

/**
** @brief Codec for unicode (UTF16) streams.
**
** By default the std c++ streams are converting all unicode strings to some
** 8 bit character string. This class defines a "converter" that simply does
** nothing i.e. it reads and writes unicode files. It is based on samples 
** provided in the article.
**	"Upgrading an STL-based application to use Unicode."
** found on code project (http://www.codeproject.com/KB/stl/upgradingstlappstounicode.aspx)
*/
class Utf16Converter : public CodePageConverter
{
private:
	typedef CodePageConverter base;

public:
    explicit Utf16Converter( size_t refCount = 0 );

	virtual ~Utf16Converter();

protected:
    virtual result do_in( mbstate_t& state
	                    , const char* pFrom , const char* pFromMax , const char*& pFromNext
						, wchar_t* pTo , wchar_t* pToMax, wchar_t*& pToNext ) const override;

    virtual result do_out( mbstate_t& state
	                     , const wchar_t* pFrom , const wchar_t* pFromMax , const wchar_t*& pFromNext 
						 , char* pTo, char* pToMax , char*& pToNext ) const override;

    virtual result do_unshift( mbstate_t& state, char* pTo , char* pToMax, char*& pToNext ) const override;

    virtual int do_length( mbstate_t& state , const char* pFrom, const char* pFromMax , size_t fromLength ) const throw() override;

    virtual bool do_always_noconv() const throw() override;

    virtual int do_max_length() const throw() override;

    virtual int do_encoding() const throw() override;
} ;

/**
** @brief Codec for unicode (UTF16BE) streams.
*/
class Utf16BeConverter : public CodePageConverter
{
private:
	typedef CodePageConverter base;

public:
    explicit Utf16BeConverter( size_t refCount = 0 );

	virtual ~Utf16BeConverter();

protected:
    virtual result do_in( mbstate_t& state
	                    , const char* pFrom , const char* pFromMax , const char*& pFromNext
						, wchar_t* pTo , wchar_t* pToMax, wchar_t*& pToNext ) const override;

    virtual result do_out( mbstate_t& state
	                     , const wchar_t* pFrom , const wchar_t* pFromMax , const wchar_t*& pFromNext 
						 , char* pTo, char* pToMax , char*& pToNext ) const override;

    virtual result do_unshift( mbstate_t& state, char* pTo , char* pToMax, char*& pToNext ) const override;

    virtual int do_length( mbstate_t& state , const char* pFrom, const char* pFromMax , size_t fromLength ) const throw() override;

    virtual bool do_always_noconv() const throw() override;

    virtual int do_max_length() const throw() override;

    virtual int do_encoding() const throw() override;
} ;


} // namespace
