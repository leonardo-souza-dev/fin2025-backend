using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Application.UseCases
{
    public class UpdateConfigUseCase(
        IConfigRepository configRepository,
        IUnitOfWork unitOfWork
        )
    {
        public void Handle(UpdateConfigRequest request)
        {
            var actualConfig = configRepository.Get(request.Id);
            if (actualConfig == null)
            {
                throw new InvalidOperationException("Config not found.");
            }

            if (actualConfig.Key != request.Key)
            {
                throw new InvalidOperationException("Configs must have same key.");
            }
            
            actualConfig.Value = request.Value;
            unitOfWork.SaveChanges();
        }
    }

    public class UpdateConfigRequest
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}