using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    // Sample legacy touches before StandaloneInputModule dispatches pointer-up and click.
    [DefaultExecutionOrder(-100)]
    public sealed class MapPanZoomController : ScrollRect
    {
        private const float MinZoom = .45f;
        private const float MaxZoom = 2f;
        private Vector2 focus;
        private bool overview;
        private bool atFocus;
        private float FocusScale => overview && viewport.rect.width > 0 && viewport.rect.height > 0
            ? Mathf.Min(1, viewport.rect.width / content.rect.width, viewport.rect.height / content.rect.height)
            : 1;
        private bool multiTouch;
        private bool pinching;
        private int firstFinger;
        private int secondFinger;
        private Vector2 previousMidpoint;
        private float previousDistance;
        private Canvas mapCanvas;
        private PointerEventData activeDrag;

        private static BaseInput PointerInput => EventSystem.current?.currentInputModule?.input;
        public bool BlocksClick => multiTouch || PointerInput?.touchCount > 1;

        public void SetMap(RectTransform surface, Vector2 focusPoint, bool overviewOnFocus = false)
        {
            viewport = (RectTransform)transform;
            content = surface;
            focus = focusPoint;
            overview = overviewOnFocus;
            movementType = MovementType.Clamped;
            inertia = false;
            mapCanvas = GetComponentInParent<Canvas>()?.rootCanvas;
            Focus();
        }

        public void Focus()
        {
            if (!content) return;
            StopMovement();
            atFocus = true;
            float scale = FocusScale;
            content.localScale = Vector3.one * scale;
            Place(-focus * scale);
            RebaseDrag();
        }

        public override void OnBeginDrag(PointerEventData data)
        {
            if (BlocksClick || data.button != PointerEventData.InputButton.Left) return;
            atFocus = false;
            activeDrag = data;
            base.OnBeginDrag(data);
        }

        public override void OnEndDrag(PointerEventData data)
        {
            base.OnEndDrag(data);
            activeDrag = null;
        }

        public override void OnDrag(PointerEventData data)
        {
            if (!BlocksClick) base.OnDrag(data);
        }

        public override void OnScroll(PointerEventData data)
        {
            if (!IsActive() || BlocksClick || data.scrollDelta.y == 0) return;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, data.position,
                data.enterEventCamera, out var cursor))
            {
                ZoomAround(cursor, cursor, Mathf.Pow(1.15f, data.scrollDelta.y));
                RebaseDrag();
            }
        }

        private void RebaseDrag()
        {
            if (activeDrag != null && !BlocksClick) base.OnBeginDrag(activeDrag);
        }

        private void Update()
        {
            var input = PointerInput;
            int count = input ? input.touchCount : 0;
            if (count == 0 || (count == 1 && input.GetTouch(0).phase == TouchPhase.Began)) multiTouch = false;
            if (count > 1) multiTouch = true;
            if (count != 2 || !content)
            {
                pinching = false;
                return;
            }

            Touch a = input.GetTouch(0);
            Touch b = input.GetTouch(1);
            if (Ended(a) || Ended(b))
            {
                pinching = false;
                return;
            }
            var camera = mapCanvas && mapCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? mapCanvas.worldCamera : null;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, a.position, camera, out var p) ||
                !RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, b.position, camera, out var q)) return;

            var midpoint = (p + q) / 2;
            float distance = Vector2.Distance(p, q);
            int low = Mathf.Min(a.fingerId, b.fingerId);
            int high = Mathf.Max(a.fingerId, b.fingerId);
            if (!pinching || firstFinger != low || secondFinger != high)
            {
                if (!viewport.rect.Contains(p) || !viewport.rect.Contains(q)) return;
                StopMovement();
                pinching = true;
                firstFinger = low;
                secondFinger = high;
            }
            else if (previousDistance > .01f && distance > .01f)
                ZoomAround(previousMidpoint, midpoint, distance / previousDistance);

            previousDistance = distance;
            previousMidpoint = midpoint;
        }

        private static bool Ended(Touch touch) => touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;

        private void ZoomAround(Vector2 from, Vector2 to, float multiplier)
        {
            atFocus = false;
            float previous = content.localScale.x;
            float next = Mathf.Clamp(previous * multiplier, Mathf.Min(MinZoom, FocusScale), MaxZoom);
            var center = viewport.rect.center;
            Vector2 offset = from - center - content.anchoredPosition;
            content.localScale = Vector3.one * next;
            Place(to - center - offset * (next / previous));
        }

        private void Place(Vector2 position)
        {
            // Center smaller maps on each fitting axis and keep larger maps covering the mask.
            var limit = Vector2.Max(Vector2.zero, (content.rect.size * content.localScale.x - viewport.rect.size) / 2);
            SetContentAnchoredPosition(new Vector2(Mathf.Clamp(position.x, -limit.x, limit.x),
                Mathf.Clamp(position.y, -limit.y, limit.y)));
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (!viewport || !content) return;
            if (overview && atFocus) Focus();
            else Place(content.anchoredPosition);
        }

        protected override void OnDisable()
        {
            activeDrag = null;
            pinching = false;
            multiTouch = false;
            base.OnDisable();
        }
    }
}
