using OpenTK;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.CommandValues;
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

            Action<OsuHitObject> GetSliderMovement = objects =>
            {
                var keyframe = new KeyframedValue<Vector2>(null);
                var timeStep = Beatmap.GetTimingPointAt((int)objects.StartTime).BeatDuration / 64;
                var startTime = objects.StartTime;
                
                while (true)
                {
                    var endTime = startTime + timeStep;
                    var complete = objects.EndTime - startTime < 1;
                    if (complete) endTime = objects.EndTime;

                    keyframe.Add(startTime, objects.PositionAtTime(startTime));

                    if (complete) break;
                    startTime += timeStep;
                }
                keyframe.Simplify2dKeyframes(1.0, v => v);
                keyframe.ForEachPair((start, end) =>
                {
                    sprite.Move(start.Time, end.Time, start.Value, end.Value);
                });
            };

            OsuHitObject lastObj = null;
            foreach (var hit in Beatmap.HitObjects)
            {
                if (hit.StartTime >= StartTime - 5 && hit.StartTime <= EndTime + 5)
                {
                    if (lastObj == null)
                    {
                        if (hit is OsuSlider) GetSliderMovement(hit);
                    }
                    else
                    {
                        if (lastObj is OsuSlider)
                        {
                            if (lastObj.EndPosition != hit.Position) sprite.Move(OsbEasing.Out, lastObj.EndTime, hit.StartTime, lastObj.EndPosition, hit.Position);
                            if (pixels) sprite.Rotate(OsbEasing.Out, lastObj.EndTime, hit.StartTime, Math.PI / 4, -Math.PI / 4);
                        }
                        else if (lastObj is OsuCircle)
                        {
                            if (lastObj.Position != hit.Position) sprite.Move(OsbEasing.Out, lastObj.StartTime, hit.StartTime, lastObj.Position, hit.Position);
                            if (pixels) sprite.Rotate(OsbEasing.Out, lastObj.EndTime, hit.StartTime, -Math.PI / 4, Math.PI / 4);
                        }
                        if (hit is OsuSlider)
                        {
                            GetSliderMovement(hit);
                        }
                    }
                    lastObj = hit;
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
                var lastPos = new CommandPosition(0, 0);
                for (var i = StartTime; i < EndTime; i += (int)Beatmap.GetTimingPointAt(StartTime).BeatDuration / 24)
                {
                    var pos = sprite.PositionAt(i);
                    if (pos != lastPos)
                    {
                        var trail = pool.Get(i, i + (pixels ? 1000 : 750));
                        if (pixels) trail.Move(OsbEasing.InBack, i, i + 1000, pos, new Vector2(pos.X, pos.Y + 150));
                        else trail.Move(i, pos);
                        trail.Fade(i, i + (pixels ? 1000 : 750), pixels ? 0.5 : 0.3, 0);
                    }
                    lastPos = pos;
                }
            }
        }
    }
}
