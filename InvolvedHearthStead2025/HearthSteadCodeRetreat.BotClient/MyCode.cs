using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using HearthStead.Dto;
using HearthStead.Dto.Enums;
using HearthStead.Dto.Upgrades;

namespace HearthSteadCodeRetreat.BotClient
{
    internal static class MyCode
    {
        public static async Task Do(HearthSteadDto hearthSteadDto, HearthSteadHttpClient _client)
        {
            Console.Clear();
            Console.WriteLine("started " + hearthSteadDto.CurrentDay);

            Console.WriteLine(hearthSteadDto.CharcoalKiln.GetStuff()); Console.WriteLine();
            Console.WriteLine(hearthSteadDto.BlackSmith.GetStuff()); Console.WriteLine();
            Console.WriteLine(hearthSteadDto.GuardTower.GetStuff()); Console.WriteLine();

            Console.WriteLine(hearthSteadDto.Workshop.GetStuff()); Console.WriteLine();

            Console.WriteLine(hearthSteadDto.SawMill.GetStuff()); Console.WriteLine();

            Console.WriteLine(hearthSteadDto.Storage.GetStuff()); Console.WriteLine();
            Console.WriteLine(hearthSteadDto.TrainingGrounds.GetStuff()); Console.WriteLine(); Console.WriteLine();


            Console.WriteLine(hearthSteadDto.StoneMine.GetProduction());
            Console.WriteLine(hearthSteadDto.Forest.GetProduction());

            Console.WriteLine();

            

            await _client.Repair(hearthSteadDto.Workshop.Name);
            //await _client.CraftingMakePlanks();

            //await _client.MineStone();
        }

        private static bool CanUpgrade(HearthSteadDto dto, UpgradeDto cost)
        {
            if (dto.Storage.MaterialStorage.First(a => a.MaterialType == MaterialTypeDto.Wood).MaterialQuantity <
                cost.UpgradeCost.First(a => a.MaterialType == MaterialTypeDto.Wood).MaterialQuantity) return false;


            if (dto.Storage.MaterialStorage.First(a => a.MaterialType == MaterialTypeDto.Charcoal).MaterialQuantity <
                cost.UpgradeCost.First(a => a.MaterialType == MaterialTypeDto.Charcoal).MaterialQuantity) return false;

            if (dto.Storage.MaterialStorage.First(a => a.MaterialType == MaterialTypeDto.Ingots).MaterialQuantity <
                cost.UpgradeCost.First(a => a.MaterialType == MaterialTypeDto.Ingots).MaterialQuantity) return false;
            if (dto.Storage.MaterialStorage.First(a => a.MaterialType == MaterialTypeDto.Food).MaterialQuantity <
                cost.UpgradeCost.First(a => a.MaterialType == MaterialTypeDto.Food).MaterialQuantity) return false;

            if (dto.Storage.MaterialStorage.First(a => a.MaterialType == MaterialTypeDto.Stone).MaterialQuantity <
                cost.UpgradeCost.First(a => a.MaterialType == MaterialTypeDto.Stone).MaterialQuantity) return false;

            if (dto.Storage.MaterialStorage.First(a => a.MaterialType == MaterialTypeDto.Planks).MaterialQuantity <
                cost.UpgradeCost.First(a => a.MaterialType == MaterialTypeDto.Planks).MaterialQuantity) return false;

            if (dto.Storage.MaterialStorage.First(a => a.MaterialType == MaterialTypeDto.Ore).MaterialQuantity <
                cost.UpgradeCost.First(a => a.MaterialType == MaterialTypeDto.Ore).MaterialQuantity) return false;

            return true;
        }
    }
}
