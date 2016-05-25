using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS431OS
{
    // Basic Utilities not provided within the COSMOS base.
    public class Utilities
    {
        // Convert Strings to Int32 values.
        public static Int32 stringToInt(String input)
        {
            Int32 output;
            output = Int32.Parse(input);
            return output;
        }

        // Calculate the result of an exponential expression.
        public static Int64 exponential(Int64 baseNum, Int64 exponent)
        {
            Int64 result = 1;

            for (int i = 0; i < exponent; i++)
            {
                result *= baseNum;
            }

            return result;
        }
    }
}
