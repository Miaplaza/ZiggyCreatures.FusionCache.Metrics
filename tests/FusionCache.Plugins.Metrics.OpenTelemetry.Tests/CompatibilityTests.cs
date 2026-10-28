using System.Diagnostics.Metrics;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Plugins.Metrics.OpenTelemetry;

namespace FusionCache.Plugins.Metrics.OpenTelemetry.Tests;

/// <summary>
/// Covers the custom count accessor and the runtime plugin toggle used by Miaplaza.
/// </summary>
public class CompatibilityTests
{
    [Fact]
    public void AddPlugin_ShouldResumeCountersWithoutDuplicateHandlers_WhenToggled()
    {
        var meterName = Guid.NewGuid().ToString();
        var sets = 0;
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, currentListener) =>
        {
            if (instrument.Meter.Name == meterName && instrument.Name == "SET")
                currentListener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<int>((_, value, _, _) => sets += value);
        listener.Start();
        using var meter = new FusionMeter(meterName);
        using var cache = new ZiggyCreatures.Caching.Fusion.FusionCache(new FusionCacheOptions
        {
            EnableSyncEventHandlersExecution = true
        });

        cache.AddPlugin(meter);
        cache.Set("first", 1);
        cache.RemovePlugin(meter);
        cache.Set("untracked", 2);
        cache.AddPlugin(meter);
        cache.Set("last", 3);

        Assert.Equal(expected: 2, actual: sets);
    }

    [Fact]
    public void ItemCount_ShouldReadCurrentCount_WhenUsingCustomAccessor()
    {
        var meterName = Guid.NewGuid().ToString();
        long count = -1;
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, currentListener) =>
        {
            if (instrument.Meter.Name == meterName && instrument.Name == "ITEM_COUNT")
                currentListener.EnableMeasurementEvents(instrument);
        };
        listener.SetMeasurementEventCallback<long>((_, value, _, _) => count = value);
        listener.Start();
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        using var meter = new FusionMeter(meterName, () => memoryCache.Count);
        using var cache = new ZiggyCreatures.Caching.Fusion.FusionCache(new FusionCacheOptions(), memoryCache);
        cache.AddPlugin(meter);

        cache.Set("item", 42);
        listener.RecordObservableInstruments();
        Assert.Equal(expected: 1, actual: count);

        cache.Remove("item");
        listener.RecordObservableInstruments();
        Assert.Equal(expected: 0, actual: count);
    }
}
