using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System;

namespace StorybrewScripts
{
    class BackgroundManager : StoryboardObjectGenerator
    {
        double beat;
        public override void Generate()
        {
            beat = Beatmap.GetTimingPointAt(1421).BeatDuration;

            using (var pool = new SpritePool(GetLayer("Foreground"), "sb/p.png", new Vector2(320, 240), (pooledSprite, start, end) =>
            {
                pooledSprite.ScaleVec(start, 854, 480);
                pooledSprite.Additive(start);
            }))
            {
                Action<int, double> Flash = (sTime, fade) =>
                {
                    var flash = pool.Get(sTime, sTime + beat * 4);
                    flash.Fade(sTime, sTime + beat * 4, fade, 0);
                };

                Flash(1421, 0.8);
                Flash(23115, 1);
                Flash(27183, 0.6);
                Flash(27861, 0.7);
                Flash(28539, 1);
                Flash(32607, 0.5);
                Flash(33200, 0.6);
                Flash(33624, 0.7);
                Flash(33963, 0.8);
                Flash(39387, 0.8);
                Flash(43454, 0.5);
                Flash(44048, 0.6);
                Flash(44471, 0.7);
                Flash(44810, 0.8);
                Flash(66505, 0.8);
                Flash(77353, 0.8);
                Flash(86844, 0.6);
                Flash(87522, 0.7);
                Flash(88200, 1);
                Flash(97692, 0.8);
                Flash(99048, 1);
                Flash(109895, 1);
                Flash(164132, 0.8);
                Flash(174980, 1);
                Flash(184471, 0.8);
                Flash(185827, 1);
                Flash(196675, 1);
                Flash(202098, 0.8);
                Flash(207522, 1);
                Flash(212946, 0.8);
                Flash(218370, 1);
                Flash(229217, 1);
                Flash(240065, 1);
            }

            SquareTransition(27183, 28539, 28539, 30, Color4.Purple);
            SquareTransition(32607, 33963, 33963, 15, Color4.IndianRed, true);
            SquareTransition(33200, 33963, 33963, 30, Color4.White);
            SquareTransition(38709, 39387, 39387, 30, Color4.IndianRed);
            SquareTransition(43454, 44810, 44810, 30, Color4.Purple);

            Action<int> RedPart = delay => 
            {
                Glitch(66505 + delay, 67183 + delay, 0.4);
                Glitch(67861 + delay, 69217 + delay, 0.2);
                Glitch(69217 + delay, 70573 + delay, 0.4);
                Glitch(70573 + delay, 71929 + delay, 0.2);
                Glitch(71251 + delay, 71929 + delay, 0.4);
                Glitch(71929 + delay, 72607 + delay, 0.4);
                Glitch(72607 + delay, 74641 + delay, 0.2);
                Glitch(73285 + delay, 74641 + delay, 0.2);
                Glitch(73963 + delay, 74641 + delay, 0.2);
                Glitch(74641 + delay, 77353 + delay, 0.2);
                Glitch(77353 + delay, 78031 + delay, 0.4);
                Glitch(78031 + delay, 80065 + delay, 0.2);
                Glitch(78709 + delay, 80065 + delay, 0.2);
                Glitch(79387 + delay, 80065 + delay, 0.2);
                Glitch(80065 + delay, 80743 + delay, 0.4);
                Glitch(80743 + delay, 82776 + delay, 0.2);
                Glitch(81420 + delay, 82776 + delay, 0.2);
                Glitch(82098 + delay, 82776 + delay, 0.4);
                Glitch(82776 + delay, 83454 + delay, 0.4);
                Glitch(83454 + delay, 85488 + delay, 0.2);
                Glitch(84132 + delay, 85488 + delay, 0.2);
                Glitch(84810 + delay, 85488 + delay, 0.2);
                Glitch(85488 + delay, 86166 + delay, 0.4);
                Glitch(86166 + delay, 88200 + delay, 0.2);
                Glitch(87522 + delay, 88200 + delay, 0.2);

                SquareTransition(43454 + delay, 44810 + delay, 44810 + delay, 30, Color4.Purple);
                SquareTransition(66505 + delay, 69217 + delay, 69217 + delay, 30, Color4.IndianRed);
                SquareTransition(69217 + delay, 71929 + delay, 71929 + delay, 30, Color4.IndianRed);
                SquareTransition(71929 + delay, 74641 + delay, 74641 + delay, 30, Color4.IndianRed);
                SquareTransition(76675 + delay, 77353 + delay, 77353 + delay, 15, Color4.Red, true);
                SquareTransition(74641 + delay, 77353 + delay, 77353 + delay, 30, Color4.IndianRed);
                SquareTransition(77353 + delay, 80065 + delay, 80065 + delay, 30, Color4.IndianRed);
                SquareTransition(80065 + delay, 82776 + delay, 82776 + delay, 30, Color4.IndianRed);
                SquareTransition(82776 + delay, 85488 + delay, 85488 + delay, 30, Color4.IndianRed);
                SquareTransition(85488 + delay, 88115 + delay, 88115 + delay, 30, Color4.IndianRed);
                SquareTransition(86844 + delay, 88200 + delay, 88200 + delay, 15, Color4.White, true);
            };

            RedPart(0);
            RedPart(151865);

            Glitch(153285, 155997, 0.4, "sb/effectthing.png");
            Glitch(155573, 155997, 0.4, "sb/effectthing.png");
            Glitch(155997, 158709, 0.4, "sb/effectthing.png");
            Glitch(158285, 158709, 0.4, "sb/effectthing.png");
            Glitch(158709, 161421, 0.4, "sb/effectthing.png");
            Glitch(160997, 161421, 0.4, "sb/effectthing.png");
            Glitch(161421, 164132, 0.4, "sb/effectthing.png");
            Glitch(163115, 164132, 0.4, "sb/effectthing.png");

            var blurSprite = GetLayer("BeatBG").CreateSprite("sb/effectthing.png");
            BeatBlur(23115, 44810, blurSprite);
            BeatBlur(196675, 212946, blurSprite);

            var white = GetLayer("BeatBG").CreateSprite("sb/bgwhite.png");
            BeatBlur(44810, 65149, white);
            BeatBlur(109895, 130234, white);

            Action<int, int, int, OsbSprite> EqualBlur = (startTime, endTime, divisor, sprite) =>
            {
                sprite.StartLoopGroup(startTime, (endTime - startTime) / (int)(beat / divisor));
                sprite.Scale(OsbEasing.Out, 0, beat / divisor, 1.04, 1);
                sprite.Fade(0, beat / divisor, 0.2, 0);
                sprite.EndGroup();
            };

            EqualBlur(164132, 169556, 1, blurSprite);
            EqualBlur(169556, 172268, 2, blurSprite);
            EqualBlur(172268, 173539, 4, blurSprite);
            EqualBlur(212946, 215658, 2, blurSprite);
            EqualBlur(215658, 218370, 4, blurSprite);

            var letterbox = GetLayer("Letterbox").CreateSprite("sb/p.png", OsbOrigin.TopCentre, new Vector2(320, 0));
            var letterbox2 = GetLayer("Letterbox").CreateSprite("sb/p.png", OsbOrigin.BottomCentre, new Vector2(320, 480));
            
            letterbox.Color(1421, Color4.Black);
            letterbox2.Color(1421, Color4.Black);
            letterbox.ScaleVec(1421, 854, 40);
            letterbox2.ScaleVec(1421, 854, 40);

            Action<int, int> Letterbox = (startTime, endTime) =>
            {
                letterbox.Fade(startTime, 1);
                letterbox2.Fade(startTime, 1);

                letterbox.Fade(endTime, 0);
                letterbox2.Fade(endTime, 0);
            };

            Letterbox(1421, 21929);
            Letterbox(131590, 153285);
        }
        void Glitch(int start, int end, double fade, string path = "sb/effectthingred.png")
        {
            var sprite = GetLayer("GlitchAnim").CreateSprite(path);
            sprite.Scale(start, 860f / GetMapsetBitmap(path).Width);
            sprite.Fade(start, end, fade, 0);

            var lastPos = new Vector2(320, 240);
            for (var i = start; i < end; i += 40)
            {
                var newPos = new Vector2(Random(315, 325), Random(235, 245));
                sprite.Move(OsbEasing.OutExpo, i, i + 40, lastPos, newPos);
                
                lastPos = newPos;
            }
        }
        void SquareTransition(int startTime, int endTime, int endFade, int amount, Color4 color, bool reverse = false)
        {
            var squareScale = 854f / amount;
            var posX = -107 + squareScale / 2;
            var posY = squareScale / 2;
            var duration = endTime - startTime;

            while (posX < 750)
            {
                while (posY <= 485)
                {
                    var sprite = GetLayer("Transition").CreateSprite("sb/p.png", OsbOrigin.Centre, new Vector2(posX, posY));
                    sprite.Scale(reverse ? OsbEasing.In : OsbEasing.None, startTime, endTime, reverse ? squareScale : 0, reverse ? 0 : squareScale);
                    sprite.Rotate(startTime, endTime, reverse ? -Math.PI : 0, reverse ? 0 : -Math.PI);
                    sprite.Additive(startTime);
                    if (color != Color4.White) sprite.Color(endFade, color);
                    if (reverse) sprite.Fade(OsbEasing.Out, startTime, startTime + 500, 0, 0.4);
                    else sprite.Fade(OsbEasing.Out, endFade, endFade + 500, 0.4, 0);

                    posY += squareScale;
                }
                posY = squareScale / 2;
                posX += squareScale;
            }
        }
        void BeatBlur(int startTime, int endTime, OsbSprite sprite)
        {
            var beat = Beatmap.GetTimingPointAt(startTime).BeatDuration;
            sprite.StartLoopGroup(startTime, (endTime - startTime) / (int)(beat * 4));
            sprite.Scale(0, beat * 1.5, 1.02, 1);
            sprite.Scale(OsbEasing.OutSine, beat * 1.5, beat * 4, 1.04, 1);
            sprite.Fade(0, beat * 1.5, 0.2, 0);
            sprite.Fade(beat * 1.5, beat * 4, 0.2, 0);
            sprite.EndGroup();
        }
    }
}
