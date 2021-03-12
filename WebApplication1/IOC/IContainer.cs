using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.IOC
{
    interface IContainer
    {
        /// <summary>
        /// 单接口单引用
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TIntance"></typeparam>
        void Register<TService, TIntance>(object[] objList = null) where TIntance : TService;
        /// <summary>
        /// 单接口多调用
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TIntance"></typeparam>
        /// <param name="Name"></param>
        void Register<TService, TIntance>(string Name=null,object[] objList=null) where TIntance : TService;
       /// <summary>
       /// 单接口单引用
       /// </summary>
       /// <typeparam name="TIntance"></typeparam>
       /// <returns></returns>
        TIntance Resolve<TIntance>();
        /// <summary>
        /// 单接口多调用
        /// </summary>
        /// <typeparam name="TIntance"></typeparam>
        /// <param name="Name"></param>
        /// <returns></returns>
        TIntance ResolveMore<TIntance>(string Name);
    }
}
