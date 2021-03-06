﻿using BookSleeve;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudStructures.Redis
{
    public class RedisSet<T>
    {
        const string CallType = "RedisSet";

        public string Key { get; private set; }
        public RedisSettings Settings { get; private set; }

        public RedisSet(RedisSettings settings, string stringKey)
        {
            this.Settings = settings;
            this.Key = stringKey;
        }

        public RedisSet(RedisGroup connectionGroup, string stringKey)
            : this(connectionGroup.GetSettings(stringKey), stringKey)
        {
        }

        protected RedisConnection Connection
        {
            get
            {
                return Settings.GetConnection();
            }
        }

        protected ISetCommands Command
        {
            get
            {
                return Connection.Sets;
            }
        }

        /// <summary>
        /// SADD http://redis.io/commands/sadd
        /// </summary>
        public async Task<bool> Add(T value, bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                return await Command.Add(Settings.Db, Key, Settings.ValueConverter.Serialize(value), queueJump).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// SADD http://redis.io/commands/sadd
        /// </summary>
        public async Task<long> Add(T[] values, bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                var v = values.Select(x => Settings.ValueConverter.Serialize(x)).ToArray();
                return await Command.Add(Settings.Db, Key, v, queueJump).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// SISMEMBER http://redis.io/commands/sismember
        /// </summary>
        public async Task<bool> Contains(T value, bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                return await Command.Contains(Settings.Db, Key, Settings.ValueConverter.Serialize(value), queueJump).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// SMEMBERS http://redis.io/commands/smembers
        /// </summary>
        public async Task<T[]> GetAll(bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                var v = await Command.GetAll(Settings.Db, Key, queueJump).ConfigureAwait(false);
                return v.Select(Settings.ValueConverter.Deserialize<T>).ToArray();
            }
        }

        /// <summary>
        /// SCARD http://redis.io/commands/scard
        /// </summary>
        public async Task<long> GetLength(bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                return await Command.GetLength(Settings.Db, Key, queueJump).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// SRANDMEMBER http://redis.io/commands/srandmember
        /// </summary>
        public async Task<T> GetRandom(bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                var v = await Command.GetRandom(Settings.Db, Key, queueJump).ConfigureAwait(false);
                return Settings.ValueConverter.Deserialize<T>(v);
            }
        }

        /// <summary>
        /// SRANDMEMBER http://redis.io/commands/srandmember
        /// </summary>
        public async Task<T[]> GetRandom(int count, bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                var v = await Command.GetRandom(Settings.Db, Key, count, queueJump).ConfigureAwait(false);
                return v.Select(Settings.ValueConverter.Deserialize<T>).ToArray();
            }
        }

        /// <summary>
        /// SREM http://redis.io/commands/srem
        /// </summary>
        public async Task<bool> Remove(T member, bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                return await Command.Remove(Settings.Db, Key, Settings.ValueConverter.Serialize(member), queueJump).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// SREM http://redis.io/commands/srem
        /// </summary>
        public async Task<long> Remove(T[] members, bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                var v = members.Select(x => Settings.ValueConverter.Serialize(x)).ToArray();
                return await Command.Remove(Settings.Db, Key, v, queueJump).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// SPOP http://redis.io/commands/spop
        /// </summary>
        public async Task<T> RemoveRandom(bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                var v = await Command.RemoveRandom(Settings.Db, Key, queueJump).ConfigureAwait(false);
                return Settings.ValueConverter.Deserialize<T>(v);
            }
        }

        /// <summary>
        /// expire subtract Datetime.Now
        /// </summary>
        public Task<bool> SetExpire(DateTime expire, bool queueJump = false)
        {
            return SetExpire(expire - DateTime.Now, queueJump);
        }

        public Task<bool> SetExpire(TimeSpan expire, bool queueJump = false)
        {
            return SetExpire((int)expire.TotalSeconds, queueJump);
        }

        public async Task<bool> SetExpire(int seconds, bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                return await Connection.Keys.Expire(Settings.Db, Key, seconds, queueJump).ConfigureAwait(false);
            }
        }

        public async Task<bool> KeyExists(bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                return await Connection.Keys.Exists(Settings.Db, Key, queueJump).ConfigureAwait(false);
            }
        }

        public async Task<bool> Clear(bool queueJump = false)
        {
            using (Monitor.Start(Settings.CommandTracerFactory, Key, CallType))
            {
                return await Connection.Keys.Remove(Settings.Db, Key, queueJump).ConfigureAwait(false);
            }
        }
    }
}