using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubSonicDemo
{
	public interface AppService
	{
		int CountCategories();
		List<Category> GetAllCategories();
		Category GetCategoryById(int id);
		List<Category> GetCategoryByParentAndName(int id, string name);
		void SaveCategory(Category data);
		void DeleteCategory(int id);

		List<Product> GetProductsByCategory(int p);
	}
}
