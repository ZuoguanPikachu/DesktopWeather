namespace DesktopWeather.Util
{
    internal static class WindConvert
    {
        public static string SpeedToDesc(int power)
        {
            if (power < 1)
            {
                return "0级 无风";
            }
            else if (power < 6)
            {
                return "1级 微风徐徐";
            }
            else if (power < 12)
            {
                return "2级 清风";
            }
            else if (power < 20)
            {
                return "3级 树叶摇摆";
            }
            else if(power < 29)
            {
                return "4级 树枝摇动";
            }
            else if (power < 39)
            {
                return "5级 风力强劲";
            }
            else if (power < 50)
            {
                return "6级 树枝摇动";
            }
            else if (power < 62)
            {
                return "7级 风力超强";
            }
            else if (power < 75)
            {
                return "8级 狂风大作";
            }
            else if (power < 89)
            {
                return "9级 狂风呼啸";
            }
            else if (power < 103)
            {
                return "10级 暴风毁树";
            }
            else if (power < 118)
            {
                return "11级 暴风毁树";
            }
            else if (power < 134)
            {
                return "12级 飓风";
            }
            else if (power < 150)
            {
                return "13级 台风";
            }
            else if (power < 167)
            {
                return "14级 强台风";
            }
            else if (power < 184)
            {
                return "15级 强台风";
            }
            else if (power < 202)
            {
                return "16级 超强台风";
            }
            else
            {
                return "17级 超强台风";
            }
        }

        public static string CornerToDirection(float corner)
        {
            if(corner <= 22.5 || corner > 337.5)
            {
                return "北风";
            }
            else if (corner <= 67.5)
            {
                return "东北风";
            }
            else if (corner <= 112.5)
            {
                return "东风";
            }
            else if (corner <= 157.5)
            {
                return "东南风";
            }
            else if (corner <= 202.5)
            {
                return "南风";
            }
            else if (corner <= 247.5)
            {
                return "西南风";
            }
            else if (corner <= 292.5)
            {
                return "西风";
            }
            else if (corner <= 337.5)
            {
                return "西北风";
            }
            else
            {
                return "北风";
            }
        }
    }
}
