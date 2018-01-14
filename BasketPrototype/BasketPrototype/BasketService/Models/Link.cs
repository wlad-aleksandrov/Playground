using System;

namespace Aleksandrov.BasketService.Models {
    public sealed class Link {
        public string Rel { get; }
        public string Href { get; }

        public Link(string rel, string href) {
            Rel = rel;
            Href = href;
        }

        public static implicit operator Link((string rel, Uri uri) tuple) {
            return new Link(tuple.rel, tuple.uri.ToString());
        }
    }
}