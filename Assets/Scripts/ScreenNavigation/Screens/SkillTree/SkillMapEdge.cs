using System.Collections.Generic;
using UnityEngine;

namespace Game.ScreenNavigation
{
    public sealed class SkillMapEdge
    {
        // A null dependency is a presentation-only Core spoke.
        public SkillDependency Dependency { get; }
        public string Source => Dependency?.Source;
        public string Target { get; }
        public bool Primary { get; }
        public IReadOnlyList<Vector2> Points { get; }

        internal SkillMapEdge(SkillDependency dependency, string target, bool primary, List<Vector2> points)
        {
            Dependency = dependency;
            Target = target;
            Primary = primary;
            Points = points;
        }
    }
}
