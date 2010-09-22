using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubSonicDemo
{

	/// <summary>
	/// This is antoher mock class to show that we can setup mocking the core data level.  This layer can typically be replaced with other frameworks like EF4 or Nhibernate
	/// </summary>
	public class MockLinqRepository : LinqRepository
	{

		#region LinqRepository Members

		public IQueryable<T> All<T>() where T : class, new()
		{
			throw new NotImplementedException();
		}

		public void Add<T>(T data) where T : class, new()
		{
			throw new NotImplementedException();
		}

		public void Add(object data)
		{
			throw new NotImplementedException();
		}

		public void AddMany<T>(IEnumerable<T> list) where T : class, new()
		{
			throw new NotImplementedException();
		}

		public void Update<T>(T data) where T : class, new()
		{
			throw new NotImplementedException();
		}

		public void Delete<T>(object key) where T : class, new()
		{
			throw new NotImplementedException();
		}

		public void DeleteMany<T>(IEnumerable<T> list) where T : class, new()
		{
			throw new NotImplementedException();
		}

		public void UpdateMany<T>(IEnumerable<T> list) where T : class, new()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
