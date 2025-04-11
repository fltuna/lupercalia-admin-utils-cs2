using System.Globalization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NativeVoteAPI;
using NativeVoteAPI.API;
using TNCSSPluginFoundation.Models.Plugin;
using TNCSSPluginFoundation.Utils.Entity;

namespace LupercaliaAdminUtils.Modules;

public class ExtendTimeLimit(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "ExtendRoundTime";
    public override string ModuleChatPrefix => "[ExtendRoundTime]";

    private ConVar? mp_timelimit => ConVar.Find("mp_timelimit");
    
    private INativeVoteApi? _nativeVoteApi;
    private const string NativeVoteIdentifier = "LupercaliaAdminUtils:ExtendTimeLimit";
    private int _timeToExtend = 0;
    
    protected override void OnInitialize()
    {
        Plugin.AddCommand("css_extend", "Extend mp_timelimit", CommandExtendTimeLimit);
        Plugin.AddCommand("css_voteextend", "Vote to extend mp_timelimit", CommandVoteExtendTimeLimit);
    }
    
    protected override void OnAllPluginsLoaded()
    {
        try
        {
            _nativeVoteApi = ServiceProvider.GetRequiredService<INativeVoteApi>();
        }
        catch (Exception)
        {
            // Unused
        }

        if (_nativeVoteApi == null)
        {
            Logger.LogError("Failed to find required service: NativeVoteAPI. Unloading module...");
            UnloadModule();
            return;
        }
        
        
        _nativeVoteApi.OnVotePass += OnVotePass;
        _nativeVoteApi.OnVoteFail += OnVoteFail;
    }

    protected override void OnUnloadModule()
    {
        Plugin.RemoveCommand("css_extend",  CommandExtendTimeLimit);
        Plugin.RemoveCommand("css_voteextend", CommandVoteExtendTimeLimit);
        
        if (_nativeVoteApi != null)
        {
            _nativeVoteApi.OnVotePass -= OnVotePass;
            _nativeVoteApi.OnVoteFail -= OnVoteFail;
        }
    }
    
    
    [RequiresPermissions(@"css/root")]
    private void CommandExtendTimeLimit(CCSPlayerController? client, CommandInfo info)
    {
        if(info.ArgCount < 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("ExtendTimeLimit.Command.Notification.Usage"));
            return;
        }

        if (!int.TryParse(info.GetArg(1), out var extendTime))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }

        if (!IsTimeLimitBased())
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("ExtendTimeLimit.Command.Notification.IsNotTimeLimitBased"));
            return;
        }

        string executorName = PlayerUtil.GetPlayerName(client);

        if (!ExtendCurrentMap(extendTime))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("ExtendTimeLimit.Command.Notification.ExtendFailed"));
        }
        else
        {
            PrintLocalizedChatToAll("ExtendTimeLimit.Command.Broadcast.TimeChanged", executorName, extendTime);
        }
    }


    private void CommandVoteExtendTimeLimit(CCSPlayerController? client, CommandInfo info)
    {
        if(info.ArgCount < 2)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("VoteExtendTimeLimit.Command.Notification.Usage"));
            return;
        }

        if (!int.TryParse(info.GetArg(1), out var extendTime))
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.InvalidArgumentsInput"));
            return;
        }

        if (!IsTimeLimitBased())
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("ExtendTimeLimit.Command.Notification.IsNotTimeLimitBased"));
            return;
        }
        
        
        var potentialClients = Utilities.GetPlayers().Where(p => p is { IsBot: false, IsHLTV: false }).ToList();
        var potentialClientsIndex = potentialClients.Select(p => p.Index).ToList();

        string detailsString = LocalizeString("VoteExtendTimeLimit.Vote.Text.SubjectText", extendTime);

        float voteThreshold = 50.0F;
        
        
        NativeVoteInfo nInfo = new NativeVoteInfo(NativeVoteIdentifier, "#SFUI_vote_passed_nextlevel_extend",
            detailsString, potentialClientsIndex, VoteThresholdType.Percentage,
            voteThreshold, 15.0f, client?.Slot ?? 99);

        NativeVoteState state = _nativeVoteApi!.InitiateVote(nInfo);

        if (state == NativeVoteState.InitializeAccepted)
        {
            _timeToExtend = extendTime;
            PrintLocalizedChatToAll("VoteExtendTimeLimit.Vote.Broadcast.InitiatedVote", PlayerUtil.GetPlayerName(client));
        }
        else if (state == NativeVoteState.Voting)
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("VoteExtendTimeLimit.Command.Notification.AnotherVoteIsInProgress"));
        }
        else
        {
            info.ReplyToCommand(LocalizeWithPluginPrefix("General.Command.Notification.UnknownError"));
            Logger.LogError($"[VoteExtend] Returned NativeVoteAPI state is not match any normal state: {state}");
        }
    }

    
    private void OnVotePass(YesNoVoteInfo? info)
    {
        if (info == null)
            return;

        if (info.VoteInfo.voteIdentifier != NativeVoteIdentifier)
            return;
        
        ExtendCurrentMap(_timeToExtend);
        PrintLocalizedChatToAll("VoteExtendTimeLimit.Vote.Broadcast.Extended", _timeToExtend);
    }

    private void OnVoteFail(YesNoVoteInfo? info)
    {
        if (info == null)
            return;

        if (info.VoteInfo.voteIdentifier != NativeVoteIdentifier)
            return;

        PrintLocalizedChatToAll("VoteExtendTimeLimit.Vote.Broadcast.FailedToExtend");
    }

    private bool IsTimeLimitBased()
    {
        if (mp_timelimit == null)
            return false;

        return mp_timelimit.GetPrimitiveValue<float>() != 0.0;
    }

    private bool ExtendCurrentMap(float extendTime)
    {
        if(mp_timelimit == null)
            return false;

        float oldTime = mp_timelimit.GetPrimitiveValue<float>();
        float newTime = oldTime + extendTime;

        mp_timelimit.SetValue(newTime);
        
        foreach (CCSPlayerController player in Utilities.GetPlayers())
        {
            if (player.IsHLTV)
                continue;
            
            player.ReplicateConVar(mp_timelimit.Name, mp_timelimit.GetPrimitiveValue<float>().ToString(CultureInfo.InvariantCulture));
        }
        return true;
    }
}