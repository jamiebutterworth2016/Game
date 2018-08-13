namespace Game
{
    public class SavedHero
    {
        public SavedHero(int id, Unit hero)
        {
            Id = id;
            Hero = hero;
        }

        public int Id { get; set; }
        public Unit Hero { get; set; }
    }
}