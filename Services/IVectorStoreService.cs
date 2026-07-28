using MultiModalRagDemo.Models;

namespace MultiModalRagDemo.Services;

public interface IVectorStoreService
{
    Task<int> StoreVectorsAsync(List<VectorRecord> records);
}
