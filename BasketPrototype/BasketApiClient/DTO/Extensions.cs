using System;
using System.Collections.Generic;
using System.Text;

namespace BasketApiClient.DTO {
    public static class Extensions {

        public static Uri ToUri(this string uri) => new Uri(uri);
    }
}
