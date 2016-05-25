using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace CS431OS
{
    // The main Kernel file of the OS.
    public class Kernel : Sys.Kernel
    {
        // Attributes include various LinkedLists for storing files, variables, and operations to be performed.
        public static LinkedList<File> file_directory;
        public static LinkedList<Variable> variables;
        public static LinkedList<File> readyQueue;

        // BeforeRun displays an intro message and instantiates the Kernel attributes.
        protected override void BeforeRun()
        {
            Console.WriteLine("Cosmos booted successfully.\n Welcome to Andrew Pearson's OS.\n Type HELP for a list of commands.");
            file_directory = new LinkedList<File>();
            variables = new LinkedList<Variable>();
            readyQueue = new LinkedList<File>();
        }

        // Main run loop for the OS.
        protected override void Run()
        {
            Console.Write("C:\\>");
            var input = Console.ReadLine();
            Commands.interpret(input);
        }
    }
}
