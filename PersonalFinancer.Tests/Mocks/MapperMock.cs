using AutoMapper;

using PersonalFinancer.Web.Infrastructure;

namespace PersonalFinancer.Tests.Mocks
{
	static class MapperMock
	{
		public static IMapper Instance
		{
			get
			{
				var config = new MapperConfiguration(cfg => cfg.AddProfile<ViewModelsMappingProfile>());

				return config.CreateMapper();
			}
		}
	}
}
