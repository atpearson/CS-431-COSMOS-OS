using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS431OS
{
    // The Commands class contains most of the functionality of the OS by containing all the methods that the user calls 
    // while working with the OS.
    public class Commands
    {
        // The interpret method takes the user's input and separates out the keyword representing a command and runs 
        // the appropriate method corresponding to the given command.
        public static void interpret(String input)
        {
            Char[] inputC = input.ToCharArray();
            Boolean space = false;
            for (int i = 0; i < inputC.Length; i++)
            {
                if (inputC[i] == ' ')
                {
                    space = true;
                    break;
                }
            }
                if (space)
                {
                    String command = "";
                    Int32 index = 0;
                    while (inputC[index] != ' ')
                    {
                        command += inputC[index];
                        index++;
                    }
                    while (inputC[index] == ' ' || inputC[index] == '=')
                    {
                        index++;
                    }
                    Char[] sub = new Char[inputC.Length - index];
                    for (int i = 0; i < sub.Length; i++)
                    {
                        sub[i] = inputC[index];
                        index++;
                    }
                    if (command.ToLower() == "create")
                    {
                        create(sub);
                    }
                    else if (command.ToLower() == "run")
                    {
                        run(sub);
                    }
                    else if (command.ToLower() == "runall")
                    {
                        runAll(sub);
                    }
                    else if (command.ToLower() == "set")
                    {
                        Variable newVar = convertVariable(sub);
                        LinkedListNode<Variable> temp = Kernel.variables.First;
                        Boolean found = false;
                        while (temp != null)
                        {
                            if (temp.Value.name == newVar.name) 
                            {
                                temp.Value.setValue(newVar.value);
                                found = true;
                                break;
                            }
                            else
                                temp = temp.Next;
                        }
                        if(!found)
                            Kernel.variables.AddLast(newVar);
                    }
                    else if (command.ToLower() == "out")
                    {
                        String name = new String(sub), var = output(sub);
                        if (var == null)
                            Console.WriteLine("Variable does not exist.");
                        else
                            Console.WriteLine(name + " = " + var);
                    }
                    else
                    {
                        Console.WriteLine("Invalid Command.");
                    }
                }
                else
                {
                    command(input);
                }
        }

        // If the input contains no spaces then this command method is run which then executes one of the basic commands such as date or time.
        public static void command(String input)
        {
            if (input.ToLower() == "date")
                Commands.date();
            else if (input.ToLower() == "time")
                Commands.time();
            else if (input.ToLower() == "dir")
                Commands.dir();
            else if (input.ToLower() == "help")
                Commands.help();
            else
                Console.WriteLine("Invalid Command.");
        }

        // Displays the current time to the user.
        public static void time()
        {
            Console.WriteLine("The current time is: " + Cosmos.Hardware.RTC.Hour + ":" + Cosmos.Hardware.RTC.Minute + ":" + Cosmos.Hardware.RTC.Second);
        }

        // Displays the current date to the user.
        public static void date()
        {
            Console.WriteLine("The current date is: " + Cosmos.Hardware.RTC.Month + "/" + Cosmos.Hardware.RTC.DayOfTheMonth + "/" + Cosmos.Hardware.RTC.Year);
        }

        // Allows the user to create a new File object by giving it a name, extension, and data.
        public static void create(Char[] full)
        {
            String name = "", ext = "";
            Boolean e = false;
            for (int i = 0; i < full.Length; i++)
            {
                name += full[i];
                if (full[i] == '.')
                    e = true;
                if (e)
                    ext += full[i];
            }
            File file = new File(name, ext);
            String input = "";
            Int32 count = 1;
            while (input != "save")
            {
                Console.Write(count + ": ");
                input = Console.ReadLine();
                if (input != "save")
                {
                    file.setData(input);
                    count++;
                }
            }
            Console.WriteLine("*** File Saved ***");
            Kernel.file_directory.AddLast(file);
        }

        // Displays the File directory which contains all the saved files along with their extensions, data, and size.
        public static void dir()
        {
            Console.WriteLine("Filename\tExtension\tDate\tSize");
            Console.WriteLine("---------------------------------------");
            LinkedListNode<File> temp = Kernel.file_directory.First;
            while (temp != null)
            {
                Console.WriteLine(temp.Value.getName() + "\t" + temp.Value.getExtension() + "\t\t" + temp.Value.getDate() + "\t" + temp.Value.getSize() + " b");
                temp = temp.Next;
            }
            Console.WriteLine("Total Files: " + Kernel.file_directory.Count);
        }

        // Runs the commands contained with the given file.
        // Originally only meant to run BAT files, this was changed at the request of the professor.
        public static void run(Char[] fileName)
        {
            String name = "", ext = "";
            Boolean e = false;
            for (int i = 0; i < fileName.Length; i++)
            {
                name += fileName[i];
                if (fileName[i] == '.')
                    e = true;
                if (e)
                    ext += fileName[i];
            }
            /*
             * if (!checkIfBat(ext))
             * {
             *     Console.WriteLine("Invalid file type.");
             *     return;
             * }
             */
            File file;
            if (checkFileExists(name) == null)
            {
                Console.WriteLine(name + " does not exist.");
                return;
            }
            file = new File(checkFileExists(name));
            Kernel.readyQueue.AddLast(file);
            process();
        }

        /* 
         * Not needed if we can execute any file type.
         * public static Boolean checkIfBat(String ext)
         * {
         *     if (ext == ".bat")
         *         return true;
         *     else
         *         return false;
         * }
         */

        //Runs all the files given.
        public static void runAll(Char[] fileNames)
        {
            String name = "";
            ArrayList files = new ArrayList();
            for (int i = 0; i < fileNames.Length; i++)
            {
                if (fileNames[i] == ' ')
                {
                    File file;
                    if (checkFileExists(name) == null)
                    {
                        Console.WriteLine(name + " does not exist.");
                        return;
                    }
                    file = new File(checkFileExists(name));
                    Kernel.readyQueue.AddLast(file);
                    name = "";
                }
                else
                    name += fileNames[i];
            }
            if (name.Length > 0)
            {
                File file;
                if (checkFileExists(name) == null)
                {
                    Console.WriteLine(name + " does not exist.");
                    return;
                }
                file = new File(checkFileExists(name));
                Kernel.readyQueue.AddLast(file);
            }
            process();
        }

        /*
         * The process method runs one command from the File at the front of the queue and adds it back to the end of the queue if 
         * more commands remain.  This allows the OS to execute multiple files somewhat in parallel.
         */
        public static void process()
        {
            while (Kernel.readyQueue.Count > 0)
            {
                File current = Kernel.readyQueue.First.Value;
                Kernel.readyQueue.RemoveFirst();
                ArrayList data = current.getData();
                interpret(data[current.getLine()] as String);
                current.incrementLine();
                if(current.getLine() != data.Count)
                    Kernel.readyQueue.AddLast(current);
            }
        }

        // Checks if the given File exists.
        public static File checkFileExists(String name)
        {
            LinkedListNode<File> temp = Kernel.file_directory.First;
            while (temp != null)
            {
                if (temp.Value.getName() == name)
                    return temp.Value;
                temp = temp.Next;
            }
            return null;
        }

        // Converts a char array into a Variable object.
        public static Variable convertVariable(Char[] var)
        {
            String name = "";
            Int32 index = 0;
            while (var[index] != ' ' && var[index] != '=')
            {
                name += var[index];
                index++;
            }
            while (var[index] == ' ' || var[index] == '=')
            {
                index++;
            }
            Char[] sub = new Char[var.Length - index];
            for (int i = 0; i < sub.Length; i++)
            {
                sub[i] = var[index];
                index++;
            }
            String value = evaluate(sub);
            Variable v = new Variable(name, value);
            return v;
        }

        // The evaluate method takes a char array and runs it as either a string or numeric expression depending on 
        // whether it includes and strings within the expression.
        public static String evaluate(Char[] eq)
        {
            String value;

            if (checkIfContainsString(eq))
            {
                value = evaluateString(eq);
            }
            else
            {
                value = evaluateNumeric(eq);
            }

            return value;
        }

        // Evaluates the char array as a string rather than numeric values, so arithmetic cannot be used.
        public static String evaluateString(Char[] eq)
        {
            String tempA = "", value = "";
            LinkedList<String> equation = new LinkedList<String>();
            for (int i = 0; i < eq.Length; i++)
            {
                if (eq[i] != '+' && eq[i] != '=')
                {
                    tempA += eq[i];
                }
                if (eq[i] == '+' && tempA.Length > 0)
                {
                    equation.AddLast(tempA);
                    tempA = "";
                }
            }
            if(tempA.Length > 0)
                equation.AddLast(tempA);
            LinkedListNode<String> current = equation.First;
            Char[] tempB;
            while(current != null)
            {
                tempB = current.Value.ToCharArray();
                if (tempB[0] == '"')
                {
                    for (int j = 1; j < tempB.Length-1; j++)
                    {
                        value += tempB[j];
                    }
                }
                else
                {
                    value += output(tempB);
                }
                current = current.Next;
            }
            return value;
        }

        // The evaluateNumeric method takes a char array representing a basic equation such as 3 + 5 and evaluates it.
        public static String evaluateNumeric(Char[] eq)
        {
            String tempA = "", value = "";
            LinkedList<String> equation = new LinkedList<String>();
            for (int i = 0; i < eq.Length; i++)
            {
                if ((eq[i] == ' ' || eq[i] == '+' || eq[i] == '-' || eq[i] == '*' || eq[i] == '/' || eq[i] == '&' || eq[i] == '|' || eq[i] == '^') && tempA.Length > 0)
                {
                    equation.AddLast(tempA);
                    tempA = "";
                }
                if (eq[i] != ' ' && eq[i] != '=')
                {
                    tempA += eq[i];
                }
                if ((eq[i] == ' ' || eq[i] == '+' || eq[i] == '-' || eq[i] == '*' || eq[i] == '/' || eq[i] == '&' || eq[i] == '|' || eq[i] == '^') && tempA.Length > 0)
                {
                    equation.AddLast(tempA);
                    tempA = "";
                }
            }
            if (tempA.Length > 0)
                equation.AddLast(tempA);

            LinkedListNode<String> current = equation.First;
            Char[] tempB;
            Int64 op = 0, solve = 0;
            while (current != null)
            {
                tempB = current.Value.ToCharArray();
                if (tempB[0] == '+')
                {
                    op = 1;
                }
                else if (tempB[0] == '-')
                {
                    op = 2;
                }
                else if (tempB[0] == '*')
                {
                    op = 3;
                }
                else if (tempB[0] == '/')
                {
                    op = 4;
                }
                else if (tempB[0] == '&')
                {
                    op = 5;
                }
                else if (tempB[0] == '|')
                {
                    op = 6;
                }
                else if (tempB[0] == '^')
                {
                    op = 7;
                }
                else
                {
                    if ((tempB[0] >= 'A' && tempB[0] <= 'Z') || (tempB[0] >= 'a' && tempB[0] <= 'z'))
                    {
                        if (op == 0)
                            solve = Utilities.stringToInt(output(tempB));
                        else if (op == 1)
                        {
                            solve += Utilities.stringToInt(output(tempB));
                            op = 0;
                        }
                        else if (op == 2)
                        {
                            solve -= Utilities.stringToInt(output(tempB));
                            op = 0;
                        }
                        else if (op == 3)
                        {
                            solve *= Utilities.stringToInt(output(tempB));
                            op = 0;
                        }
                        else if (op == 4)
                        {
                            solve /= Utilities.stringToInt(output(tempB));
                            op = 0;
                        }
                        else if (op == 5)
                        {
                            solve &= Utilities.stringToInt(output(tempB));
                            op = 0;
                        }
                        else if (op == 6)
                        {
                            solve |= Utilities.stringToInt(output(tempB));
                            op = 0;
                        }
                        else if (op == 7)
                        {
                            solve = Utilities.exponential(solve, Utilities.stringToInt(output(tempB)));
                            op = 0;
                        }
                    }
                    else
                    {
                        tempA = new String(tempB);
                        if (op == 0)
                            solve = Utilities.stringToInt(tempA);
                        else if (op == 1)
                        {
                            solve += Utilities.stringToInt(tempA);
                            op = 0;
                        }
                        else if (op == 2)
                        {
                            solve -= Utilities.stringToInt(tempA);
                            op = 0;
                        }
                        else if (op == 3)
                        {
                            solve *= Utilities.stringToInt(tempA);
                            op = 0;
                        }
                        else if (op == 4)
                        {
                            solve /= Utilities.stringToInt(tempA);
                            op = 0;
                        }
                        else if (op == 5)
                        {
                            solve &= Utilities.stringToInt(tempA);
                            op = 0;
                        }
                        else if (op == 6)
                        {
                            solve |= Utilities.stringToInt(tempA);
                            op = 0;
                        }
                        else if (op == 7)
                        {
                            solve = Utilities.exponential(solve, Utilities.stringToInt(tempA));
                            op = 0;
                        }
                    }
                }
                current = current.Next;
            }
            value = solve.ToString();
            return value;
        }

        // The checkIfContainsString method checks a char array for the appearance of any strings.
        public static Boolean checkIfContainsString(Char[] input)
        {
            String tempA = "", value = "";
            LinkedList<String> equation = new LinkedList<String>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != ' ' && input[i] != '+' && input[i] != '-' && input[i] != '*' && input[i] != '/' && input[i] != '=')
                {
                    tempA += input[i];
                }
                if (input[i] == ' ' && tempA.Length > 0)
                {
                    equation.AddLast(tempA);
                    tempA = "";
                }
            }
            if (tempA.Length > 0)
                equation.AddLast(tempA);
            LinkedListNode<String> current = equation.First;
            Char[] tempB, tempC;
            while (current != null)
            {
                tempB = current.Value.ToCharArray();
                if (tempB[0] == '"')
                {
                    return true;
                }
                else
                {
                    if (output(tempB) != null)
                    {
                        value = output(tempB);
                        tempC = value.ToCharArray();
                        for (int j = 0; j < tempC.Length; j++)
                        {
                            if ((tempC[j] >= 'A' && tempC[j] <= 'Z') || (tempC[j] >= 'a' && tempC[j] <= 'z'))
                            {
                                return true;
                            }
                        }
                    }
                }
                current = current.Next;
            }
            return false;
        }

        // The output method converts a char array to a string.
        public static String output(Char[] var)
        {
            String varName = new String(var);
            LinkedListNode<Variable> temp = Kernel.variables.First;
            while (temp != null)
            {
                if (temp.Value.name == varName)
                    break;
                else
                    temp = temp.Next;
            }
            if (temp == null)
                return null;
            else
                return temp.Value.getValueString();
        }

        // The help method displays all available commands to the user with a brief description of its' respective function.
        public static void help()
        {
            Console.WriteLine("Command List:");
            Console.WriteLine("time - displays current time");
            Console.WriteLine("date - displays current date");
            Console.WriteLine("create <filename>.<extension> - creates a file with the specified name and extension");
            Console.WriteLine("dir - shows all saved files in the system");
            Console.WriteLine("set <var> = <expression> - creates a new variable with a value equal to the given expression");
            Console.WriteLine("out <var> - displays the value of the saved variable specified");
        }
    }
}
