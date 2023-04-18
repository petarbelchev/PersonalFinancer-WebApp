namespace PersonalFinancer.Tests.Mocks
{
	using AutoMapper;

	using PersonalFinancer.Services.Infrastructure;

	static class MapperMock
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
