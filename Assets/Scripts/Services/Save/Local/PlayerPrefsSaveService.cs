using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Services.Save
{
    /// <summary>
    /// Saves on the device: every value as JSON in PlayerPrefs.
    /// This is the only class in the project that touches PlayerPrefs.
    /// </summary>
    public sealed class PlayerPrefsSaveService : ISaveService, IInitializableService
    {
        // Keeps our keys apart from PlayerPrefs written by Unity or by plugins.
        private const string KeyPrefix = "save.";

        // PlayerPrefs is ready as soon as the app starts, so there is nothing to load up front.
        public Task InitializeAsync(CancellationToken cancellation) => Task.CompletedTask;

        public bool HasKey(string key) => PlayerPrefs.HasKey(StorageKey(key));

        public T Load<T>(string key, T fallback = default)
        {
            var json = PlayerPrefs.GetString(StorageKey(key), string.Empty);
            if (json.Length == 0) return fallback;

            try
            {
                return JsonUtility.FromJson<Box<T>>(json).value;
            }
            catch (ArgumentException exception)
            {
                // Broken or old data must not crash the game: keep playing with the default.
                Debug.LogWarning($"Save: could not read '{key}', using the default. {exception.Message}");
                return fallback;
            }
        }

        public void Save<T>(string key, T value)
        {
            var storageKey = StorageKey(key);
            bool existed = PlayerPrefs.HasKey(storageKey);
            string previous = existed ? PlayerPrefs.GetString(storageKey) : null;
            try
            {
                PlayerPrefs.SetString(storageKey, JsonUtility.ToJson(new Box<T> { value = value }));
                PlayerPrefs.Save();
            }
            catch
            {
                // A later flush must not publish a candidate rejected by this save.
                if (existed) PlayerPrefs.SetString(storageKey, previous);
                else PlayerPrefs.DeleteKey(storageKey);
                throw;
            }
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(StorageKey(key));
            PlayerPrefs.Save();
        }

        private static string StorageKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("A save key cannot be empty.", nameof(key));
            return KeyPrefix + key;
        }

        // JsonUtility serializes objects only, so a bare int or bool is wrapped in a box.
        [Serializable]
        private sealed class Box<T>
        {
            public T value;
        }
    }
}
