using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApplication1.IOC;

namespace WebApplication1
{
    public class Container1
    {
        private Dictionary<string, Type> dicType = new Dictionary<string, Type>();
        private Dictionary<string ,object[]> paraList = new Dictionary<string, object[]>();
        public string getKey(Type type,string Name)
        {
            return Name==null?type.FullName:type.FullName + "_" + Name;
        }
        /// <summary>
        /// 注册服务的方法
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="NickName"></param>
        /// <param name="objList"></param>
        public void Register<TService,TClass>( string NickName=null,object[] objList=null )where TClass:TService
        {
            string key = NickName==null?typeof(TService).FullName: getKey(typeof(TService), NickName);
            dicType.Add(key, typeof(TClass));
            if (objList!=null&&objList.Length>0)
            {
                paraList.Add(key, objList);
            }
        }
        public TService Revole<TService>(string name = null)
        {
            return (TService)this.Revole(typeof(TService), name);
        }
        public object Revole(Type type,string name)
        {
            string key = name == null ? type.FullName : this.getKey(type, name);
            Type type1 = this.dicType[key];

            ConstructorInfo ctor = null;
            var ctorlist = type1.GetConstructors().Where(u => u.IsDefined(typeof(ContainerAttribute), true));
            if (ctorlist!=null)
            {
                ctor = ctorlist.FirstOrDefault() ;
            }
              else
            {
                ctor = type1.GetConstructors().OrderByDescending(o => o.GetParameters().Length).FirstOrDefault();
            }
            List<object> arrList = new List<object>();
            object[] objlist = this.paraList.ContainsKey(key)?paraList[key] :null;
            int index = 0;
            foreach (var item in ctor.GetParameters())
            {
                
                if (item.IsDefined(typeof(ParamterAttribute),true))
                {
                    arrList.Add(objlist[index]);
                    index++;
                }
                else
                {
                    Type ctortype = item.ParameterType;
                    string Name = this.GetNickName(item);
                    object obj1 = this.Revole(ctortype, Name);
                    arrList.Add(obj1);
                }

            }
            object obj = new object();
            obj = Activator.CreateInstance(type1, arrList.ToArray());
            //属性注入
            foreach ( var item in type1.GetProperties().Where(u=>u.IsDefined(typeof(ContainerProptry),true)))
            {
                Type type2 = item.PropertyType;
                object o = this.Revole(type2, null);
                item.SetValue(o, type2);
            }
            //方法注入
            foreach (var item in type1.GetMethods().Where(u => u.IsDefined(typeof(IOCMethodsAttribute), true)))
            {
                foreach (var method in item.GetParameters())
                {
                    Type methodtype = method.ParameterType;
                    string NickName = this.GetNickName(method);
                    object intance = this.Revole(methodtype, NickName);
                    arrList.Add(intance);
                }
                item.Invoke(obj,arrList.ToArray());
            }
            return obj;
        }
        public string GetNickName(ParameterInfo type)
        {
            if (type.IsDefined(typeof(NickNameAttribute),true))
            {
                return type.GetCustomAttribute<NickNameAttribute>().NickName;
            }
            else
            {
                return null;
            }
        }
    }
}
