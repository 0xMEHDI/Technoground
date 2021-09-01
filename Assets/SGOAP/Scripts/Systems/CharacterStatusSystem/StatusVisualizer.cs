using UnityEngine;
using UnityEngine.UI;

namespace SGoap
{
    public class StatusVisualizer : MonoBehaviour
    {
        public RendererViewModifier ViewModifer;
        public CharacterStatusController Status;

        public Image HealthImage;

        private void Awake()
        {
            Status.OnDamageTaken += OnDamageTaken;
        }

        private void OnDestroy()
        {
            Status.OnDamageTaken -= OnDamageTaken;
        }

        private void OnDamageTaken(int hp, int maxHp, IAttacker attacker)
        {
            ViewModifer.Highlight(Color.red, 0.3f);

            if (HealthImage != null)
                HealthImage.fillAmount = (float) hp / maxHp;
        }
    }
}