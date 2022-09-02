using OpenTK;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Animations;
using System;

namespace StorybrewScripts
{
    class ObjectHighlight : StoryboardObjectGenerator
    {
        public override void Generate()
        {
            Cursor(88200, 109810);
            Cursor(153285, 173539, true);
            Cursor(174980, 196590);
            Cursor(196675, 218285, true);
        }
        void Cursor(int StartTime, int EndTime, bool pixels = false)
        {
            var sprite = GetLayer("").CreateSprite(pixels ? "sb/p.png" : "sb/hl.png");
            sprite.Fade(StartTime, StartTime + 1000, 0, 0.6);
            sprite.Fade(EndTime, EndTime + 1000, 0.6, 0);
            sprite.Scale(StartTime, pixels ? 70 : 0.4);
            sprite.Additive(StartTime);
            sprite.Color(StartTime, pixels ? "#BC62F5" : "#7FACF5");
            if (pixels) sprite.Rotate(StartTime, Math.PI / 4);

            OsuHitObject lastObject = null;
            foreach (var hitobject in Beatmap.HitObjects)
            {
                var timestep = Beatmap.GetTimingPointAt((int)hitobject.StartTime).BeatDuration / 32;
                if (hitobject.StartTime >= StartTime - 5 && hitobject.StartTime <= EndTime + 5)
                {
                    if (lastObject == null)
                    {
                        if (hitobject is OsuSlider)
                        {
                            var keyframe = new KeyframedValue<Vector2>(null);
                            var startTime = hitobject.StartTime;
                            while (true)
                            {
                                var endTime = startTime + timestep;

                                var complete = hitobject.EndTime - startTime < 1;
                                if (complete) endTime = hitobject.EndTime;

                                var startPosition = hitobject.PositionAtTime(startTime);
                                keyframe.Add(startTime, startPosition);
                                keyframe.Simplify2dKeyframes(1, v => v);

                                if (complete) break;
                                startTime += timestep;
                            }
                            keyframe.ForEachPair((start, end) =>
                            {
                                sprite.Move(start.Time, end.Time, start.Value, end.Value);
                            });
                        }
                    }
                    else
                    {
                        if (lastObject is OsuSlider)
                        {
                            if (lastObject.Position != hitobject.Position)
                            sprite.Move(OsbEasing.Out, lastObject.EndTime, hitobject.StartTime, lastObject.EndPosition, hitobject.Position);
                            if (pixels && Random(2) % 2 == 0) 
                            sprite.Rotate(OsbEasing.Out, lastObject.EndTime, hitobject.StartTime, Math.PI / 4, -Math.PI / 4);
                        }
                        else if (lastObject is OsuCircle)
                        {
                            if (lastObject.Position != hitobject.Position)
                            sprite.Move(OsbEasing.Out, lastObject.StartTime, hitobject.StartTime, lastObject.Position, hitobject.Position);
                            if (pixels && Random(2) % 2 == 0) 
                            sprite.Rotate(OsbEasing.Out, lastObject.EndTime, hitobject.StartTime, -Math.PI / 4, Math.PI / 4);
                        }
                        if (hitobject is OsuSlider)
                        {
                            var keyframe = new KeyframedValue<Vector2>(null);
                            var startTime = hitobject.StartTime;
                            while (true)
                            {
                                var endTime = startTime + timestep;

                                var complete = hitobject.EndTime - startTime < 1;
                                if (complete) endTime = hitobject.EndTime;

                                var startPosition = hitobject.PositionAtTime(startTime);
                                keyframe.Add(startTime, startPosition);
                                keyframe.Simplify2dKeyframes(1, v => v);

                                if (complete) break;
                                startTime += timestep;
                            }
                            keyframe.ForEachPair((start, end) =>
                            {
                                sprite.Move(start.Time, end.Time, start.Value, end.Value);
                            });
                        }
                    }
                    lastObject = hitobject;
                }
            }
            using (var pool = new SpritePool(GetLayer(""), pixels ? "sb/p.png" : "sb/hl.png", (pooledSprite, start, end) =>
            {
                pooledSprite.Scale(start, pixels ? 5 : 0.35);
                pooledSprite.Color(start, pixels ? "#BC62F5" : "#7FACF5");
                pooledSprite.Additive(start);
                if (pixels) pooledSprite.Rotate(start, Math.PI / 4);
            }))
            {
                var lastPos = Vector2.Zero;
                for (var i = StartTime; i < EndTime; i += 10)
                {
                    var pos = (Vector2)sprite.PositionAt(i);
                    if (pos != lastPos)
                    {
                        var trail = pool.Get(i, i + (pixels ? 1000 : 750));
                        if (pixels) trail.Move(OsbEasing.InBack, i, i + 1000, pos, new Vector2(pos.X, pos.Y + 150));
                        else trail.Move(i, pos);
                        trail.Fade(i, i + (pixels ? 1000 : 750), pixels ? 0.5 : 0.3, 0);
                    }
                }
            }
        }
    }
}
