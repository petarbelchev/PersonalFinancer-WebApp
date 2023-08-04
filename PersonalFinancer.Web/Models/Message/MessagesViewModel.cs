namespace PersonalFinancer.Web.Models.Message
{
	using PersonalFinancer.Services.Messages.Models;
	using PersonalFinancer.Web.Models.Shared;
	using static PersonalFinancer.Common.Constants.PaginationConstants;

	public class MessagesViewModel
	{
        public MessagesViewModel(MessagesDTO messagesDTO, int page)
        {
            this.Messages = messagesDTO.Messages;

            this.Pagination = new PaginationModel(
                MessagesName, 
				MessagesPerPage, 
				messagesDTO.TotalMessagesCount, 
				page);
        }

        public IEnumerable<MessageOutputDTO> Messages { get; set; }

        public PaginationModel Pagination { get; set; }
    }
}
