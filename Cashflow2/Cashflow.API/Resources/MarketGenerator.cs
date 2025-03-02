using Cashflow.API.Entities;

namespace Cashflow.API.Resources;

public static class MarketGenerator
{
    public static PurchaseOffer GenerateAssetOffer()
    {
        Random random = new();

        PurchaseOffer offer = new()
        {
            Type = AssetGenerator.GetRandomWeightedAsset(
                random,
                new Dictionary<AssetType, int>
                {
                    { AssetType.apartment, 8 },
                    { AssetType.business, 4 },
                    { AssetType.threeTwo, 10 },
                    { AssetType.land, 4 },
                    { AssetType.cd, 4 },
                    { AssetType.gold, 1 },
                    { AssetType.mlm2, 1 },
                    { AssetType.twoOne, 6 },
                })
        };

        offer.Price = offer.Type switch
        {
            AssetType.mlm1 => throw new ArgumentOutOfRangeException($"mlm1 should not be offered for purchase"),
            AssetType.mlm2 => random.Next(20, 500),
            AssetType.business => random.Next(8000, 1000000),
            AssetType.twoOne => random.Next(8000, 150000),
            AssetType.threeTwo => random.Next(8000, 500000),
            AssetType.apartment => random.Next(8000, 100000),
            AssetType.cd => random.Next(500, 8000),
            AssetType.land => random.Next(1000, 50000),
            AssetType.gold => random.Next(500, 8000),
            _ => throw new ArgumentOutOfRangeException()
        };

        offer.Name = offer.Type switch
        {
            AssetType.mlm1 => throw new ArgumentOutOfRangeException($"mlm1 should not be offered for purchase"),
            AssetType.mlm2 => "Someone offers to buy your downstream!",
            AssetType.business => "Business buyer approaches you",
            AssetType.twoOne => "Condo buyer",
            AssetType.threeTwo => "House buyer",
            AssetType.apartment => "Apartment buyer, price offered is per unit",
            AssetType.cd => "Certificate of Deposit buyer",
            AssetType.land => "Land buyer, price offered is per 10 acres",
            AssetType.gold => "Gold buyer",
            _ => throw new ArgumentOutOfRangeException()
        };

        return offer;
    }
}