using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Interface;
using WebApplication1.IOC;

namespace WebApplication1.Models
{
    public class StudentB : IStudentB
    {
        public string name { get; set; }
        public int num { get; set; }
        public IStudentA studentA { get; set; }
        public double num2 { get; set; }
        public StudentB([Paramter]string name,IStudentA studentA,[Paramter]int  num ,[Paramter]double num2)
        {
            this.name = name;
            this.studentA = studentA;
            this.num = num;
            this.num2 = num2;
        }
        public void test(string name)
        {
            Console.WriteLine("带参数IOC容器注入B，参数："+name);
        }

        public void testB()
        {
            Console.WriteLine("IOC容器注入B"+this.name);
        }
    }
}
