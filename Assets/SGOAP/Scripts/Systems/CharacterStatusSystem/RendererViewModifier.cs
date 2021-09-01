using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGoap
{
    public class RendererViewModifier : MonoBehaviour
    {
        private Renderer[] _renderers;

        private Dictionary<Material, Color> _materialColorMap = new Dictionary<Material, Color>();
        private Dictionary<Material, Color> _materialCurrentColorMap = new Dictionary<Material, Color>();
        private List<Material> _materials = new List<Material>();

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>(includeInactive: true);

            foreach (var renderer in _renderers)
            {
                var color = renderer.material.color;

                _materials.Add(renderer.material);
                _materialColorMap.Add(renderer.material, color);
                _materialCurrentColorMap.Add(renderer.material, color);
            }
        }

        public void Highlight(Color color, float duration = 0.5F)
        {
            if (gameObject == null)
                return;

            StartCoroutine(Routine());

            IEnumerator Routine()
            {
                foreach (var material in _materials)
                    material.Lerp(color * 10, duration);

                yield return new WaitForSeconds(duration);

                foreach (var material in _materials)
                    material.Lerp(_materialColorMap[material], duration);
            }
        }
    }
}