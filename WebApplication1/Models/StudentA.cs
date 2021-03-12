using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Interface;

namespace WebApplication1.Models
{
    public class StudentA : IStudentA
    {
        public IStudentB studentB { get; set; }
        public StudentA(IStudentB studentB)
        {
            this.studentB = studentB;
        }
        [IOC.ContainerProptry]
        public string str { get; set; }=0+"";
        [IOC.IOCMethods]
        public void testA()
        {
            Console.WriteLine("IOC容器注入A");
        }

        public void testB(string name)
        {
            Console.WriteLine("带参数注入A");
        }
    }
}
