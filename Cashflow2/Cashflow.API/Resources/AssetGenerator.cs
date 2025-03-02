using Cashflow.API.Entities;

namespace Cashflow.API.Resources;

public static class AssetGenerator
{
    private const int BIG_DEAL_MIN_COST = 5000;
    private const int BIG_DEAL_MAX_COST = 100000;
    private const int SMALL_DEAL_MIN_COST = 500;
    private const int SMALL_DEAL_MAX_COST = 8000;

    public static Asset GenerateBigDeal()
    {
        Random random = new();
        AssetType type = GetRandomWeightedAsset(
            random,
            new Dictionary<AssetType, int>
            {
                { AssetType.apartment, 3 },
                { AssetType.business, 3 },
                { AssetType.threeTwo, 3 },
                { AssetType.land, 1 }
            });

        Asset asset = new()
        {
            Type = type,
            Quantity = type switch
            {
                AssetType.apartment => random.Next(4, 16),
                AssetType.land => random.Next(10, 100),
                _ => null
            },
        };

        asset.Name = GenerateAssetName(asset, true);
        asset.Equity = GenerateEquityAmount(random, BIG_DEAL_MIN_COST, BIG_DEAL_MAX_COST, asset.Type == AssetType.land ? asset.Quantity / 10 : asset.Quantity);
        asset.Value = GenerateValue(random, 1, 10, asset.Type, asset.Equity);
        asset.RateOfReturn = GenerateRoR(random, 0, 5, asset.Type);

        return asset;
    }

    public static Asset GenerateSmallDeal()
    {
        Random random = new();
        AssetType type = GetRandomWeightedAsset(
            random,
            new Dictionary<AssetType, int>
            {
                { AssetType.apartment, 2 },
                { AssetType.business, 1 },
                { AssetType.threeTwo, 2 },
                { AssetType.land, 2 },
                { AssetType.cd, 2 },
                { AssetType.gold, 1 },
                { AssetType.mlm1, 1 },
                { AssetType.mlm2, 1 },
                { AssetType.twoOne, 3 },
            });

        Asset asset = new()
        {
            Type = type,
            Quantity = type switch
            {
                AssetType.apartment => 2,
                AssetType.land => 1,
                _ => null
            }
        };

        asset.Name = GenerateAssetName(asset, true);
        asset.Equity = GenerateEquityAmount(random, SMALL_DEAL_MIN_COST, SMALL_DEAL_MAX_COST, asset.Quantity);
        asset.Value = GenerateValue(random, 8, 20, asset.Type, asset.Equity);
        asset.RateOfReturn = GenerateRoR(random, -1, 5, asset.Type);

        asset.Name = GenerateAssetName(asset, false);

        return asset;
    }

    public static AssetType GetRandomWeightedAsset(Random random, Dictionary<AssetType, int> weights)
    {
        // Flatten the weighted pool into a list based on weights
        var weightedList = weights.SelectMany(pair => Enumerable.Repeat(pair.Key, pair.Value)).ToList();

        // Select a random item from the flattened list
        return weightedList[random.Next(weightedList.Count)];
    }

    private static decimal GenerateRoR(Random random, int min, int max, AssetType type)
    {
        if (new List<AssetType> { AssetType.apartment, AssetType.business, AssetType.threeTwo, AssetType.twoOne, AssetType.cd, AssetType.land }.Contains(type))
        {
            return random.Next(min, max) / 100.0M;
        }

        return 0;
    }

    private static decimal GenerateValue(Random random, int min, int max, AssetType type, decimal equity)
    {
        if (new List<AssetType> { AssetType.apartment, AssetType.business, AssetType.threeTwo, AssetType.twoOne, AssetType.land }.Contains(type))
        {
            return equity * random.Next(min, max);
        }

        return equity;
    }

    private static decimal GenerateEquityAmount(Random random, int min, int max, int? multiplier)
    {
        return multiplier == null ? random.Next(min, max) : random.Next(min * (multiplier ?? 0), max);
    }

    private static string GenerateAssetName(Asset asset, bool isBig)
    {
        return asset.Type switch
        {
            AssetType.mlm1 => "Direct 2 You - Level 1",
            AssetType.mlm2 => "Direct 2 You - Level 2",
            AssetType.business => isBig ? "Business" : "Small Business",
            AssetType.twoOne => "Condo - 2br/1ba",
            AssetType.threeTwo => "House - 3br/2ba",
            AssetType.apartment => asset.Quantity == 2 ? "Duplex" : $"{asset.Quantity}-Plex",
            AssetType.cd => "Certificate of Deposit",
            AssetType.land => $"Land - {asset.Quantity}0 acres",
            AssetType.gold => "Gold Coins",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}