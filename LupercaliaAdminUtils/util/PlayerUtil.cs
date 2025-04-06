using CounterStrikeSharp.API.Core;

namespace LupercaliaAdminUtils.util;

public static class PlayerUtil
{
    public static bool IsPlayerAlive(CCSPlayerController? client)
    {
        if (client == null)
            return false;
        
        var playerPawn = client.PlayerPawn.Value;
        
        if (playerPawn == null)
            return false;
        
        return playerPawn.LifeState == (byte)LifeState_t.LIFE_ALIVE;
    }
    
    public static string GetPlayerModel(CCSPlayerController client)
    {
        if (client.PlayerPawn.Value == null)
            return string.Empty;

        CCSPlayerPawn playerPawn = client.PlayerPawn.Value;

        if (playerPawn.CBodyComponent == null)
            return string.Empty;

        if (playerPawn.CBodyComponent.SceneNode == null)
            return string.Empty;

        return playerPawn.CBodyComponent.SceneNode.GetSkeletonInstance().ModelState.ModelName;
    }

    public static bool SetPlayerModel(CCSPlayerController client, string modelPath)
    {
        if (client.PlayerPawn.Value == null)
            return false;
        
        client.PlayerPawn.Value.SetModel(modelPath);
        return true;
    }
}