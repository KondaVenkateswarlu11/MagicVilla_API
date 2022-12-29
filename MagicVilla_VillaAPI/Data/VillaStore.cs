using MagicVilla_VillaAPI.Models.Dto;

namespace MagicVilla_VillaAPI.Data
{
    public class VillaStore
    {
        public static List<VillaDto> villaList = new List<VillaDto>
            {
                new VillaDto {Id = 1,Name="Pool View",Sqft = 1000,Occupancy = 5 },
                new VillaDto {Id = 2,Name="Beach View",Sqft = 2000,Occupancy = 10}
            };
    }
}
