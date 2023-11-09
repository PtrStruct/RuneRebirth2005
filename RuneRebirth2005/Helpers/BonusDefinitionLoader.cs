using Newtonsoft.Json;
using Serilog;

namespace RuneRebirth2005.Helpers;

public class BonusDefinitionLoader
{
    public static Root ItemDefinitions { get; set; }
    public static void Load()
    {
        ItemDefinitions = JsonConvert.DeserializeObject<Root>(File.ReadAllText("C:\\Users\\developer\\Desktop\\ItemDefinition.json"));
        Log.Information("ItemDefinitions loaded..");
    }
}

public class Bonuses
{
    [JsonProperty("short")]
    public List<int> Short { get; set; }
}

public class ItemBonusDefinition
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("bonuses")]
    public Bonuses Bonuses { get; set; }
}

public class List
{
    [JsonProperty("ItemBonusDefinition")]
    public List<ItemBonusDefinition> ItemBonusDefinition { get; set; }
}

public class Root
{
    [JsonProperty("list")]
    public List List { get; set; }
}