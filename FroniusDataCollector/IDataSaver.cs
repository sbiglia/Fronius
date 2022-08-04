using FroniusSolarClient.Entities.SolarAPI.V1.PowerFlowRealtimeData;

namespace FroniusDataCollector;

public interface IDataSaver
{
    void SaveData(PowerFlowRealtimeData data);
}