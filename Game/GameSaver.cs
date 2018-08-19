using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Game
{
    public class GameSaver
    {
        private const string HeroSaveExtension = ".hero";
        private readonly DirectoryInfo _savesDirectory;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public GameSaver()
        {
            _savesDirectory = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Game", "Saves"));

            if (!_savesDirectory.Exists)
                _savesDirectory.Create();

            _key = new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 0,
                1, 2, 3, 4, 5, 6, 7, 8, 9, 0,
                1, 2, 3, 4, 5, 6, 7, 8, 9, 0,
                1, 2
            };

            _iv = new byte[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6
            };
        }

        public void SaveNew(Unit hero) => Save(hero, FileMode.CreateNew);
        public void OverwriteSave(Unit hero) => Save(hero, FileMode.Create);

        private void Save(Unit hero, FileMode fileMode)
        {
            var heroSavePath = Path.Combine(_savesDirectory.FullName, $"{hero.Name}{HeroSaveExtension}");

            using (var memoryStream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(Unit));
                serializer.WriteObject(memoryStream, hero);
                memoryStream.Position = 0L;

                using (var memoryStreamReader = new StreamReader(memoryStream))
                using (var fileStream = new FileStream(heroSavePath, fileMode))
                using (var algorithm = Aes.Create())
                using (var encryptor = algorithm.CreateEncryptor(_key, _iv))
                using (var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write))
                using (var cryptoStreamWriter = new StreamWriter(cryptoStream))
                {
                    var dataToEncrypt = memoryStreamReader.ReadToEnd();
                    cryptoStreamWriter.Write(dataToEncrypt);
                    cryptoStreamWriter.Flush();
                }
            }
        }

        public bool SavedHeroExists(string heroName)
        {
            var file = _savesDirectory.EnumerateFiles().SingleOrDefault(x => x.Extension == $"{heroName}{HeroSaveExtension}");

            if (file == null)
                return false;

            return true;
        }

        public IEnumerable<SavedHero> GetSavedHeroes()
        {
            var files = _savesDirectory.EnumerateFiles().Where(x => x.Extension == HeroSaveExtension);

            var id = 0;

            var heroes = new List<SavedHero>();

            var serializer = new DataContractSerializer(typeof(Unit));

            foreach (var file in files)
            {
                using (var fileStream = file.OpenRead())
                using (var algorithm = Aes.Create())
                using (var decryptor = algorithm.CreateDecryptor(_key, _iv))
                using (var cryptoStream = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Read))
                using (var cryptoStreamReader = new StreamReader(cryptoStream))
                using (var memoryStream = new MemoryStream())
                using (var memoryStreamWriter = new StreamWriter(memoryStream))
                {
                    var decrypted = cryptoStreamReader.ReadToEnd();
                    memoryStreamWriter.Write(decrypted);
                    memoryStreamWriter.Flush();
                    memoryStream.Position = 0L;

                    var hero = (Unit)serializer.ReadObject(memoryStream);

                    heroes.Add(new SavedHero(id, hero));
                }

                id++;
            }

            return heroes;
        }
    }
}