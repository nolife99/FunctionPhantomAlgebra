using OpenTK;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using System;

namespace StorybrewScripts
{
    class ParticleManager : StoryboardObjectGenerator
    {
        public override void Generate()
        {
            Danmaku(44810, 65149, 5000);
            Danmaku(44810, 65149, 5000, true);

            Danmaku(109895, 126166, 5000);
            Danmaku(109895, 126166, 5000, true);

		    FallingDownParticle(88200, 109895, true);
            FallingDownParticle(131590, 151929);

            FallingDownParticle(174980, 217370, true);
        }
        void Danmaku(int startTime, int endTime, int speed, bool reverse = false)
        {
            var basePosition = new Vector2(470, 165);
            for (var i = 0; i < 4; i++)
            {
                var angle = (reverse ? -Math.PI / 2 : Math.PI / 2) * i;
                for (var l = 0; l < 50; l++)
                {
                    var endPosition = new Vector2(
                        (float)(basePosition.X + Math.Cos(angle) * 500),
                        (float)(basePosition.Y + Math.Sin(angle) * 500));

                    var loopCount = (endTime - startTime - l * 35) / speed;
    
                    var sprite = GetLayer("").CreateSprite("sb/p.png");
                    sprite.StartLoopGroup(startTime + l * 100, loopCount);
                    sprite.Move(0, speed, basePosition, endPosition);
                    sprite.Rotate(OsbEasing.In, 0, speed, angle, angle + (reverse ? 1 : -1));
                    sprite.ScaleVec(OsbEasing.In, speed / 6, speed, 10, 1, 10, 0);
                    sprite.Fade(OsbEasing.OutCubic, speed / 6, speed / 3, 0, 1);
                    sprite.Fade(speed, 1);
                    sprite.EndGroup();
    
                    angle += reverse ? -Math.PI / 50 : Math.PI / 50;
                }
            }
        }
        void FallingDownParticle(int startTime, int endTime, bool circles = false)
        {
            for (var i = 0; i < (circles ? 125 : 200); i++)
            {
                var speed = Random(5000, 10000);
                var particleFade = Random(0.1, 0.8);
                var startPos = new Vector2(Random(-107, 747), -10);
                var rotation = Random(-Math.PI / 2, Math.PI / 2);
                var realStart = startTime + i * 40 > startTime + speed / 10 ? startTime + i * 50 - speed : startTime + i * 50;

                var sprite = GetLayer("").CreateSprite("sb/p.png", OsbOrigin.Centre, startPos);
                sprite.StartLoopGroup(realStart, (endTime - realStart) / speed + 1);

                sprite.MoveY(0, speed, startPos.Y, 490);

                int scale = Random(2, 10);
                sprite.ScaleVec(0, speed / 2, scale, scale, scale, -scale);
                sprite.ScaleVec(speed / 2, speed, scale, -scale, scale, scale);

                sprite.Rotate(0, speed / 2, -rotation, rotation);
                sprite.Rotate(speed / 2, speed, rotation, rotation * 2);
                sprite.EndGroup();

                sprite.Additive(startTime);
                sprite.Fade(startTime, startTime + speed / 10, 0, particleFade);
                sprite.Fade(endTime, endTime + speed / 10, particleFade, 0);

                if (circles)
                {
                    var circleSpeed = Random(5000, 10000);
                    var fade = Random(0.1, 0.8);
                    var posStart = new Vector2(Random(-107, 747), -10);
                    var rot = Random(-Math.PI / 2, Math.PI / 2);
                    var start = startTime + i * 50 > startTime + circleSpeed / 10 ? startTime + i * 50 - circleSpeed : startTime + i * 50;

                    var circle = GetLayer("").CreateSprite("sb/c.png", OsbOrigin.Centre, posStart);
                    circle.StartLoopGroup(start, (endTime - start) / circleSpeed + 1);
                    circle.MoveY(0, circleSpeed, posStart.Y, 490);

                    double cScale = Random(0.007f, 0.049f);
                    circle.ScaleVec(0, circleSpeed / 2, cScale, cScale, cScale, -cScale);
                    circle.ScaleVec(circleSpeed / 2, circleSpeed, cScale, -cScale, cScale, cScale);

                    circle.Rotate(0, circleSpeed / 2, -rot, rot);
                    circle.Rotate(circleSpeed / 2, circleSpeed, rot, rot * 2);
                    circle.EndGroup();

                    circle.Additive(startTime);
                    circle.Fade(startTime, startTime + circleSpeed / 10, 0, fade);
                    circle.Fade(endTime, endTime + circleSpeed / 10, fade, 0);
                }
            }
        }
    }
}
