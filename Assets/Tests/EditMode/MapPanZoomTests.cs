using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.ScreenNavigation.Tests
{
    public sealed class MapPanZoomTests
    {
        [TestCase(720, 1280)]
        [TestCase(1080, 2400)]
        public void PortraitViewportClampsBothAxesAndCentersContentThatFits(int width, int height)
        {
            var root = new GameObject("Portrait UI", typeof(RectTransform));
            try
            {
                var screen = (RectTransform)root.transform;
                screen.sizeDelta = new Vector2(width, height);
                var viewport = (RectTransform)new GameObject("Map viewport", typeof(RectTransform)).transform;
                viewport.SetParent(screen, false);
                viewport.anchorMin = new Vector2(.02f, .39f);
                viewport.anchorMax = new Vector2(.98f, .85f);
                viewport.offsetMin = viewport.offsetMax = Vector2.zero;
                var map = viewport.gameObject.AddComponent<MapPanZoomController>();
                var surface = (RectTransform)new GameObject("Map", typeof(RectTransform)).transform;
                surface.SetParent(viewport, false);
                surface.sizeDelta = new Vector2(2000, 1700);
                var focus = new Vector2(-125, 130);
                map.SetMap(surface, focus);
                Assert.That(Vector2.Distance(surface.anchoredPosition, -focus), Is.LessThan(.01f));

                var pointer = new PointerEventData(null) { scrollDelta = Vector2.up * 100 };
                map.OnScroll(pointer);
                map.OnBeginDrag(pointer);
                pointer.position = new Vector2(10000, -10000);
                map.OnDrag(pointer);
                map.OnEndDrag(pointer);
                var limit = (surface.rect.size * surface.localScale.x - viewport.rect.size) / 2;
                Assert.That(surface.anchoredPosition.x, Is.EqualTo(limit.x).Within(.01f));
                Assert.That(surface.anchoredPosition.y, Is.EqualTo(-limit.y).Within(.01f));

                map.Focus();
                screen.sizeDelta = surface.sizeDelta * 3;
                Assert.That(surface.anchoredPosition.magnitude, Is.LessThan(.01f));
                Assert.AreEqual(Vector3.one, surface.localScale);
                map.Focus();
                Assert.That(surface.anchoredPosition.magnitude, Is.LessThan(.01f));
            }
            finally { Object.DestroyImmediate(root); }
        }
    }
}
