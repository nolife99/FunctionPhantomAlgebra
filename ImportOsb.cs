using OpenTK;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace StorybrewScripts
{
    class ImportOsb : StoryboardObjectGenerator
    {
        [Configurable] public string Path = "storyboard.osb";
        Dictionary<string, string> variables = new Dictionary<string, string>();

        public override void Generate()
        {
            using (var file = OpenProjectFile(Path))
            using (var reader = new StreamReader(file, new UTF8Encoding()))
            reader.ParseSections(section =>
            {
                switch (section)
                {
                    case "Variables": parseVariables(reader); break;
                    case "Events": parseEvents(reader); break;
                }
            });
        }
        void parseVariables(StreamReader reader)
        {
            reader.ParseSectionLines(line =>
            {
                var value = line.Split('=');
                if (value.Length == 2) variables.Add(value[0], value[1]);
            });
        }
        void parseEvents(StreamReader reader)
        {
            OsbSprite sprite = null;
            var inCommandGroup = false;
            reader.ParseSectionLines(line =>
            {
                if (line.StartsWith("//")) return;

                var depth = 0;
                while (line.Substring(depth).StartsWith(" ")) ++depth;

                var trim = applyVariables(line.Trim());
                var value = trim.Split(',');

                if (inCommandGroup && depth < 2)
                {
                    sprite.EndGroup();
                    inCommandGroup = false;
                }
                switch (value[0])
                {
                    case "Sprite":
                    {
                        var layerName = value[1];
                        var origin = (OsbOrigin)Enum.Parse(typeof(OsbOrigin), value[2]);
                        var path = removePathQuotes(value[3]);
                        var x = float.Parse(value[4]);
                        var y = float.Parse(value[5]);
                        sprite = GetLayer(layerName).CreateSprite(path, origin, new Vector2(x, y));
                    }
                    break;
                    case "Animation":
                    {
                        var layerName = value[1];
                        var origin = (OsbOrigin)Enum.Parse(typeof(OsbOrigin), value[2]);
                        var path = removePathQuotes(value[3]);
                        var x = float.Parse(value[4]);
                        var y = float.Parse(value[5]);
                        var frameCount = int.Parse(value[6]);
                        var frameDelay = double.Parse(value[7]);
                        var loopType = (OsbLoopType)Enum.Parse(typeof(OsbLoopType), value[8]);
                        sprite = GetLayer(layerName).CreateAnimation(path, frameCount, frameDelay, loopType, origin, new Vector2(x, y));
                    }
                    break;
                    case "Sample":
                    {
                        var time = int.Parse(value[1]);
                        var layerName = value[2];
                        var path = removePathQuotes(value[3]);
                        var volume = float.Parse(value[4]);
                        GetLayer(layerName).CreateSample(path, time, volume);
                    }
                    break;
                    case "T":
                    {
                        var triggerName = value[1];
                        var startTime = int.Parse(value[2]);
                        var endTime = int.Parse(value[3]);
                        var groupNumber = value.Length > 4 ? int.Parse(value[4]) : 0;
                        sprite.StartTriggerGroup(triggerName, startTime, endTime, groupNumber);
                        inCommandGroup = true;
                    }
                    break;
                    case "L":
                    {
                        var startTime = int.Parse(value[1]);
                        var loopCount = int.Parse(value[2]);
                        sprite.StartLoopGroup(startTime, loopCount);
                        inCommandGroup = true;
                    }
                    break;
                    default:
                    {
                        if (string.IsNullOrEmpty(value[3])) value[3] = value[2];

                        var command = value[0];
                        var easing = (OsbEasing)int.Parse(value[1]);
                        var startTime = int.Parse(value[2]);
                        var endTime = int.Parse(value[3]);

                        switch (command)
                        {
                            case "F":
                            {
                                var startValue = double.Parse(value[4]);
                                var endValue = value.Length > 5 ? double.Parse(value[5]) : startValue;
                                sprite.Fade(easing, startTime, endTime, startValue, endValue);
                            }
                            break;
                            case "S":
                            {
                                var startValue = double.Parse(value[4]);
                                var endValue = value.Length > 5 ? double.Parse(value[5]) : startValue;
                                sprite.Scale(easing, startTime, endTime, startValue, endValue);
                            }
                            break;
                            case "V":
                            {
                                var startX = double.Parse(value[4]);
                                var startY = double.Parse(value[5]);
                                var endX = value.Length > 6 ? double.Parse(value[6]) : startX;
                                var endY = value.Length > 7 ? double.Parse(value[7]) : startY;
                                sprite.ScaleVec(easing, startTime, endTime, startX, startY, endX, endY);
                            }
                            break;
                            case "R":
                            {
                                var startValue = double.Parse(value[4]);
                                var endValue = value.Length > 5 ? double.Parse(value[5]) : startValue;
                                sprite.Rotate(easing, startTime, endTime, startValue, endValue);
                            }
                            break;
                            case "M":
                            {
                                var startX = double.Parse(value[4]);
                                var startY = double.Parse(value[5]);
                                var endX = value.Length > 6 ? double.Parse(value[6]) : startX;
                                var endY = value.Length > 7 ? double.Parse(value[7]) : startY;
                                sprite.Move(easing, startTime, endTime, startX, startY, endX, endY);
                            }
                            break;
                            case "MX":
                            {
                                var startValue = double.Parse(value[4]);
                                var endValue = value.Length > 5 ? double.Parse(value[5]) : startValue;
                                sprite.MoveX(easing, startTime, endTime, startValue, endValue);
                            }
                            break;
                            case "MY":
                            {
                                var startValue = double.Parse(value[4]);
                                var endValue = value.Length > 5 ? double.Parse(value[5]) : startValue;
                                sprite.MoveY(easing, startTime, endTime, startValue, endValue);
                            }
                            break;
                            case "C":
                            {
                                var startX = double.Parse(value[4]);
                                var startY = double.Parse(value[5]);
                                var startZ = double.Parse(value[6]);
                                var endX = value.Length > 7 ? double.Parse(value[7]) : startX;
                                var endY = value.Length > 8 ? double.Parse(value[8]) : startY;
                                var endZ = value.Length > 9 ? double.Parse(value[9]) : startZ;
                                sprite.Color(easing, startTime, endTime, startX / 255f, startY / 255f, startZ / 255f, endX / 255f, endY / 255f, endZ / 255f);
                            }
                            break;
                            case "P":
                            {
                                var type = value[4];
                                switch (type)
                                {
                                    case "A": sprite.Additive(startTime, endTime); break;
                                    case "H": sprite.FlipH(startTime, endTime); break;
                                    case "V": sprite.FlipV(startTime, endTime); break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }, false);

            if (inCommandGroup)
            {
                sprite.EndGroup();
                inCommandGroup = false;
            }
        }
        static string removePathQuotes(string path) => path.StartsWith("\"") && path.EndsWith("\"") ? path.Substring(1, path.Length - 2) : path;
        string applyVariables(string line)
        {
            if (!line.Contains("$")) return line;
            foreach (var entry in variables) line = line.Replace(entry.Key, entry.Value);
            return line;
        }
    }
}
