using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace WebApplication1.IOC
{
    public class Container:IContainer
    {
        public Dictionary<string, Type> dicTypes = new Dictionary<string, Type>();
        public Dictionary<string, object[]> paraList = new Dictionary<string, object[]>();
        /// <summary>
        /// 注册容器
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TIntance"></typeparam>
      public void Register<TService, TIntance>(object [] objList=null) where TIntance:TService
        {
            dicTypes.Add(typeof(TService).FullName, typeof(TIntance));
            paraList.Add(typeof(TService).FullName, objList);
        }
        /// <summary>
        /// 实现单接口多实现的注册 （添加别名）
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TIntance"></typeparam>
        /// <param name="Name"></param>
        public void Register<TService, TIntance>(string Name=null,object[] objList=null) where TIntance : TService
        {
            dicTypes.Add(GetKey(typeof(TService),Name), typeof(TIntance));
            paraList.Add(GetKey(typeof(TService), Name), objList);
        }
        /// <summary>
        /// 获取服务的类型
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="Name"></param>
        /// <returns></returns>
        private string GetKey(Type service,string Name) => $"{service.FullName}__{Name}";

        /// <summary>
        /// 获取抽象的类
        /// 
        /// </summary>
        /// <typeparam name="TIntance"></typeparam>
        /// <returns></returns>
        public TIntance ResolveMore<TIntance>(string Name)
        {
            return (TIntance)this.Resolve(typeof(TIntance),Name);
        }
        /// <summary>
        /// 运用递归来依赖细节
        /// </summary>
        /// <param name="typeIntance"></param>
        /// <returns></returns>
        public object Resolve(Type typeIntance,string Name)
        {
            string key = GetKey(typeIntance,Name);
            Type type = dicTypes[key];

            #region 选择合适的构造函数
            ConstructorInfo ctorinfo = null;

            #region 选择构造函数
            //2、标记特性  标记指定的类
            ctorinfo = type.GetConstructors().FirstOrDefault(c => c.IsDefined(typeof(ContainerAttribute), true));
            if (ctorinfo == null)
            {
                //1、选择参数个数最多的
                ctorinfo = type.GetConstructors().OrderByDescending(u => u.GetParameters().Length).FirstOrDefault();
            } 
            #endregion
            #endregion

            #region 构造函数参数 （运用递归进行无限依赖）
            List<object> objList = new List<object>();
            object[] objlist = this.paraList.ContainsKey(key)? this.paraList[key]:null;
            int index = 0;
            foreach (var item in ctorinfo.GetParameters())
            {
                if (item.IsDefined(typeof(ParamterAttribute),true))
                {
                    objList.Add(objlist[index]);
                    index++;
                }
                else
                {
                    Type type1 = item.ParameterType;
                    if (item.IsDefined(typeof(NickNameAttribute), true))
                    {
                        string name = this.GetNickName(item);
                    }
                    object objtype = this.Resolve(type1);
                    objList.Add(objtype);
                }
               
            }
            object obj = new object();
            obj = Activator.CreateInstance(type, objList.ToArray());
            #endregion


            #region 属性注入 
            foreach (var item in type.GetProperties().Where(u => u.IsDefined(typeof(ContainerProptry), true)))
            {
                Type propType = item.PropertyType;
                object propIntance = this.Resolve(propType);
                item.SetValue(propIntance, propType);
            }
            #endregion
            #region 方法注入
            foreach (var item in type.GetMethods().Where(u => u.IsDefined(typeof(IOCMethodsAttribute))))
            {
                List<object> paraList = new List<object>();
                #region 获取方法的参数并使用递归获取参数实例
                foreach (var item1 in item.GetParameters())
                {
                    Type methodpartype = item1.ParameterType;
                    object objtype = this.Resolve(methodpartype);
                    paraList.Add(objtype);

                }
                #endregion
                item.Invoke(obj, paraList.ToArray());
            }
            #endregion
            return obj;
        }


        /// <summary>
        /// 获取抽象的类
        /// 
        /// </summary>
        /// <typeparam name="TIntance"></typeparam>
        /// <returns></returns>
        public TIntance Resolve<TIntance>()
        {
            return (TIntance)this.Resolve(typeof(TIntance));
        }
        /// <summary>
        /// 运用递归来依赖细节
        /// </summary>
        /// <param name="typeIntance"></param>
        /// <returns></returns>
        public object Resolve(Type typeIntance)
        {
            string key = typeIntance.FullName;
            Type type = dicTypes[key];

            #region 选择合适的构造函数
            ConstructorInfo ctorinfo = null;
           
            //2、标记特性  标记指定的类
            ctorinfo = type.GetConstructors().FirstOrDefault(c => c.IsDefined(typeof(ContainerAttribute), true));
            if (ctorinfo==null)
            {
                //1、选择参数个数最多的
                ctorinfo = type.GetConstructors().OrderByDescending(u => u.GetParameters().Length).FirstOrDefault();
            }
            #endregion
            #region 构造函数参数 （运用递归进行无限依赖）
            List<object> objList = new List<object>();
            //首先得判断常量是否有数据  如果有就返回   没有就返回null
            object[] oList = this.paraList.ContainsKey(key)? this.paraList[key]:null;
            int index = 0;
            foreach (var item in ctorinfo.GetParameters())
            {
                if (item.IsDefined(typeof(ParamterAttribute),true))
                {
                    objList.Add(oList[index]);
                    index++;
                }
                else
                {
                    Type type1 = item.ParameterType;
                    object objtype = this.Resolve(type1);
                    objList.Add(objtype);
                }
               
            }
            object obj = new object();
            obj = Activator.CreateInstance(type, objList.ToArray()); 
            #endregion


            #region 属性注入 
            foreach (var item in type.GetProperties().Where(u=>u.IsDefined(typeof(ContainerProptry), true)))
            {
                Type propType = item.PropertyType;
                object propIntance = this.Resolve(propType);
                item.SetValue(propIntance, propType);
            }
            #endregion
            #region 方法注入
            foreach (var item in type.GetMethods().Where(u=>u.IsDefined(typeof(IOCMethodsAttribute))))
            {
                List<object> paraList = new List<object>();
                #region 获取方法的参数并使用递归获取参数实例
                foreach (var item1 in item.GetParameters())
                {
                    Type methodpartype = item1.ParameterType;
                    string NickName = this.GetNickName(item1);
                    object objtype = new object();
                    if (NickName!=null)
                    {
                        objtype = this.Resolve(methodpartype, Name: NickName);
                    }
                    else
                    {
                         objtype = this.Resolve(methodpartype);
                    }
                    paraList.Add(objtype);

                } 
                #endregion
                item.Invoke(obj, paraList.ToArray());
            }
            #endregion
            return obj;
        }
        /// <summary>
        /// 获取类的别名
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private string GetNickName(ParameterInfo parameter)
        {
            if (parameter.IsDefined(typeof(NickNameAttribute),true))
            {
                return parameter.GetCustomAttribute<NickNameAttribute>().NickName;
            }
            else
            {
                return null;
            }
        }
    }
}
