using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.ScreenNavigation.Tests
{
    public sealed class SkillMapTestInput : BaseInput
    {
        public Touch[] Fingers = Array.Empty<Touch>();
        public override int touchCount => Fingers.Length;
        public override Touch GetTouch(int index) => Fingers[index];
        public override bool touchSupported => true;
        public override bool mousePresent => true;
        public Vector2 Cursor;
        public Vector2 Wheel;
        public bool Press;
        public bool Release;
        public override Vector2 mousePosition => Cursor;
        public override Vector2 mouseScrollDelta => Wheel;
        public override bool GetMouseButtonDown(int button) => button == 0 && Press;
        public override bool GetMouseButtonUp(int button) => button == 0 && Release;
    }
}
