#include "DAEDOCExtraDataCallbackHandler.h"
#include "generated14/COLLADASaxFWLColladaParserAutoGen14Attributes.h"


namespace DAEDOC
{

    //------------------------------
    ExtraDataCallbackHandler::ExtraDataCallbackHandler() 
        : m_IsOriginalIdField (false)
	{
	}
	
    //------------------------------
	ExtraDataCallbackHandler::~ExtraDataCallbackHandler()
	{
	}

    //------------------------------
    const ExtraInfo* ExtraDataCallbackHandler::findExtraInfo ( 
        const COLLADAFW::UniqueId& uniqueId, 
		const GeneratedSaxParser::StringHash& hashElement, const GeneratedSaxParser::String getElementName ) const
    {
        ExtraInfosMap::const_iterator it = m_ExtraInfos.find ( uniqueId );
        if ( it != m_ExtraInfos.end () )
        {
            const std::vector<ExtraInfo>& extraInfos = it->second;
            size_t numInfos = extraInfos.size ();
            for ( size_t i=0; i<numInfos; ++i )
            {

				GeneratedSaxParser::StringHash workhashElement = extraInfos[i].getElementHash ();
				if ( extraInfos[i].getElementHash () == hashElement ){
					if( extraInfos[i].getElementName() == getElementName ){
						return &extraInfos[i];
					}
				}
            }
        }
        return 0;
    }

    //------------------------------
    bool ExtraDataCallbackHandler::parseElement ( 
        const GeneratedSaxParser::ParserChar* profileName, 
		const GeneratedSaxParser::StringHash& elementHash, 
        const COLLADAFW::UniqueId& uniqueId ) 
    {
     
       m_CurrentExtraInfo.setElementHash ( elementHash );
       m_CurrentExtraInfo.setUniqueId ( uniqueId );
       
	   m_CurrentExtraInfo.setElementNameClear();
	   
	   return true;
        
       
    }

    //------------------------------
    bool ExtraDataCallbackHandler::elementBegin ( const GeneratedSaxParser::ParserChar* elementName, const GeneratedSaxParser::xmlChar** attributes ) 
    {

		m_IsOriginalIdField = true;
		m_CurrentExtraInfo.setElementNameIndent( elementName, strlen( elementName ) );
		m_workElementName.assign( elementName, strlen( elementName ) ) ;

        return true;
    }

    //------------------------------
    bool ExtraDataCallbackHandler::elementEnd ( const GeneratedSaxParser::ParserChar* elementName ) 
    {
        if ( m_IsOriginalIdField )
        {
            m_IsOriginalIdField = false;

            m_ExtraInfos [m_CurrentExtraInfo.getUniqueId ()].push_back ( m_CurrentExtraInfo );
            

			m_CurrentExtraInfo.setElementNameDeindent();
        }

        return true;
    }

    //------------------------------
    bool ExtraDataCallbackHandler::textData ( const GeneratedSaxParser::ParserChar* text, size_t textLength ) 
    {
        if ( m_IsOriginalIdField )
        {
			if( !m_workText.empty() )
			{
				int chkHeaderFindStart;
				int chkHeaderFindEnd;

				std::string findStr = "<" + m_workElementName + " " ;
				chkHeaderFindStart = m_workText.find( findStr );

				if( std::string::npos != chkHeaderFindStart ){
					chkHeaderFindEnd = m_workText.find( ">", chkHeaderFindStart + findStr.size() );
					if( std::string::npos != chkHeaderFindEnd ){
						std::string getVal = m_workText.substr( chkHeaderFindStart + findStr.size() , chkHeaderFindEnd - chkHeaderFindStart - findStr.size() -1 );
						
						m_CurrentExtraInfo.setOriginalVal(getVal);
						m_ExtraInfos [m_CurrentExtraInfo.getUniqueId ()].push_back ( m_CurrentExtraInfo );

					}
					
				}
			}

            m_CurrentExtraInfo.setOriginalVal ( text, textLength );
			m_workText = GeneratedSaxParser::String( text );

        }
        return true;
    }

} // namespace DAEDOC
