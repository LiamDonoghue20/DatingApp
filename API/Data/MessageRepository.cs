using API.DTOs;
using API.Entities;
using API.helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            this._mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = await _context.Messages
            //we include the full entity to update the date read property of the messages as well as including the photos
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(
                    //get both the messages sent by the user and the message received by the user
                    //messages received
                    m => m.RecipientUsername == currentUserName && 
                    m.SenderUsername == recipientUserName ||
                    // OR messages sent
                    m.RecipientUsername == recipientUserName &&
                    m.SenderUsername == currentUserName
                )
                .OrderBy(m => m.DateSent)
                .ToListAsync();

                //unread messages for the user (where date read is equal to null and recipient name is current username)
                var unreadMessages = messages.Where(m => m.DateRead == null && 
                m.RecipientUsername == currentUserName).ToList();

                if(unreadMessages.Any())
                {
                    //loop over all unread messages
                    foreach (var message in unreadMessages)
                    {
                        //set the date read to be now
                        message.DateRead = DateTime.UtcNow;
                    }
                    //save changes back to database
                    await _context.SaveChangesAsync();
                }
                return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<PagedList<MessageDto>> GetMesssagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
            //get the latest message first
                .OrderByDescending(x => x.DateSent)
                .AsQueryable();

            //get the container from the message params to see what the user wants to view via the switch statement
            //inbox outbox or unread messages
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username),
                _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.DateRead == null)
            };
            //project that query to a message DTO to fill in the fields
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            //return it as a paged list with the current page number and page size for the purpose of pagination
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}