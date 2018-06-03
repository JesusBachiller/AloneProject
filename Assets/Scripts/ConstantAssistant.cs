public class ConstantAssistant
{
    //Player
    public const int INVENTORY_SLOT = 4;
    public const float MAX_DIST_TO_SELECT_OBJECT = 20f;

    //NATURAL RESOURCES
    public const float COLLECTION_TIME_TREE = 1f;//2f
    public const int NUMBER_OF_EXTRACTIONS_TREE = 4;

    public const float COLLECTION_TIME_ROCK = 2f;
    public const int NUMBER_OF_EXTRACTIONS_ROCK = 4;

    public const float COLLECTION_TIME_COAL_MINE = 1f;//4f;
    public const int NUMBER_OF_EXTRACTIONS_COAL_MINE = 5;

    public const float COLLECTION_TIME_IRON_MINE = 1f;//7f;
    public const int NUMBER_OF_EXTRACTIONS_IRON_MINE = 6;

    public const float COLLECTION_TIME_STONE = 0f;
    public const int NUMBER_OF_EXTRACTIONS_STONE = 1;

    public const float COLLECTION_TIME_HERB = 0f;
    public const int NUMBER_OF_EXTRACTIONS_HERB = 1;

    public enum EnumNoNaturalResources
    {
        empty = -1,
        CookedSeed,
        Seed,
        Coal,
        Iron,
        IronPlate,
        Rope,
        Steel,
        Stone,
        TreeTrunk,
        WoodenPlate,
        WoodenStick,
        Anvil,
        Box,
        IronFurnace,
        IronTable,
        StoneFurnace,
        WoodTable,
        IronAxe,
        IronPeak,
        StoneAxe,
        StonePeak
    }

    //Modes open inventory canvas buttons
    public const int MODE_INVENTORY_PLAYER = 0;
    public const int MODE_WOOD_TABLE = 1;
    public const int MODE_IRON_TABLE = 2;
    public const int MODE_STONE_FURNACE = 3;
    public const int MODE_IRON_FURNACE = 4;
    public const int MODE_ANVIL = 5;
}