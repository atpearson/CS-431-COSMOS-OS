using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS431OS
{
    // The File class allows for the creation of file objects to be used by the OS.
    public class File
    {
        // Attributes include name, extension, date of creation, size, number of lines, and the data to be stored in the file.
        String name;
        String extension;
        String date;
        Int32 size;
        Int32 line;
        ArrayList data;

        // Main constructor for File objects.
        public File(String n, String e)
        {
            name = n;
            extension = e;
            date = Cosmos.Hardware.RTC.Month + "/" + Cosmos.Hardware.RTC.DayOfTheMonth + "/" + Cosmos.Hardware.RTC.Year;
            size = 0;
            line = 0;
            data = new ArrayList();
        }

        // Alternate constructor for File objects which copies the attributes of an existing file.
        public File(File f)
        {
            name = f.name;
            extension = f.extension;
            date = f.date;
            size = f.size;
            line = 0;
            data = f.data;
        }

        // Allows the File data to be modified.
        public void setData(String s)
        {
            data.Add(s);
            size += s.Length;
        }

        // Retrieves the name of the file.
        public String getName()
        {
            return name;
        }

        // Retrieves the File extension.
        public String getExtension()
        {
            return extension;
        }

        // Retrieves the File's date.
        public String getDate()
        {
            return date;
        }

        // Retrieves the File's size.
        public Int32 getSize()
        {
            return size;
        }

        // Retrieves the number of lines in the File.
        public Int32 getLine()
        {
            return line;
        }

        // Retrieves the data of the File.
        public ArrayList getData()
        {
            return data;
        }

        // Allows the number of lines in the File to be modified.
        public void setLine(Int32 num)
        {
            line = num;
        }

        // Allows the number of lines in the File to be reset to zero.
        public void resetLine()
        {
            line = 0;
        }

        // Increase the number of lines in the File by one.
        public void incrementLine()
        {
            line++;
        }
    }
}
