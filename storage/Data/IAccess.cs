using System.Collections.Generic;
namespace storage.Data;

public interface IAccess<T>
{
    void Save(T element);
    void Update(T element);
    List<T> GetAll();
    void Delete(int id);
}
