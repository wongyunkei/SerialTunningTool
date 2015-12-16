using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialTunningTool
{
    class MathTools
    {
        unsafe static int floatToIntBits(float f)
        {
            return *((int*)&f);
             
        }

        unsafe static float intBitsToFloat(int intBits)
        {            
            return *((float*)&intBits);
        }

        static public int FloatToHalfInt(float _float){

    	    int fbits = floatToIntBits(_float);
    	    int sign = fbits >> 16 & 0x8000;
    	    int val = ( fbits & 0x7fffffff ) + 0x1000;

    	    if( val >= 0x47800000 )
    	    {
    		    if( ( fbits & 0x7fffffff ) >= 0x47800000 )
    		    {
    			    if( val < 0x7f800000 ){
    				    return sign | 0x7c00;
    			    }
    			    return sign | 0x7c00 | ( fbits & 0x007fffff ) >> 13;
    		    }
    		    return sign | 0x7bff;
    	    }
    	    if( val >= 0x38800000 ){
    		    return sign | val - 0x38000000 >> 13;
    	    }
    	    if( val < 0x33000000 ){
    		    return sign;
    	    }
    	    val = ( fbits & 0x7fffffff ) >> 23;
    	    return sign | ( ( (fbits & 0x7fffff) | 0x800000 ) + ( 0x800000 >> val - 102 ) >> 126 - val );
        }

        static public float HalfIntToFloat(int hbits){

    	    int mant = hbits & 0x03ff;
    	    int exp =  hbits & 0x7c00;
    	        if( exp == 0x7c00 ){
    	            exp = 0x3fc00;
    	        }
    	        else if( exp != 0 ){
    	            exp += 0x1c000;
    	            if( mant == 0 && exp > 0x1c400 ){
    	        	    return intBitsToFloat(( hbits & 0x8000 ) << 16 | exp << 13 | 0x3ff);
    	            }
    	        }
    	        else if( mant != 0 ){
    	            exp = 0x1c400;
    	            do {
    	                mant <<= 1;
    	                exp -= 0x400;
    	            } while( ( mant & 0x400 ) == 0 );
    	            mant &= 0x3ff;
    	        }
    	        return intBitsToFloat(( hbits & 0x8000 ) << 16 | ( exp | mant ) << 13);
        }

    }
}
