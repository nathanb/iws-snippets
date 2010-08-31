using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;

namespace SubSonicDemo
{
	//you may optionally specify a custom table name for your domain object. 
	//If you left this out, it would likely pluralize and pascal case the class name for its table representation
	[SubSonicTableNameOverride("sample_table")]
	public class Category
	{
		//Id is assumed primary key unless you specify otherwise using SubSonicPrimaryKey attribuye. 
		public int Id { get; set; }
		public int ParentId { get; set; }
		public bool Hide { get; set; }

		//use nullables for primitive types to make them translate to nullable database fields. 
		public DateTime? NullableDate { get; set; }
		public int? NullableInt { get; set; }

		//all strings are assumed not null. Use this attribute to indicate a nullable string. 
		[SubSonicNullString]
		//default string size is 255; you can manually specify your own length or use SubSonicLongString for Text fields. 
		[SubSonicStringLength(300)]
		public string Description { get; set; }
		public string Name { get; set; }
	}
}
