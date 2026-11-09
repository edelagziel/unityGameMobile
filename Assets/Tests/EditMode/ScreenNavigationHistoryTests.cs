using Game.ScreenNavigation;
using NUnit.Framework;

namespace Game.ScreenNavigation.Tests
{
    public sealed class ScreenNavigationHistoryTests
    {
        [Test]
        public void PopAtRootDoesNotChangeHistory()
        {
            var history = new ScreenNavigationHistory();
            history.ResetTo(typeof(MainMenuView));

            Assert.IsFalse(history.Pop());
            Assert.AreEqual(1, history.Count);
            Assert.AreEqual(typeof(MainMenuView), history.Current);
        }

        [Test]
        public void PushAndPopRestorePreviousScreen()
        {
            var history = new ScreenNavigationHistory();
            history.ResetTo(typeof(MainMenuView));

            Assert.IsTrue(history.Push(typeof(SettingsView)));
            Assert.AreEqual(typeof(SettingsView), history.Current);
            Assert.IsTrue(history.Pop());
            Assert.AreEqual(typeof(MainMenuView), history.Current);
        }

        [Test]
        public void DuplicatePushDoesNotChangeHistory()
        {
            var history = new ScreenNavigationHistory();
            history.ResetTo(typeof(MainMenuView));
            history.Push(typeof(SettingsView));

            Assert.IsFalse(history.Push(typeof(MainMenuView)));
            Assert.AreEqual(2, history.Count);
            Assert.AreEqual(typeof(SettingsView), history.Current);
        }

        [Test]
        public void ReplaceRejectsAnExistingScreenBelowTop()
        {
            var history = new ScreenNavigationHistory();
            history.ResetTo(typeof(MainMenuView));
            history.Push(typeof(SettingsView));

            Assert.IsFalse(history.Replace(typeof(MainMenuView)));
            Assert.AreEqual(typeof(SettingsView), history.Current);
            Assert.AreEqual(2, history.Count);
        }

        [Test]
        public void ResetClearsPriorHistory()
        {
            var history = new ScreenNavigationHistory();
            history.ResetTo(typeof(MainMenuView));
            history.Push(typeof(SettingsView));

            history.ResetTo(typeof(SettingsView));

            Assert.AreEqual(1, history.Count);
            Assert.IsFalse(history.Pop());
            Assert.AreEqual(typeof(SettingsView), history.Current);
        }
    }
}
