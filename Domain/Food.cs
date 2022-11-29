using AnnaWebDiningFin.Data.Enums;

namespace AnnaWebDiningFin.Domain
{
    public class Food
    {
        public long Id { get; private set; }
        public string Name { get; private set; }
        public int PreparationTime { get; private set; }
        public int Comlexity { get; private set; }
        public CookingApparatusType? CookingApparatus { get; private set; }

        public Food(long id, string name, int preparationTime, int complexity, CookingApparatusType? cookingApparatus)
        {
            Id = id;
            Name = name;
            PreparationTime = preparationTime;
            Comlexity = complexity;
            CookingApparatus = cookingApparatus;
        }
    }
}