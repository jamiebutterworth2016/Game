using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Game
{
    public class GameSaver
    {
        private const string HeroSaveExtension = ".hero";

        private readonly string _savesPath;

        public GameSaver()
        {
            _savesPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Game", "Saves");
        }

        public void SaveNew(Unit hero) => Save(hero, FileMode.CreateNew);

        public void OverwriteSave(Unit hero) => Save(hero, FileMode.Create);

        private void Save(Unit hero, FileMode fileMode)
        {
            if (!Directory.Exists(_savesPath))
                Directory.CreateDirectory(_savesPath);

            var heroSavePath = Path.Combine(_savesPath, $"{hero.Name}{HeroSaveExtension}");

            using (var stream = new FileStream(heroSavePath, fileMode))
            {
                var serializer = new DataContractSerializer(typeof(Unit));
                serializer.WriteObject(stream, hero);
            };

        }

        public bool SaveExists(string heroName)
        {
            var heroSavePath = Path.Combine(_savesPath, heroName);

            if (!Directory.Exists(heroSavePath))
                return false;

            return true;
        }

        public IEnumerable<SavedHero> GetSavedHeroes()
        {
            var savesDirectory = new DirectoryInfo(_savesPath);

            var files = savesDirectory.EnumerateFiles().Where(x => x.Extension == HeroSaveExtension);

            var id = 0;

            var heroes = new List<SavedHero>();

            var serializer = new DataContractSerializer(typeof(Unit));

            foreach (var file in files)
            {
                using (var stream = file.OpenRead())
                {
                    var readObject = serializer.ReadObject(stream);
                    var hero = (Unit)readObject;

                    heroes.Add(new SavedHero(id, hero));
                }

                id++;
            }

            return heroes;
        }

        public Unit GetSavedHero(string heroName)
        {
            var heroSavePath = Path.Combine(_savesPath, $"{heroName}{HeroSaveExtension}");

            using (var stream = new FileStream(heroSavePath, FileMode.Open))
            {
                var serializer = new DataContractSerializer(typeof(Unit));
                var readObject = serializer.ReadObject(stream);
                var hero = (Unit)readObject;

                return hero;
            };
        }
    }
}