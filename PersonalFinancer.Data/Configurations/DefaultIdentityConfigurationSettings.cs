namespace PersonalFinancer.Data.Configurations
{
	public class DefaultIdentityConfigurationSettings
	{
        public bool RequireConfirmedAccount { get; set; }

		public bool RequireNonAlphanumeric { get; set; }

		public bool RequireUniqueEmail { get; set; }

        public bool RequireUppercase { get; set; }
    }
}
