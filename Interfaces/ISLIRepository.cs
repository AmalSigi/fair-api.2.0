using FairMount_api.Models.Dtos;
using FairMount_api.Models.Tables;

public interface ISLIRepository
{
    Task<IEnumerable<Sli_Document>> GetAllAsync();
    Task<Sli_Document?> GetByIdAsync(int id);
    Task<Sli_Document> CreateAsync(Sli_Document sli);
    Task<Sli_Document?> GetByComercialIdAsync(int id);
}