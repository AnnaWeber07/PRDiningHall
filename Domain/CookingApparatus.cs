using AnnaWebDiningFin.Data.Enums;
using System.Runtime.Serialization;

namespace AnnaWebDiningFin.Domain
{
    public class CookingApparatus
    {
        private static ObjectIDGenerator idGenerator = new();

        public long Id { get; private set; }
        public CookingApparatusType Type { get; private set; }

        public CookingApparatus(CookingApparatusType type)
        {
            Id = idGenerator.GetId(this, out _);

            Type = type;
        }
    }
}
