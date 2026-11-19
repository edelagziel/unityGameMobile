namespace Game.Services.Save
{
    /// <summary>
    /// Stores player progress by key. Callers say what to save, never how:
    /// today it is <see cref="PlayerPrefsSaveService"/> on the phone, later it can be a cloud save,
    /// and nothing that uses this interface has to change.
    /// </summary>
    public interface ISaveService
    {
        bool HasKey(string key);

        /// <summary>Returns the saved value, or <paramref name="fallback"/> if nothing was saved under this key.</summary>
        T Load<T>(string key, T fallback = default);

        /// <summary>
        /// Saves any value JsonUtility can serialize: numbers, bools, strings,
        /// [Serializable] classes and lists. Not dictionaries or DateTime.
        /// </summary>
        void Save<T>(string key, T value);

        void Delete(string key);
    }
}
