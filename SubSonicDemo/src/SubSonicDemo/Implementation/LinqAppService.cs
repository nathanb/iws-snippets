using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubSonicDemo
{

	/// <summary>
	/// This interfacae could be implementd by more than just SubSonic. Consider ANY
	/// </summary>
	public class LinqAppService : AppService
	{
		LinqRepository repo;

		public LinqAppService(LinqRepository repo)
		{
			this.repo = repo;
		}

		public List<Category> GetAllCategories()
		{
			return repo.All<Category>().ToList();
		}

		public Category GetCategoryById(int id)
		{
			return repo.All<Category>().Where(o => o.Id == id).FirstOrDefault();
		}

		public int CountCategories()
		{
			return repo.All<Category>().Count();
		}

		public void SaveCategory(Category data)
		{
			if (data.Id == 0)
				repo.Add<Category>(data);
		}

		public List<Category> GetCategoryByParentAndName(int id, string name)
		{
			//this isn't a useful query, but it shows linq using more than one parameter. 
			return repo.All<Category>().Where(o => o.ParentId == id && o.Name == name).ToList();
		}

		public void DeleteCategory(int id)
		{
			repo.Delete<Category>(id);
		}

		public List<Product> GetProductsByCategory(int categoryId)
		{
			return repo.All<Product>()
				.Where(o => o.CategoryId == categoryId)
				.ToList();

			//could also use traditional linq
			//return (from product in repo.All<Product>()
			//        where product.CategoryId == categoryId
			//        select product).ToList();
		}
	}
}
