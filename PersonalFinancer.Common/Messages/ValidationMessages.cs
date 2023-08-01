namespace PersonalFinancer.Common.Messages
{
    public static class ValidationMessages
    {
        public const string InvalidLength = "The \"{0}\" must be between {2} and {1} characters long.";

        public const string InvalidNumericLength = "The \"{0}\" must be a number between {1} and {2}";

        public const string InvalidPhoneNumberLength = "The \"{0}\" must be 10 digits.";

        public const string CompareDoNotMatch = "The \"{0}\" do not match.";

        public const string RequiredProperty = "Please enter \"{0}\". It's required.";

        public const string InvalidEmailAddress = "Please enter a valid email address.";

        public const string InvalidDate = "Please enter a valid date.";

		public const string MessageImageConstraints = "must be a JPEG or PNG file with maximum of 200 KB";

		public const string InvalidImageFileType = "Please choose a valid image file (JPEG or PNG).";

		public const string InvalidImageSize = "The image must not exceed 200 KB.";
	}
}
