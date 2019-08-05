using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Light.Core.DependencyInjection
{
    /// <summary>
    /// 注入扩展类的，自动反射注册ServiceCollection
    /// </summary>
    public static class LightDIExtension
    {
        /// <summary>
        /// 根据命名空间批量注入
        /// </summary>
        /// <param name="serColl"></param>
        /// <param name="assemyName"></param>
        public static void SingAdd(this IServiceCollection serColl, string assemyName)
        {
            if (string.IsNullOrEmpty(assemyName)) return;
            Assembly assembly = Assembly.Load(assemyName);
            var cls = GetClassName(assemyName);
            foreach (var item in cls)
            {
                //类没有实现接口的情况,类自己映射自己
                if (item.Value.Count == 0)
                {
                    serColl.AddSingleton(item.Key, item.Key);
                }
                else
                {
                    //继承了接口这种，类也自己映射自己一次，不然只能通过接口拿
                    serColl.AddSingleton(item.Key, item.Key);
                    //类实现了接口的情况，继承的所有接口都创建一个类的映射
                    foreach (var typeArray in item.Value)
                    {
                        serColl.AddSingleton(typeArray, item.Key);
                    }
                }
            }
        }

        public static void ScopedAdd(this IServiceCollection services, string assemyName)
        {
            if (string.IsNullOrEmpty(assemyName)) return;
            Assembly assembly = Assembly.Load(assemyName);
            //集中注册服务
            foreach (var item in GetClassName(assemyName))
            {
                foreach (var typeArray in item.Value)
                {
                    services.AddScoped(typeArray, item.Key);
                }
            }
        }
        
        /// <summary>
        /// 根据程序集反射类和类集成的接口
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private static Dictionary<Type, List<Type>> GetClassName(string assemblyName)
        {
            var result = new Dictionary<Type, List<Type>>();
            if (string.IsNullOrEmpty(assemblyName))
            {
                return result;
            }
            //根据命名空间加载程序集
            Assembly assembly = Assembly.Load(assemblyName);
            //获取程序集下面所有的类型
            IEnumerable<Type> types = assembly.GetTypes().ToList().Where(s => !s.IsInterface && !s.IsAbstract);
            //过滤接口和抽象类
            foreach (var item in types)
            {
                //当前类实现的接口类型，用于接口和实现类的映射
                List<Type> interTypes = new List<Type>();
                //接口中过滤一些System.IDispose等 无法创建映射的系统接口
                foreach (var tp in item.GetInterfaces())
                {
                    if (!(tp.Namespace.StartsWith("System") || tp.Namespace.StartsWith("Microsoft")))
                    {
                        interTypes.Add(tp);
                    }
                }
                //如果当前的类没有命名空间的也过滤掉，一般是一些无用的系统类
                if (!string.IsNullOrEmpty(item.Namespace))
                {
                    result.Add(item, interTypes);
                }
            }
            return result;
        }
    }
}
