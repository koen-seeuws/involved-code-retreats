namespace LastStand.Helpers;

public static class ItemsHelper
{
    public static string GetRandomCraftableItem(Game game)
    {
       
        
        
        
        
        
        return Constants.Longbow;
    }
    
    
    
    
    
    public static string GetItemType(string item)
    {
        switch (item)
        {
            //Tool
            case Constants.Sickle
                or Constants.Axe
                or Constants.Pickaxe
                or Constants.Scythe:
                return "main";
            //Weapons (melee)
            case Constants.Dagger
                or Constants.Sword
                or Constants.Hammer
                or Constants.Spear
                or Constants.SwordOfBleeding:
                return "main";
            //Weapons (range)
            case Constants.Sling
                or Constants.Shortbow
                or Constants.Longbow
                or Constants.Crossbow:
                return "main";
            //Additional
            case Constants.WoodenShield
                or Constants.IronShield:
                return "additional";
            //Additional (non-craftable)
            case Constants.CloakOfProtection
                or Constants.BottomlessBag
                or Constants.RingOfSpeed
                or Constants.BeltOfStrength
                or Constants.Horse:
                return "additional";
            //Head
            case Constants.Cap
                or Constants.LeatherHelmet
                or Constants.PlateHelmet:
                return "helmet";
            //Body
            case Constants.Robe
                or Constants.LeatherArmor
                or Constants.PlateArmor:
                return "body";
            //Hands
            case Constants.Gloves
                or Constants.LeatherGloves
                or Constants.PlateGloves:
                return "hands";
            //Legs
            case Constants.Pants
                or Constants.LeatherPants
                or Constants.PlatePants:
                return "legs";
            //Feet
            case Constants.Shoes
                or Constants.LeatherBoots
                or Constants.PlateBoots:
                return "feet";
            default: return string.Empty;
        }
    }
}