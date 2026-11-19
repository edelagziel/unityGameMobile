using System;
using System.Collections.Generic;
using Game.Services.Save;
using NUnit.Framework;

namespace Game.Services.Tests
{
    public sealed class PlayerPrefsSaveServiceTests
    {
        private const string Key = "tests.save-service";
        private PlayerPrefsSaveService save;

        [SetUp]
        public void SetUp()
        {
            save = new PlayerPrefsSaveService();
            save.Delete(Key);
        }

        [TearDown]
        public void TearDown() => save.Delete(Key);

        [Test]
        public void MissingKeyReturnsTheFallback()
        {
            Assert.IsFalse(save.HasKey(Key));
            Assert.AreEqual(7, save.Load(Key, 7));
        }

        [Test]
        public void IntSurvivesARoundTrip()
        {
            save.Save(Key, 42);
            Assert.IsTrue(save.HasKey(Key));
            Assert.AreEqual(42, save.Load(Key, 0));
        }

        [Test]
        public void BoolSurvivesARoundTrip()
        {
            save.Save(Key, true);
            Assert.IsTrue(save.Load(Key, false));
        }

        [Test]
        public void StringSurvivesARoundTrip()
        {
            save.Save(Key, "forge");
            Assert.AreEqual("forge", save.Load(Key, string.Empty));
        }

        [Test]
        public void SerializableClassSurvivesARoundTrip()
        {
            save.Save(Key, new Progress { stage = 3, cards = new List<string> { "archers", "freeze" } });

            var loaded = save.Load<Progress>(Key);

            Assert.AreEqual(3, loaded.stage);
            CollectionAssert.AreEqual(new[] { "archers", "freeze" }, loaded.cards);
        }

        [Test]
        public void DeleteRemovesTheValue()
        {
            save.Save(Key, 1);
            save.Delete(Key);
            Assert.IsFalse(save.HasKey(Key));
        }

        [Test]
        public void EmptyKeyIsRejected()
        {
            Assert.Throws<ArgumentException>(() => save.Save(string.Empty, 1));
        }

        [Serializable]
        private sealed class Progress
        {
            public int stage;
            public List<string> cards = new List<string>();
        }
    }
}
