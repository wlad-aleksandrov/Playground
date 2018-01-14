using System;

namespace BasketService.Utils {
    public interface IClock {
        DateTimeOffset Now { get; }
        DateTimeOffset UtcNow { get; }
    }
}
