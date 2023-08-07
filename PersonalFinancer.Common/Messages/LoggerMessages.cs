namespace PersonalFinancer.Common.Messages
{
	public static class LoggerMessages
	{
		// Accounts
		public const string CreateAccountWithAnotherUserId = "User with ID: \"{0}\", tried to create an account with another user ID: \"{1}\".";
		public const string DeleteAccountWithInvalidInputData = "User with ID: \"{0}\", tried to delete an account with invalid input data.";
		public const string GetAccountsInfoWithInvalidInputData = "Admin with ID: \"{0}\", tried to get an accounts info with invalid input data.";
		public const string GetAccountDetailsWithInvalidInputData = "User with ID: \"{0}\", tried to get an account details with invalid input data.";
		public const string GetAccountTransactionsWithInvalidInputData = "User with ID: \"{0}\", tried to get an account transactions with invalid input data.";
		public const string EditAccountWithInvalidInputData = "User with ID: \"{0}\", tried to edit an account with invalid input data.";
		public const string UnauthorizedAccountDeletion = "Unauthorized user with ID: \"{0}\", tried to delete an account with ID: \"{1}\".";
		public const string UnauthorizedGetAccountDetails = "Unauthorized user with ID: \"{0}\", tried to get an account details with ID: \"{1}\".";
		public const string UnauthorizedGetAccountTransactions = "Unauthorized user with ID: \"{0}\", tried to get an account transactions with ID: \"{1}\".";
		public const string UnauthorizedAccountEdit = "Unauthorized user with ID: \"{0}\", tried to edit an account with ID: \"{1}\".";

		// Transactions
		public const string CreateTransactionWithAnotherUserId = "User with ID: \"{0}\", tried to create a transaction with another user ID: \"{1}\".";
		public const string CreateTransactionWithInvalidInputData = "User with ID: \"{0}\", tried to create a transaction with invalid input data.";
		public const string DeleteTransactionWithInvalidInputData = "User with ID: \"{0}\", tried to delete a transaction with invalid input data.";
		public const string GetTransactionDetailsWithInvalidInputData = "User with ID: \"{0}\", tried to get a transaction details with invalid input data.";
		public const string EditTransactionWithInvalidInputData = "User with ID: \"{0}\", tried to edit a transaction with invalid input data.";
		public const string UnauthorizedTransactionDeletion = "Unauthorized user with ID: \"{0}\", tried to delete a transaction with ID: \"{1}\".";
		public const string UnauthorizedGetTransactionDetails = "Unauthorized user with ID: \"{0}\", tried to get a transaction details with ID: \"{1}\".";
		public const string UnauthorizedTransactionEdit = "Unauthorized user with ID: \"{0}\", tried to edit a transaction with ID: \"{1}\".";

		// Messages
		public const string AddReplyWithInvalidInputData = "User with ID: \"{0}\", tried to add a reply with invalid input data.";
		public const string ArchiveMessageWithInvalidInputData = "User with ID: \"{0}\", tried to archive a message with invalid input data.";
		public const string DeleteMessageWithInvalidInputData = "User with ID: \"{0}\", tried to delete a message with invalid input data.";
		public const string GetMessagesInfoWithInvalidInputData = "Admin with ID: \"{0}\", tried to get a messages info with invalid input data.";
		public const string GetMessageDetailsWithInvalidInputData = "User with ID: \"{0}\", tried to get a message details with invalid input data.";
		public const string MarkAsSeenWithInvalidInputData = "User with ID: \"{0}\", tried to mark a message as seen with invalid input data.";
		public const string UnauthorizedGetMessageDetails = "Unauthorized user with ID: \"{0}\", tried to get a message details with ID: \"{1}\".";
		public const string UnauthorizedArchiveMessage = "Unauthorized user with ID: \"{0}\", tried to archive a message with ID: \"{1}\".";
		public const string UnauthorizedMessageDeletion = "Unauthorized user with ID: \"{0}\", tried to delete a message with ID: \"{1}\".";
		public const string UnauthorizedReplyAddition = "Unauthorized user with ID: \"{0}\", tried to add a reply to message with ID: \"{1}\".";
		public const string UnsuccessfulMessageArchiving = "User with ID: \"{0}\", unsuccessfully tried to archive a message with ID: \"{1}\".";
		public const string UnsuccessfulMessageDeletion = "User with ID: \"{0}\", unsuccessfully tried to delete a message with ID: \"{1}\".";
		public const string UnsuccessfulReplyAddition = "User with ID: \"{0}\", unsuccessfully tried to add a reply to a message with ID: \"{1}\".";
		public const string UnsuccessfulMarkMessageAsSeen = "User with ID: \"{0}\", unsuccessfully tried to mark a message as seen with ID: \"{1}\".";

		// Users
		public const string GetUsersInfoWithInvalidInputData = "Admin with ID: \"{0}\", tried to get a users info with invalid input data.";
		public const string GetUserDetailsWithInvalidInputData = "Admin with ID: \"{0}\", tried to get a user details with invalid input data.";
		public const string GetUserTransactionsWithInvalidInputData = "User with ID: \"{0}\", tried to get his transactions with invalid input data.";
		public const string UnauthorizedGetUserTransactions = "Unauthorized user with ID: \"{0}\", tried to get an user transactions with ID: \"{1}\".";

		// Account Types, Currencies, Categories
		public const string CreateEntityWithInvalidInputData = "User with ID: \"{0}\", tried to create a {1} with invalid input data.";
		public const string DeleteEntityWithInvalidInputData = "User with ID: \"{0}\", tried to delete a {1} with invalid input data.";
		public const string UnauthorizedEntityDeletion = "Unauthorized user with ID: \"{0}\", tried to delete a {1} with ID: \"{2}\".";
	}
}
