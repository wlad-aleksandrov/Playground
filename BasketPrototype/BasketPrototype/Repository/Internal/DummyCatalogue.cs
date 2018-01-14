using Aleksandrov.Repository.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Aleksandrov.Inventory {
    public sealed class DummyCatalogue : ICatalogue {
        private readonly ConcurrentDictionary<string, CatalogueItem> _itemsById;
        private readonly ConcurrentDictionary<string, CatalogueItem> _itemsByName;

        public DummyCatalogue() {
            _itemsById = new ConcurrentDictionary<string, CatalogueItem>();
            _itemsByName = new ConcurrentDictionary<string, CatalogueItem>();
        }

        public void Add(string name, string description, string picture, decimal price) {
            var id = Guid.NewGuid().ToString();
            var item = new CatalogueItem(id, name, description, picture, price);
            if (!_itemsByName.TryAdd(name, item)) {
                throw new ArgumentException(string.Format(Resources.Error_ItemAlreadyExists, name), nameof(name));
            }
            _itemsById.TryAdd(id, item);
        }

        public bool Exists(string id) => _itemsById.ContainsKey(id);

        public CatalogueItem Get(string id) {
            return _itemsById.TryGetValue(id, out var item) ? item : null;
        }

        public IEnumerable<CatalogueItem> GetAll() {
            return _itemsById.Values;
        }

        public int GetCount() => _itemsById.Keys.Count;

        public bool Remove(string id) => _itemsById.Remove(id, out var _);

        public IEnumerable<CatalogueItem> Search(string searchPattern) {
            return _itemsById.Values
                .Where(item => item.Name.StartsWith(searchPattern, StringComparison.CurrentCultureIgnoreCase)
                || item.Description.StartsWith(searchPattern, StringComparison.CurrentCultureIgnoreCase));
        }

    }
}