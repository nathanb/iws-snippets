using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.Repository;

namespace SubSonicDemo
{
	public class SS3LinqRepository : LinqRepository
	{
		SubSonic.Repository.SimpleRepository repo;
		public SS3LinqRepository()
		{
			var migrations = SimpleRepositoryOptions.Default;
			if (Config.RunMigrations)
				migrations = SimpleRepositoryOptions.RunMigrations;
			else
				migrations = SimpleRepositoryOptions.None;

			repo = new SubSonic.Repository.SimpleRepository(Config.DefaultConnectionStringName, migrations); //no migrations.

		}
		#region LinqRepository Members

		public IQueryable<T> All<T>() where T : class, new()
		{
			return repo.All<T>();
		}

		public void Add<T>(T data) where T : class, new()
		{
			repo.Add<T>(data);
		}
		public void Add(object data)
		{
			repo.Add(data);
		}

		public void Update<T>(T data) where T : class, new()
		{
			repo.Update<T>(data);
		}

		public void Delete<T>(object key) where T : class, new()
		{
			repo.Delete<T>(key);
		}

		public void DeleteMany<T>(IEnumerable<T> items) where T : class, new()
		{
			repo.DeleteMany<T>(items);
		}

		public void UpdateMany<T>(IEnumerable<T> items) where T : class, new()
		{
			repo.UpdateMany<T>(items);
		}

		public void AddMany<T>(IEnumerable<T> items) where T : class, new()
		{
			repo.AddMany<T>(items);
		}

		#endregion
	}
}
