namespace PersonalFinancer.Web
{
    public static class Constants
    {
        public static class HostConstants
        {
            public const string ApiCategoriesUrl = "/api/categories/";
            public const string ApiTransactionsUrl = "/api/transactions/";
            public const string ApiAccountTypesUrl = "/api/accounttypes/";
            public const string ApiCurrencyUrl = "/api/currencies/";
            public const string ApiAccountsUrl = "/api/accounts/";
            public const string ApiAccountsCashFlowUrl = "/api/accounts/cashflow";
            public const string ApiAccountTransactionsUrl = "/api/accounts/transactions";
            public const string ApiUsersUrl = "/api/users/";

            public const string BadRequestImgUrl = "/images/400BadRequest.webp";
            public const string InternalServerErrorImgUrl = "/images/500InternalServerError.webp";
            public const string NotFoundImgUrl = "/images/404NotFound.webp";
        }
    }
}
