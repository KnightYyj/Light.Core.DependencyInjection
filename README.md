轻量级，启动时通过反射程序集一次性注入：

提供了2种方式：

​	1使用特性 AppServiceAttribute(支持Singleton，Scoped，Transient)

​	2根据命名空间批量注入LightDIExtension(目前只有单例)

目前组将使用的是AppServiceAttribute注入方式



1在需要注入的类上打上特性

 [AppService]
 public class UserService

2在Startup中：添加需要注入的类库
		services.AddAppServices("TESTWebApi.facade");
       

3在controller 或者service等层 实现使用特性注入

​	    [Autowired]
​        public UserService uService;//特性注入

        public ValuesController(LightAutowiredService autowiredService)
        {
            autowiredService.Autowired(this);
        }