using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper;

        public AuctionCreatedConsumer(IMapper mapper)
        {
            _mapper = mapper;
            
        }

        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine($"--> Consuming auction created: {context.Message.Id}");

            Item item = _mapper.Map<Item>(context.Message);

            await item.SaveAsync();
        }
    }
}