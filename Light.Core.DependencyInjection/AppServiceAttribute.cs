﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace Light.Core.DependencyInjection
{
    /// <summary>
    /// 标记服务
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class AppServiceAttribute : Attribute
    {
        /// <summary>
        /// 生命周期
        /// </summary>
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;

        /// <summary>
        /// 指定服务类型
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// 是否可以从第一个接口获取服务类型
        /// </summary>
        public bool InterfaceServiceType { get; set; } = true;

        /// <summary>
        /// 服务(实现)唯一标识
        /// </summary>
        public string Identifier { get; set; }

        public AppServiceAttribute()
        {
        }

        public AppServiceAttribute(Type serviceType) : this(serviceType, ServiceLifetime.Singleton, null, false)
        {
        }

        public AppServiceAttribute(ServiceLifetime serviceLifetime) : this(null, serviceLifetime, null, true)
        {
        }

        public AppServiceAttribute(string identifier) : this(null, ServiceLifetime.Singleton, identifier, true)
        {
        }

        private AppServiceAttribute(Type serviceType, ServiceLifetime serviceLifetime, string identifier,
            bool interfaceServiceType)
        {
            ServiceType = serviceType;
            Lifetime = serviceLifetime;
            Identifier = identifier;
            InterfaceServiceType = interfaceServiceType;
        }
    }
}
