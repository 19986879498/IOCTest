using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Interface;

namespace WebApplication1.Models
{
    public class StudentC:IStudentA
    {
        public void test()
        {
            Console.WriteLine("单接口多实现");
        }

        public void testA()
        {
            throw new NotImplementedException();
        }

        public void  testB(string name )
        {
            Console.WriteLine(name);
        }
    }
}
