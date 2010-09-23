using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.SqlGeneration.Schema;

namespace SubSonicDemo.Core
{
	//not using table name override. 
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int CategoryId { get; set; }
		[SubSonicLongString]
		[SubSonicNullString]
		public string LongDescription { get; set; }
		public decimal MSRP { get; set; }
	}
}
