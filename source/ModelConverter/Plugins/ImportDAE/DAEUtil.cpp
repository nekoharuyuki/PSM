#include "COLLADAFWPolygons.h"
#include "COLLADAFWTrifans.h"
#include "COLLADAFWTristrips.h"
#include "COLLADAFWEdge.h"
#include "COLLADAFWMorphController.h"

#include "DAEUtil.h"


namespace DAEDOC
{
double getDoubleValue ( 
    const COLLADAFW::FloatOrDoubleArray &inputValuesArray, 
    const size_t position )
{
    double inputValue = 0;

    size_t numInputValues = inputValuesArray.getValuesCount ();
    if ( position > numInputValues - 1 )
        std::cerr << "Out of range error!" << std::endl;

    const COLLADAFW::FloatOrDoubleArray::DataType& inputDataType = inputValuesArray.getType ();
    switch ( inputDataType )
    {
    case COLLADAFW::FloatOrDoubleArray::DATA_TYPE_DOUBLE:
        {
            const COLLADAFW::DoubleArray* inputValues = inputValuesArray.getDoubleValues ();
            inputValue = (*inputValues) [position];
        }
        break;
    case COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT:
        {
            const COLLADAFW::FloatArray* inputValues = inputValuesArray.getFloatValues ();
            inputValue = (double)(*inputValues) [position];
        }
        break;
    default:
        std::cerr << "AnimationImporter::setInTangents(): inputDataType unknown data type!" << std::endl;
    }

    return inputValue;
}

    // -----------------------------------
float getFloatValue ( 
    const COLLADAFW::FloatOrDoubleArray &inputValuesArray, 
    const size_t position )
{
    float inputValue = 0;

    size_t numInputValues = inputValuesArray.getValuesCount ();
    if ( position > numInputValues - 1 )
        std::cerr << "Out of range error!" << std::endl;

    const COLLADAFW::FloatOrDoubleArray::DataType& inputDataType = inputValuesArray.getType ();
    switch ( inputDataType )
    {
    case COLLADAFW::FloatOrDoubleArray::DATA_TYPE_DOUBLE:
        {
            const COLLADAFW::DoubleArray* inputValues = inputValuesArray.getDoubleValues ();
            inputValue = (float)(*inputValues) [position];
        }
        break;
    case COLLADAFW::FloatOrDoubleArray::DATA_TYPE_FLOAT:
        {
            const COLLADAFW::FloatArray* inputValues = inputValuesArray.getFloatValues ();
            inputValue = (*inputValues) [position];
        }
        break;
    default:
        std::cerr << "AnimationImporter::setInTangents(): inputDataType unknown data type!" << std::endl;
    }

    return inputValue;
}


}	// namespace DAEDOC