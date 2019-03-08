#ifndef __DAEDOC_EXTRADATACALLBACKHANDLER_H__
#define __DAEDOC_EXTRADATACALLBACKHANDLER_H__

#include <vector>
#include "COLLADASaxFWLIExtraDataCallbackHandler.h"


namespace DAEDOC
{

    class ExtraInfo
    { 
    private:

        /** The hash value of the currently parsed extra data element. */
		GeneratedSaxParser::StringHash m_ElementHash; 

        /** The uniqueId of the currently parsed extra data element. */
        COLLADAFW::UniqueId m_UniqueId;

		/**  ElementName */
		GeneratedSaxParser::String m_elementName;

		/** The text value of the current original id. */
		GeneratedSaxParser::String m_OriginalVal;
	
        

    public:

        /** Constructor. */
        ExtraInfo () {}

        /** Destructor. */
        virtual ~ExtraInfo () {}

        /** The hash value of the currently parsed extra data element. */
		const GeneratedSaxParser::StringHash& getElementHash () const { return m_ElementHash; }
		void setElementHash ( const GeneratedSaxParser::StringHash& val ) { m_ElementHash = val; }

        /** The uniqueId of the currently parsed extra data element. */
        const COLLADAFW::UniqueId& getUniqueId () const { return m_UniqueId; }
        void setUniqueId ( const COLLADAFW::UniqueId& val ) { m_UniqueId = val; }


		/** The text value of the current original id. */
		const GeneratedSaxParser::String& getOriginalVal() const { return m_OriginalVal; }
        void setOriginalVal ( const GeneratedSaxParser::String& val ) { m_OriginalVal = val; }
        void setOriginalVal ( const GeneratedSaxParser::ParserChar* text, size_t textLength ) 
        { 
            m_OriginalVal.assign ( text, textLength ); 
        }

		GeneratedSaxParser::String getElementName() const 
		{ 
			return m_elementName;
		}
		

		void setElementNameClear() 
		{
			m_elementName.clear();
		}
		void setElementNameIndent ( const GeneratedSaxParser::ParserChar* text, size_t textLength ) 
        { 
			GeneratedSaxParser::String workStr;
            workStr.assign ( text, textLength ); 

			if( m_elementName.size() != 0 ){
				m_elementName = m_elementName + "::" + workStr;
			}else {
				m_elementName = workStr;
			}
        }
		void setElementNameDeindent() {
			if( m_elementName.find("::") !=  std::string::npos ){
				m_elementName = m_elementName.substr( 0, m_elementName.rfind("::") );
			}else {
				m_elementName.clear();
			}
		}
        
    };

    /** Implementation of an extra data callback handler with the callback handler interface. */
	class ExtraDataCallbackHandler : public COLLADASaxFWL::IExtraDataCallbackHandler
    {
    private:

        typedef std::map<COLLADAFW::UniqueId, std::vector<ExtraInfo> > ExtraInfosMap;

	private:
	
        /** True, if the current text field is the original id field. */
        bool m_IsOriginalIdField;

        ExtraInfo		m_CurrentExtraInfo;

        ExtraInfosMap	m_ExtraInfos;

		GeneratedSaxParser::String m_workText;
		GeneratedSaxParser::String m_workElementName;

	public:

        /** Constructor. */
		ExtraDataCallbackHandler();

        /** Destructor. */
		virtual ~ExtraDataCallbackHandler();

        /** Returns the extra info with the searched id and hash string value. */
        const ExtraInfo* findExtraInfo ( 
            const COLLADAFW::UniqueId& uniqueId, 
			const GeneratedSaxParser::StringHash& hashElement, GeneratedSaxParser::String getElementName ) const;

        /** Method to ask, if the current callback handler want to read the data of the given extra element. */
        virtual bool parseElement ( 
            const GeneratedSaxParser::ParserChar* profileName, 
			const GeneratedSaxParser::StringHash& elementHash, 
            const COLLADAFW::UniqueId& uniqueId );

        /** The methods to get the extra data tags to the registered callback handlers. */
        virtual bool elementBegin( const GeneratedSaxParser::ParserChar* elementName, const GeneratedSaxParser::xmlChar** attributes);
        virtual bool elementEnd(const GeneratedSaxParser::ParserChar* elementName );
        virtual bool textData(const GeneratedSaxParser::ParserChar* text, size_t textLength);

	private:

        /** Disable default copy ctor. */
		ExtraDataCallbackHandler( const ExtraDataCallbackHandler& pre );

        /** Disable default assignment operator. */
		const ExtraDataCallbackHandler& operator= ( const ExtraDataCallbackHandler& pre );

	};

} // namespace DAEDOC

#endif // __DAEDOC_EXTRADATACALLBACKHANDLER_H__
