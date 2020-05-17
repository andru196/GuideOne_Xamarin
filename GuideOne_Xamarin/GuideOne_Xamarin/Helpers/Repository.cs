using GuideOne_Xamarin.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GuideOne_Xamarin.Helpers
{
	public class Repository
	{
		protected SQLiteConnection database;
		public Repository(string databasePath)
		{
			database = new SQLiteConnection(databasePath);
			//database.DropTable<User>();
			database.CreateTable<User>();
		}
		public T GetItem<T>(int id) where T : Entity, new()
		{
			return database.Get<T>(id);
		}
		public IEnumerable<T> GetItems<T>() where T: Entity, new()
		{
			return database.Table<T>().ToList();
		}
		public int DeleteItem<T>(int id) where T : Entity, new()
		{
			return database.Delete<T>(id);
		}

		public int SaveItem<T>(T item) where T : Entity, new()
		{
			if (database.Find<T>(item.Id) != null)
				return database.Update(item, typeof(T));
			else
				return database.Insert(item, typeof(T));
		}
		public IEnumerable<T> Query<T>(string query, params object[] prms) where T : Entity, new()
		{
			return database.Query<T>(query, prms);
		}

		public T Find<T>(string query, params object[] prms) where T : Entity, new()
		{
			return database.FindWithQuery<T>(query, prms);
		}

		public User FindUser()
		{
			return database.FindWithQuery<User>("SELECT * FROM " + typeof(User).Name + "s WHERE IsUser = ?", 1);
		}
#if DEBUG
		public List<Table> Tables
		{
			get
			{
				return database.Query<Table>("select * from sqlite_master where type = 'table'", "_");
			}
		}
		public class Table
		{
			public string type { get; set; }
			public string name { get; set; }
			public string tbl_name { get; set; }
			public int rootpage { get; set; }
			public string sql { get; set; }
		}
#endif
	}
}


