using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS431OS
{
    // The Variable object is used for saving values and performing arithmetic within the OS.
    public class Variable
    {
        // Attributes include a name and value pair such as x = 5 where x is the name and 5 is the value.
        public String name;
        public String value;

        // Constructor for Variable objects.
        public Variable(String n, String v)
        {
            name = n;
            value = v;
        }

        // Allows the Variable name to be changed.
        public void setName(String n)
        {
            name = n;
        }

        // Allows the Variable value to be changed.
        public void setValue(String v)
        {
            value = v;
        }

        // Returns the Variable name.
        public String getName()
        {
            return name;
        }

        // Returns the Variable value.
        public String getValueString()
        {
            return value;
        }

        // Returns the Variable value as an Int32.
        public Int32 getValueNumeric()
        {
            return Utilities.stringToInt(value);
        }
    }
}
