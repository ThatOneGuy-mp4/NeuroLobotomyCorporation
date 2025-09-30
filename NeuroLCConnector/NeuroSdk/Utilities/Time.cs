using System.Diagnostics;

namespace NeuroSDKCsharp.Utilities;

public abstract class Time
{
    public static float DeltaTime
    {
        get;
        private set;
    }
    public static float ElapsedTime => Stopwatch.ElapsedMilliseconds / 1000f;

    static Stopwatch Stopwatch = Stopwatch.StartNew();
    static long previousTime = Stopwatch.ElapsedTicks;
    
    public static void Update()
    {
        long currentTime = Stopwatch.ElapsedTicks;
        DeltaTime = (currentTime - previousTime) / (float)Stopwatch.Frequency;
        previousTime = currentTime;
    }
}