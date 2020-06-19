﻿namespace HealthKitData.Core
{
    public interface IRange<T>
    {
        T Start { get; }
        T End { get; }
        bool Includes(T value, Clusivity clusivity = Clusivity.Exclusive);
        bool Includes(IRange<T> range, Clusivity clusivity = Clusivity.Exclusive);
    }
}