using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;

namespace MagicVilla_Api.Data
{
    public static class VillaStore
    {
        public static List<VillaDto> villaList = new List<VillaDto>
        {


                new VillaDto{Id = 1, Name="Pool View", Sqft=100, Occupancy=4},
                new VillaDto{Id = 2, Name="Beach View", Sqft=300, Occupancy=3 }

        };
    }
}
