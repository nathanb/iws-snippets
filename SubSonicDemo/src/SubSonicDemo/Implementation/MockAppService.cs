using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubSonicDemo
{
	/// <summary>
	/// this is just a placeholder class to show you could setup unit testing at the service level. 
	/// </summary>
	public class MockAppService : AppService
	{
		#region AppService Members

		public int CountCategories()
		{
			throw new NotImplementedException();
		}

		public List<Category> GetAllCategories()
		{
			throw new NotImplementedException();
		}

		public Category GetCategoryById(int id)
		{
			throw new NotImplementedException();
		}

		public List<Category> GetCategoryByParentAndName(int id, string name)
		{
			throw new NotImplementedException();
		}

		public void SaveCategory(Category data)
		{
			throw new NotImplementedException();
		}

		public void DeleteCategory(int id)
		{
			throw new NotImplementedException();
		}

		public List<Product> GetProductsByCategory(int p)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
