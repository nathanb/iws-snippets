using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using SubSonicDemo.Core;

namespace SubSonicDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			//setup Unity
			var container = new UnityContainer();
			container.RegisterType<LinqRepository, SS3LinqRepository>(); //uncomment for app.config configuration.
			container.RegisterType<AppService, LinqAppService>();

			//mock registers. uncomment to run mocking classes. (note: they're currently not implemented);
			//container.RegisterType<AppService, MockAppService>();
			//container.RegisterType<LinqRepository, MockLinqRepository>();

			//if you want to use app.config for unity mappings, comment out the RegisterType<>() lines, uncomment the following two lines, and the uncomnent the app.config section for unity.
			//UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
			//section.Configure(container);

			//Typically in MVC, you could replace the Controller factory with your own that constructs new instances of controllers on the fly.
			//As done here, your custom implementation of a controller factory can use Unity (or other injection framework) to resolve a new instance
			//  of the controller requested. Unity will automatically "inject" resolved interface parameters on the class being constructed.
			var demo = container.Resolve<DemoController>();
			demo.Start();
		}
	}
}
