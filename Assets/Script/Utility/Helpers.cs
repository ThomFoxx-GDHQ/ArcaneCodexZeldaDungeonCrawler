using System.Collections.Generic;
using System;
using UnityEngine;

public static class Helpers
{
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds GetWait(float timer)
    {
        if (WaitDictionary.TryGetValue(timer, out var wait)) return wait;

        WaitDictionary[timer] = new WaitForSeconds(timer);
        return WaitDictionary[timer];
    }

}

[System.Serializable]
public struct FoxxTime : IEquatable<object>, IComparable<FoxxTime>
{
    public int hour;
    public int minute;

    public FoxxTime(int hour, int minute)
    {
        this.hour = hour;
        this.minute = minute;
    }

    public string whatTime => $"{hour:D2}:{minute:D2}";

    private int TotalMinutes => hour * 60 + minute;

    public int CompareTo(FoxxTime other)
    {
        return TotalMinutes.CompareTo(other.TotalMinutes);
    }

    public override bool Equals(object obj)
    {
        return obj is FoxxTime other && Equals(other);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 31 + hour.GetHashCode();
        hash = hash * 31 + minute.GetHashCode();
        return hash;
    }

    public static bool operator ==(FoxxTime a, FoxxTime b) => a.Equals(b); 
    public static bool operator !=(FoxxTime a, FoxxTime b) => !a.Equals(b);

    public static bool operator <(FoxxTime a, FoxxTime b) => a.CompareTo(b) < 0;
    public static bool operator >(FoxxTime a, FoxxTime b) => a.CompareTo(b) > 0;
    public static bool operator <=(FoxxTime a, FoxxTime b) => a.CompareTo(b) <= 0;
    public static bool operator >=(FoxxTime a, FoxxTime b) => a.CompareTo(b) >= 0;

    public static FoxxTime operator +(FoxxTime a, FoxxTime b)
    {
        FoxxTime value = new FoxxTime(0,0);
        value.hour = a.hour + b.hour;
        value.minute = a.minute + b.minute;
        return value;
    }
}