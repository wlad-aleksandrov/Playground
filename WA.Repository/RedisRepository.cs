using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WA.Repository
{
    public class RedisRepository : IRepository, IDisposable
    {
        readonly ConnectionMultiplexer _redis;
        readonly IDatabase _redisDatabase;
        bool _disposed;

        public RedisRepository() : this("localhost")
        {
        }

        public RedisRepository(string redisConfiguration)
        {
            _redis = ConnectionMultiplexer.Connect(redisConfiguration);
            // we use just one database
            _redisDatabase = _redis.GetDatabase(0);
        }
        public bool CloseAuction(string auctionId)
        {

            var deleted = _redisDatabase.KeyDelete($"{AuctionFields.Auction}:{auctionId}");
            if (deleted)
                _redisDatabase.SortedSetRemove(AuctionFields.OpenAuctions, auctionId);
            return deleted;
        }
        public Auction? GetAuction(string auctionId) =>
            _redisDatabase.HashGetAll($"{AuctionFields.Auction}:{auctionId}").ToDictionary().ToAuction();

        public string OpenAuction(string name, string description, string imageUrl, int startingPrice, int estimate)
        {
            var nextAuctionId = _redisDatabase.StringIncrement(AuctionFields.NextAuctionNumber);
            var auctionFields = new HashEntry[]
            {
                    new HashEntry(AuctionFields.Id, nextAuctionId),
                    new HashEntry(AuctionFields.Name, name),
                    new HashEntry(AuctionFields.Description, description),
                    new HashEntry(AuctionFields.ImageUrl, imageUrl),
                    new HashEntry(AuctionFields.StartingPrice, startingPrice),
                    new HashEntry(AuctionFields.Estimate, estimate)
                };
            _redisDatabase.HashSet($"{AuctionFields.Auction}:{nextAuctionId}", auctionFields);

            // add to the set of all open auctions

            _redisDatabase.SortedSetAdd(AuctionFields.OpenAuctions, nextAuctionId, nextAuctionId);
            return nextAuctionId.ToString();
        }

        public Auction? TryPlaceBid(string auctionId, string bidderId, int bidAmount)
        {
            // we are trying to place a bid.
            // A placement is successful, if the given bid is greater than the current bid in the repository
            // This needs to an atomic operation --> We use LUA Script which is guaranteed to run atomic on Redis
            // (No other Redis command is allowed to execute while a LUA Script is running)

            var result = _redisDatabase.ScriptEvaluate(Properties.Resources.Lua_TryPlaceBid,
                  new RedisKey[] { $"{AuctionFields.Auction}:{auctionId}" }, new RedisValue[] { bidderId, bidAmount });

            return result.IsNull ? null : ((string[])result).ToAuction();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_redis != null)
                    _redis.Dispose();
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool AuctionExists(string auctionId) => _redisDatabase.KeyExists($"{AuctionFields.Auction}:{auctionId}");

        public IList<Auction> GetAuctions() =>
            _redisDatabase.SortedSetRangeByScore(AuctionFields.OpenAuctions, double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending).
            Select(auctionId => GetAuction(auctionId)).
            Where(auction => auction.HasValue).Select(auctionNullable => auctionNullable.Value).ToList();
        public long GetCountOfOpenAuctions() => _redisDatabase.SortedSetLength(AuctionFields.OpenAuctions);

        public IList<Auction> GetAuctions(int offset, int limit)
        {
            var start = offset;
            var stop = offset + limit;

            return _redisDatabase.SortedSetRangeByScore(AuctionFields.OpenAuctions, start, stop, Exclude.Stop, Order.Descending).
              Select(auctionId => GetAuction(auctionId)).
              Where(auction => auction.HasValue).Select(auctionNullable => auctionNullable.Value).ToList();
        }
    }
}