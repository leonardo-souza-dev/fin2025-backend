using Fin.Domain.Entities;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases.Configs
{
    public class CreateConfigUseCase(IConfigRepository repository, IUnitOfWork unitOfWork)
    {
        public CreateConfigResponse Handle(CreateConfigRequest request)
        {
            var configRequest = new Config
            {
                Key = request.Key,
                Value = request.Value
            };
            repository.Create(configRequest);
            unitOfWork.SaveChanges();
            
            return CreateConfigResponse.Of(configRequest);
        }
    }

    public class CreateConfigRequest
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
    }

    public class CreateConfigResponse
    {
        public required int Id { get; set; }
        public required string Key { get; set; }
        public required string Value { get; set; }

        public static CreateConfigResponse Of(Config config)
        {
            return new CreateConfigResponse
            {
                Id = config.Id,
                Key = config.Key,
                Value = config.Value
            };
        }
    }
}