using AutoMapper;
using PersonalFinancer.Services.Infrastructure;

namespace PersonalFinancer.Tests.Mocks
{
	static class ServicesMapperMock
	{
		public static IMapper Instance
		{
			get
			{
				var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceMappingProfile>());

				return config.CreateMapper();
			}
		}
	}
}
