using System;

namespace BasketService.Utils {
    public sealed class LocalClock : IClock {
        public DateTimeOffset Now => DateTimeOffset.Now;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}