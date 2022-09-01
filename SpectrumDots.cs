using OpenTK;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Animations;
using System;

namespace StorybrewScripts
{
    class SpectrumDots : StoryboardObjectGenerator
    {
        public override void Generate()
        {
            Spectrum(23115, 44810, true);
            Spectrum(153285, 173624, false);
            Spectrum(196675, 218370, true);
        }
        void Spectrum(int StartTime, int EndTime, bool DisplayBottom)
        {
            var MinimalHeight = 0.25f;
            var ScaleY = 70;
            float LogScale = 7;
            var Position = new Vector2(-103, 257);
            var Width = 854f;

            int BarCount = 100;
            int fftCount = BarCount * 2;

            var heightKeyframes = new KeyframedValue<float>[fftCount];
            for (var i = 0; i < fftCount; i++)
                heightKeyframes[i] = new KeyframedValue<float>(null);

            var timeStep = Beatmap.GetTimingPointAt(StartTime).BeatDuration / 6;
            var offset = timeStep * 0.2;
            
            for (var t = (double)StartTime; t <= EndTime; t += timeStep)
            {
                var fft = GetFft(t + offset, fftCount, null, OsbEasing.InExpo);
                for (var i = 0; i < fftCount; i++)
                {
                    var height = (float)Math.Log10(1 + fft[i] * LogScale) * ScaleY;
                    if (height < MinimalHeight) height = MinimalHeight;

                    heightKeyframes[i].Add(t, height);
                }
            }
            var barWidth = Width / BarCount;
            for (var i = 0; i < BarCount; i++)
            {
                var keyframes = heightKeyframes[i];
                keyframes.Simplify1dKeyframes(1.5, h => h);

                var topBar = GetLayer("").CreateSprite("sb/p.png", OsbOrigin.Centre, new Vector2(Position.X + i * barWidth, 0));
                var bottomBar = GetLayer("").CreateSprite("sb/p.png", OsbOrigin.Centre, new Vector2(Position.X + i * barWidth, 0));

                topBar.Scale(StartTime, barWidth / 3);
                topBar.Fade(StartTime, 0.5);
                topBar.Fade(EndTime - 1000, EndTime, 0.5, 0);
                topBar.Additive(StartTime);

                if (DisplayBottom)
                {
                    bottomBar.Scale(StartTime, barWidth / 3);
                    bottomBar.Fade(StartTime, 0.5);
                    bottomBar.Fade(EndTime - 1000, EndTime, 0.5, 0);
                    bottomBar.Additive(StartTime);

                    Position.Y = 240;
                    LogScale = 5.5f;
                }

                keyframes.ForEachPair((start, end) =>
                {
                    topBar.MoveY(start.Time, end.Time, 
                    (int)(Position.Y - start.Value / 2 * LogScale), (int)(Position.Y - end.Value / 2 * LogScale));

                    if (DisplayBottom)
                    {
                        bottomBar.MoveY(start.Time, end.Time, 
                        (int)(Position.Y + start.Value / 2 * LogScale), (int)(Position.Y + end.Value / 2 * LogScale));
                    }
                },
                MinimalHeight, s => s);
            }
        }
    }
}
