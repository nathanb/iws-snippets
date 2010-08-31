using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubSonicDemo
{
	public class DemoController
	{
		//local copy of the injected service implementation
		AppService service;

		//setup only one constructor containing the interfaces you want to use. Unity will inject a resolved version of AppService into the parameter value where
		// you can take that and store it off at member level for future use within the object. 
		public DemoController(AppService svc)
		{
			this.service = svc;
		}

		public void Start()
		{
			Console.WriteLine("Beginning test...");
			Console.WriteLine("Get all categories");
			var list = service.GetAllCategories();

			if (list.Count > 0)
			{
				Console.WriteLine("Deleting existing entries");

				foreach (var item in list)
				{
					service.DeleteCategory(item.Id); //there is also a deleteMany option in SubSonic that can be used instead for better performance onbulk deletes. 
				}
			}

			Console.WriteLine("inserting one");
			var data = new Category() { Description = "test description", Name = "Test Product", ParentId = 1, Hide = false };
			service.SaveCategory(data); //inserting

			Console.WriteLine("Updating it");
			data.Name += " 2"; //changing data.
			service.SaveCategory(data); //updating 

			Console.WriteLine("Reselecting the first one");
			var first = service.GetAllCategories().FirstOrDefault(); //get one
			Console.WriteLine("Repository Mode: " + (first == null ? "none found" : first.Name));

			Console.WriteLine("Getting by parentid and name");
			var found = service.GetCategoryByParentAndName(1, "test");
			Console.WriteLine("found " + found.Count.ToString());

			Console.WriteLine("looking for products in this category");
			var products = service.GetProductsByCategory(data.Id);

			service.DeleteCategory(first.Id); //delete it.
			Console.WriteLine("Cleaned up");
		}
	}
}
