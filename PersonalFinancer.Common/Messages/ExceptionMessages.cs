namespace PersonalFinancer.Common.Messages
{
    public static class ExceptionMessages
    {
        public const string UnauthorizedUser = "The user is not authorized.";

        public const string EntityDoesNotExist = "Entity does not exist.";

        public const string ExistingEntityName = "Entity with the same name exist.";

        public const string ExistingUserEntityName = "You already have {0} with name: {1}.";

        public const string AdminExistingUserEntityName = "The user already have {0} with name: {1}.";

        public const string EditInitialTransaction = "Cannot edit initial transaction.";

        public const string InvalidAccountType = "Invalid Account Type.";

        public const string InvalidCurrency = "Invalid Currency.";

        public const string InvalidCategory = "Invalid Category.";

        public const string UnsuccessfulDelete = "Delete was unsuccessful.";

        public const string NotNullableProperty = "{0} cannot be null.";
    }
}
