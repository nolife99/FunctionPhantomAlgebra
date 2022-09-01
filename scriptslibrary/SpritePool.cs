using System;
using System.Collections.Generic;
using OpenTK;

namespace StorybrewCommon.Storyboarding
{
    public class SpritePool : IDisposable //optimized OsbSpritePool utility (original by Damnae)
    {
        readonly StoryboardLayer layer;
        readonly string path;
        readonly OsbOrigin origin;
        readonly Vector2 position; // use only if there is a common positioning between sprites
        readonly Action<OsbSprite, double, double> finalizeSprite;
        readonly List<PooledSprite> pooledSprites = new List<PooledSprite>();

        public int MaxPoolDuration = 0;

        public SpritePool(StoryboardLayer layer, string path, OsbOrigin origin, Vector2 position, Action<OsbSprite, double, double> finalizeSprite = null)
        {
            this.layer = layer;
            this.path = path;
            this.origin = origin;
            this.position = position;
            this.finalizeSprite = finalizeSprite;
        }
        public SpritePool(StoryboardLayer layer, string path, OsbOrigin origin, Action<OsbSprite, double, double> finalizeSprite = null)
        {
            this.layer = layer;
            this.path = path;
            this.origin = origin;
            this.position = Vector2.Zero;
            this.finalizeSprite = finalizeSprite;
        }
        public SpritePool(StoryboardLayer layer, string path, Vector2 position, Action<OsbSprite, double, double> finalizeSprite = null)
        {
            this.layer = layer;
            this.path = path;
            this.origin = OsbOrigin.Centre;
            this.position = position;
            this.finalizeSprite = finalizeSprite;
        }
        public SpritePool(StoryboardLayer layer, string path, Action<OsbSprite, double, double> finalizeSprite = null)
        {
            this.layer = layer;
            this.path = path;
            this.origin = OsbOrigin.Centre;
            this.position = Vector2.Zero;
            this.finalizeSprite = finalizeSprite;
        }
        public SpritePool(StoryboardLayer layer, string path, OsbOrigin origin, Vector2 position, bool additive)
        {
            this.layer = layer;
            this.path = path;
            this.origin = origin;
            this.position = position;
            this.finalizeSprite = additive ? (sprite, start, end) => sprite.Additive(start) : (Action<OsbSprite, double, double>) null;
        }
        public SpritePool(StoryboardLayer layer, string path, bool additive)
        {
            this.layer = layer;
            this.path = path;
            this.origin = OsbOrigin.Centre;
            this.position = Vector2.Zero;
            this.finalizeSprite = additive ? (sprite, start, end) => sprite.Additive(start) : (Action<OsbSprite, double, double>) null;
        }
        public OsbSprite Get(double startTime, double endTime)
        {
            var result = (PooledSprite)null;
            foreach (var pooledSprite in pooledSprites)

            if (getMaxPoolDuration(startTime, endTime, MaxPoolDuration, pooledSprite) && 
            (result == null || pooledSprite.StartTime < result.StartTime)) result = pooledSprite;

            if (result != null)
            {
                result.EndTime = endTime;
                return result.Sprite;
            }

            var sprite = CreateSprite(layer, path, origin, position);
            pooledSprites.Add(new PooledSprite(sprite, startTime, endTime));

            return sprite;
        }
        static bool getMaxPoolDuration(double startTime, double endTime, int value, PooledSprite sprite)
        {
            var result = sprite.EndTime <= startTime;
            if (value > 0) result = sprite.EndTime <= startTime && startTime < sprite.StartTime + value;
            return result;
        }
        public void Clear()
        {
            if (finalizeSprite != null)
            foreach (var pooledSprite in pooledSprites)
            {
                var sprite = pooledSprite.Sprite;
                finalizeSprite(sprite, sprite.CommandsStartTime, pooledSprite.EndTime);
            }
            pooledSprites.Clear();
        }
        
        protected virtual OsbSprite CreateSprite(StoryboardLayer layer, string path, OsbOrigin origin, Vector2 position) 
        => layer.CreateSprite(path, origin, position);

        class PooledSprite
        {
            public OsbSprite Sprite;
            public double StartTime;
            public double EndTime;
            public PooledSprite(OsbSprite sprite, double startTime, double endTime)
            {
                Sprite = sprite;
                StartTime = startTime;
                EndTime = endTime;
            }
        }
        
        bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Clear();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
    public class SpritePools : IDisposable
    {
        readonly StoryboardLayer layer;
        readonly Dictionary<string, SpritePool> pools = new Dictionary<string, SpritePool>();

        int maxPoolDuration = 0;
        public int MaxPoolDuration
        {
            get { return maxPoolDuration; }
            set
            {
                if (maxPoolDuration == value) return;
                maxPoolDuration = value;
                foreach (var pool in pools.Values) pool.MaxPoolDuration = maxPoolDuration;
            }
        }

        public SpritePools(StoryboardLayer layer)
        {
            this.layer = layer;
        }
        public void Clear()
        {
            foreach (var pool in pools) pool.Value.Clear();
            pools.Clear();
        }
        public OsbSprite Get(double startTime, double endTime, string path, OsbOrigin origin, Vector2 position, Action<OsbSprite, double, double> finalizeSprite = null, int poolGroup = 0)
        => getPool(path, origin, position, finalizeSprite, poolGroup).Get(startTime, endTime);

        public OsbSprite Get(double startTime, double endTime, string path, Action<OsbSprite, double, double> attributes = null, int group = 0)
        => getPool(path, OsbOrigin.Centre, Vector2.Zero, attributes, group).Get(startTime, endTime);

        public OsbSprite Get(double startTime, double endTime, string path, OsbOrigin origin, Action<OsbSprite, double, double> attributes = null, int group = 0)
        => getPool(path, origin, Vector2.Zero, attributes, group).Get(startTime, endTime);

        public OsbSprite Get(double startTime, double endTime, string path, OsbOrigin origin, bool additive, int group = 0)
        => Get(startTime, endTime, path, origin, Vector2.Zero, additive ? (sprite, start, end) => sprite.Additive(start) : (Action<OsbSprite, double, double>) null, group); 

        public OsbSprite Get(double startTime, double endTime, string path, bool additive, int group = 0)
        => Get(startTime, endTime, path, OsbOrigin.Centre, Vector2.Zero, additive ? (sprite, start, end) => sprite.Additive(start) : (Action<OsbSprite, double, double>) null, group); 

        public OsbSprite Get(double startTime, double endTime, string path, OsbOrigin origin, Vector2 position, bool additive, int poolGroup = 0)
        => Get(startTime, endTime, path, origin, position, additive ? (sprite, spriteStartTime, spriteEndTime) => sprite.Additive(spriteStartTime) : (Action<OsbSprite, double, double>) null, poolGroup);
        
        SpritePool getPool(string path, OsbOrigin origin, Vector2 position, Action<OsbSprite, double, double> finalizeSprite, int poolGroup)
        {
            var key = getKey(path, origin, finalizeSprite, poolGroup);
            SpritePool pool;
            if (!pools.TryGetValue(key, out pool)) pools.Add(key, pool = new SpritePool(layer, path, origin, position, finalizeSprite) { MaxPoolDuration = maxPoolDuration, }); 
            return pool;
        }
        string getKey(string path, OsbOrigin origin, Action<OsbSprite, double, double> action, int poolGroup)
        => $"{path}#{origin}#{action?.Target}.{action?.Method.Name}#{poolGroup}";

        bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Clear();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}