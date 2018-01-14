using System.Collections.Generic;

namespace Aleksandrov.Inventory {
    public interface ICatalogue {
        void Add(string name, string description, string picture, decimal price);
        CatalogueItem Get(string id);
        int GetCount();
        bool Remove(string id);
        IEnumerable<CatalogueItem> GetAll();
        IEnumerable<CatalogueItem> Search(string searchPattern);
    }
}