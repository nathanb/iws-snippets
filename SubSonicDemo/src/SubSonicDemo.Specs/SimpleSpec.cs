using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Rhino.Mocks;

namespace SubSonicDemo.Specs
{
	[Subject("Categories")]
	public class When_getting_a_list_of_all_categories
	{
		static Core.AppService service;
		static List<Core.Category> result;

		Establish context = () =>
		{
			service = MockRepository.GenerateMock<Core.AppService>();
			service.Stub(o => o.GetAllCategories()).Return(new List<Core.Category>() {
				new Core.Category() { Name="test1", Id=1},
				new Core.Category() { Name="test2", Id=2}
			});
		};

		Because of = () => { result = service.GetAllCategories(); };

		It Should_be_a_list_of_categories = () => { result.ShouldBeOfType<List<Core.Category>>(); };
		It Should_contain_two_categories = () => { result.Count.ShouldEqual(2); };
		It Should_do_something_else_not_implemented_yet;
		It Should_fail = () => { throw new Exception("kaboom!"); };
	}
}
