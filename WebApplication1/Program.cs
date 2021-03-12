using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication1.Interface;
using WebApplication1.IOC;
using WebApplication1.Models;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IContainer container = new Container();
            container.Register<IStudentA, StudentA>(objList:new object[] { "name",20,5.20});
            container.Register<IStudentB, StudentB>(objList:new object[] { "name", 20, 5.20 });
            container.Register<IStudentA, StudentC>("c");


            IStudentB stuB = container.Resolve<IStudentB>();
            stuB.testB();
          // IStudentA studentA= container.Resolve<IStudentA>();
            //���Թ��캯��ע��
            //studentA.testA();
            //���ӿڶ�ʵ��
            StudentC studentc = (StudentC)container.ResolveMore<IStudentA>("c");
            studentc.test();

           // CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

  
}
