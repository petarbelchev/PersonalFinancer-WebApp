namespace PersonalFinancer.Tests.Mocks
{
    using AutoMapper;
    using PersonalFinancer.Web;

    static class ControllersMapperMock
	{
		public static IMapper Instance
		{
			get
			{
				var config = new MapperConfiguration(cfg => cfg.AddProfile<ControllerMappingProfile>());

				return config.CreateMapper();
			}
		}
	}
}
