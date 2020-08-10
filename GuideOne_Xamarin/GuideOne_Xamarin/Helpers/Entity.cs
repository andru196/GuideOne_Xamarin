using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GuideOne_Xamarin.Helpers
{
	public abstract class Entity
	{
		[PrimaryKey]
		public virtual int Id { get; set; }
		[NotNull]
		public virtual DateTime dt { get; set; }
		public Entity()
		{
			dt = DateTime.UtcNow;
		}
	}
}
