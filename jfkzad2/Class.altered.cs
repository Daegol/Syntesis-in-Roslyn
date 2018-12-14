using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jfkzad2
{
    class Class5 : System.Object , ISampleInterface
    {

    }

    class Class : System.Object
    {
        class Class2 : Class5
        {
            class Class3 : System.Object
            {
                
            }

        }
        
    }


    class Class4 : System.Object
    {
        static void Main(string[] args)
        {
            Console.WriteLine("test");
            Console.ReadLine();
        }

    }

    interface ISampleInterface
    {

    }


}
