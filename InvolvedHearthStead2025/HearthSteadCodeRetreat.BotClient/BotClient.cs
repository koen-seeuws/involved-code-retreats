using HearthStead.Dto;
using HearthStead.Dto.Enums;
using HearthStead.Dto.Messages;
using HearthStead.Dto.Villagers;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;
using HearthStead.Dto.Structures;

namespace HearthSteadCodeRetreat.BotClient;

public class BotClient
{
    private readonly HearthSteadHttpClient _client;

    public BotClient(HearthSteadHttpClient client)
    {
        _client = client;
    }

    public async Task Run()
    {
        // Settle villager aka enroll/register
        await _client.SettleVillager();
        // Get token
        await _client.GetEndorsement();

        var connection = new HubConnectionBuilder().WithUrl($"{_client.BaseUrl}/HearthSteadHub", options => { })
            .Build();

        await connection.StartAsync();

        // New day started for HeartStead
        // What will you do today?
        connection.On("NewDay", async (HearthSteadDto hearthSteadDto) =>
        {
            if (hearthSteadDto.CurrentDay == 1)
            {
                await _client.Farm();
            }
            else
            {
                switch (_client.GetVillagerName())
                {
                    case "Bjert":
                        var bert = await UpgradeStuff.Upgrade(_client, hearthSteadDto);
                        if (!bert)
                            bert = await FarmingStuff.Run(hearthSteadDto, _client);
                        if (!bert)
                            bert = await WoodWorkingStuff.KoenWood(_client, hearthSteadDto);
                        if (!bert)
                        {
                            var ikke = hearthSteadDto.GetMyVillager(_client.GetVillagerName());
                            if (ikke.Skills.Single(x => x.Name == "Combat").Proficiency != "Virtuoso")
                            {
                                Console.WriteLine($"{_client.GetVillagerName()}: Training!");
                                await _client.Train();
                                break;
                            }
                        }
                        if (!bert)
                            bert = await MiningStuff.DoMining(hearthSteadDto, _client);
                        if (!bert)
                            await Jezus.Run(hearthSteadDto, _client);
                        break;
                    case "Koen":
                        var koen = await WoodWorkingStuff.KoenWood(_client, hearthSteadDto);
                        if (!koen)
                            koen = await MiningStuff.DoMining(hearthSteadDto, _client);
                        if(!koen)
                            koen = await FarmingStuff.Run(hearthSteadDto, _client);
                        if (!koen)
                        {
                            var ikke = hearthSteadDto.GetMyVillager(_client.GetVillagerName());
                            if (ikke.Skills.Single(x => x.Name == "Combat").Proficiency != "Virtuoso")
                            {
                                Console.WriteLine($"{_client.GetVillagerName()}: Training!");
                                await _client.Train();
                                break;
                            }
                        }
                        if (!koen)
                            await Jezus.Run(hearthSteadDto, _client);
                        break;
                    case "Cookie":
                        var Cookie = await WoodWorkingStuff.KoenWood(_client, hearthSteadDto);
                        if ( Cookie)
                            Cookie = await MiningStuff.DoMining(hearthSteadDto, _client);
                        if( Cookie)
                            Cookie = await FarmingStuff.Run(hearthSteadDto, _client);
                        if ( Cookie)
                        {
                            var ikke = hearthSteadDto.GetMyVillager(_client.GetVillagerName());
                            if (ikke.Skills.Single(x => x.Name == "Combat").Proficiency != "Virtuoso")
                            {
                                Console.WriteLine($"{_client.GetVillagerName()}: Training!");
                                await _client.Train();
                                break;
                            }
                        }
                        if ( Cookie)
                            await Jezus.Run(hearthSteadDto, _client);
                        break;
                    case "DeKoeke":
                        var dekoeke = await UpgradeStuff.Upgrade(_client, hearthSteadDto, offset: 1);
                        if (!dekoeke)
                            dekoeke = await FarmingStuff.Run(hearthSteadDto, _client);
                        if (!dekoeke)
                            dekoeke = await WoodWorkingStuff.KoenWood(_client, hearthSteadDto);
                        if (!dekoeke)
                        {
                            var ikke = hearthSteadDto.GetMyVillager(_client.GetVillagerName());
                            if (ikke.Skills.Single(x => x.Name == "Combat").Proficiency != "Virtuoso")
                            {
                                Console.WriteLine($"{_client.GetVillagerName()}: Training!");
                                await _client.Train();
                                break;
                            }
                        }
                        if (!dekoeke)
                            dekoeke = await MiningStuff.DoMining(hearthSteadDto, _client);
                        if (!dekoeke)
                            await Jezus.Run(hearthSteadDto, _client);
                        break;
                    case "Kabouter Wesley":
                    case "Ruq":
                        await FarmingStuff.Run(hearthSteadDto, _client);
                        break;
                    case "Ante":
                        break;
                    case "Djohnnie":
                        var gedaan = await MiningStuff.DoMining(hearthSteadDto, _client);
                        if (!gedaan)
                        {
                            var ikke = hearthSteadDto.GetMyVillager(_client.GetVillagerName());
                            if (ikke.Skills.Single(x => x.Name == "Combat").Proficiency != "Virtuoso")
                            {
                                Console.WriteLine("Training!");
                                await _client.Train();
                            }
                        }
                        break;
                    case "NickThys":
                        var nick = await WoodWorkingStuff.NickWood(_client, hearthSteadDto);
                        if (!nick)
                            nick = await MiningStuff.DoMining(hearthSteadDto, _client);
                        if (!nick)
                            await Jezus.Run(hearthSteadDto, _client);
                        break;
                    case "Kevin":
                    {
                        await Jezus.Run(hearthSteadDto, _client);
                        break;
                    }
                }
            }
        });


        // A villager posted a message on the message board
        connection.On("MessagePosted", async (MessageDto message) =>
        {
            try
            {
                var structuredMessage = StructuredMessage.Deserialize(message.Content);
                if (structuredMessage.Target == _client.GetVillagerName())
                {
                    Console.WriteLine($"Message for me: {message.Author} => {message.Content}");
                    switch (structuredMessage.ActionRequested)
                    {
                        case ActionToPerform.Revive:
                            await _client.Revitalize(structuredMessage.Context!);
                            break;
                        case ActionToPerform.Extinguish:
                            await _client.Extinguish(structuredMessage.Context!);
                            break;
                        case ActionToPerform.Repair:
                            await _client.Repair(structuredMessage.Context!);
                            break;
                        case ActionToPerform.Defend:
                            await _client.Defend();
                            break;
                    }

                    await _client.RemoveMessage(message.Id);
                }
            }
            catch
            {
            }
        });
    }
}

public class StructuredMessage
{
    public string Target { get; set; }
    public ActionToPerform ActionRequested { get; set; }
    public string? Context { get; set; }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    public static StructuredMessage Deserialize(string json)
    {
        return JsonSerializer.Deserialize<StructuredMessage>(json);
    }
}

public enum ActionToPerform
{
    Revive = 0,
    Extinguish = 1,
    Repair = 2,
    Defend = 3
}

public static class HearthSteadDtoExtensions
{
    public static async Task AmIDead(this HearthSteadDto hearthSteadDto, HearthSteadHttpClient client)
    {
        var me = hearthSteadDto.GetMyVillager(client.GetVillagerName());
        if (hearthSteadDto.AmIDead(client.GetVillagerName()))
        {
            var healthyVillager = hearthSteadDto.Villagers
                .OrderByDescending(x => x.Health)
                .First();

            var structuredMessage = new StructuredMessage
            {
                Target = healthyVillager.Name,
                ActionRequested = ActionToPerform.Revive
            };

            await client.PostMessage(structuredMessage.Serialize());
        }
    }
}

public static class HearthSteadExtensions
{
    public static VillagerDto GetMyVillager(this HearthSteadDto dto, string name)
    {
        return dto.Villagers.First(x => x.Name == name);
    }

    public static bool AmIDead(this HearthSteadDto dto, string name)
    {
        return dto.GetMyVillager(name).Health == HealthStatusDto.Moribund;
    }

    public static bool VillageNeedsProtection(this HearthSteadDto dto)
    {
        return dto.RaidSpotted && dto.DaysUntilRaid == 1;
    }

    public static int GetResourceCount(this HearthSteadDto dto, MaterialTypeDto type)
    {
        return dto.Storage.MaterialStorage.First(x => x.MaterialType == type).MaterialQuantity;
    }

    public static bool CanProduce(this StructureDto structureDto, StorageDto storageDto)
    {
        if (structureDto.OnFire || structureDto.Status == StructureStatusDto.Dilapidated) return false;
        if (structureDto.ProductionCost == null) return false;

        foreach (var productionCost in structureDto.ProductionCost)
        {
            var storedMaterial =
                storageDto.MaterialStorage.Single(m => m.MaterialType == productionCost.MaterialType);
            if (storedMaterial.MaterialQuantity < productionCost.MaterialQuantity) return false;
        }

        return true;
    }

    //Farming
    public enum FarmSide
    {
        Left,
        Right
    }

    public static string? GetFarmFieldToFarm(this HearthSteadDto dto, FarmSide side)
    {
        if (dto.Farm.FarmFields.Count < 10)
            return null;

        var farmFields = dto.Farm.FarmFields.Where(_ => _.Status != FarmStatusDto.Growing).ToList();

        if (farmFields.Count == 0) return "";

        var randomFieldInt = Random.Shared.Next(0, farmFields.Count);

        return farmFields[randomFieldInt].FieldName;
    }
}