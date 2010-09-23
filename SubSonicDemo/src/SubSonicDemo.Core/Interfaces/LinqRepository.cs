using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubSonicDemo.Core
{
	public interface LinqRepository
	{
		IQueryable<T> All<T>() where T : class, new();
		void Add<T>(T data) where T : class, new();
		void Add(object data);
		void AddMany<T>(IEnumerable<T> list) where T : class, new();
		void Update<T>(T data) where T : class, new();
		void Delete<T>(object key) where T : class, new();
		void DeleteMany<T>(IEnumerable<T> list) where T : class, new();
		void UpdateMany<T>(IEnumerable<T> list) where T : class, new();
	}
}
