using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Light.Core.DependencyInjection
{
    public static class AppServiceExtensions
    {
        /// <summary>
        /// 注册应用程序域中所有有AppService特性的类
        /// </summary>
        /// <param name="services"></param>
        public static void AddAppServices(this IServiceCollection services,string assemyName)
        {
            //foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())//获取不到全部
            //Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            services.AddSingleton<LightAutowiredService, LightAutowiredService>();//测试注入
            foreach (var type in GetClassName(assemyName))
            {
                //取类上的自定义特性
                object[] objAttrs = type.GetCustomAttributes(typeof(AppServiceAttribute), true);
                foreach (var obj in objAttrs)
                {
                    AppServiceAttribute serviceAttribute = obj as AppServiceAttribute;
                    if (serviceAttribute != null)
                    {
                        var serviceType = serviceAttribute.ServiceType;
                        //类型检查,如果 type 不是 serviceType 的实现或子类或本身
                        //运行时 type 将无法解析为 serviceType 的实例
                        if (serviceType != null && !serviceType.IsAssignableFrom(type))
                        {
                            serviceType = null;
                        }
                        if (serviceType == null && serviceAttribute.InterfaceServiceType)
                        {
                            serviceType = type.GetInterfaces().FirstOrDefault();
                        }
                        if (serviceType == null)
                        {
                            serviceType = type;
                        }
                        switch (serviceAttribute.Lifetime)
                        {
                            case ServiceLifetime.Singleton:
                                services.AddSingleton(serviceType, type);
                                break;
                            case ServiceLifetime.Scoped:
                                services.AddScoped(serviceType, type);
                                break;
                            case ServiceLifetime.Transient:
                                services.AddTransient(serviceType, type);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }



        private static List<Type> GetClassName(string assemblyName)
        {
            //当前类实现的接口类型，用于接口和实现类的映射
            List<Type> interTypes = new List<Type>();
            if (string.IsNullOrEmpty(assemblyName))
            {
                return interTypes;
            }
            //根据命名空间加载程序集
            Assembly assembly = Assembly.Load(assemblyName);
            //获取程序集下面所有的类型
            IEnumerable<Type> types = assembly.GetTypes().ToList().Where(s => !s.IsInterface && !s.IsAbstract);
            //过滤接口和抽象类
            foreach (var item in types)
            {
                interTypes.Add(item);
            }
            return interTypes;
        }
    }
}
