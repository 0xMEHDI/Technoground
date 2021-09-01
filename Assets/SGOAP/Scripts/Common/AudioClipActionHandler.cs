using System.Collections;
using SGoap;
using UnityEngine;

namespace Games.Common
{
    public class AudioClipActionHandler : MonoBehaviour
    {
        public BasicAction Action;
        public AudioSource AudioSource;

        [MinMax(-3,3)]
        public RangeValue PitchRange = new RangeValue(0.9f,1.1f);
        public float PerformBetweenDelay;

        public AudioClip PrePerformClip;
        public AudioClip PerformClip;
        public AudioClip PostPerformClip;

        public float PrePerformDelay;

        public CoolDown PerformCooldown = new CoolDown();

        private void Awake()
        {
            Action.OnPrePerform += OnPrePerform;
            Action.OnPostPerform += OnPostPerform;
            Action.OnPerform += OnPerform;
        }

        private void OnPerform()
        {
            if (PerformCooldown.Active)
                return;

            if (!AudioSource.isPlaying)
            {
                AudioSource.pitch = PitchRange.GetRandomValue();
                AudioSource.clip = PerformClip;
                AudioSource.Play();

                PerformCooldown.Run(PerformBetweenDelay);
            }
        }

        private void OnPrePerform()
        {
            StartCoroutine(Routine());

            IEnumerator Routine()
            {
                yield return new WaitForSeconds(PrePerformDelay);

                if (PrePerformClip != null)
                    AudioSource.PlayOneShot(PrePerformClip, 1);
            }

        }

        private void OnPostPerform()
        {
            if(PostPerformClip != null)
                AudioSource.PlayOneShot(PostPerformClip, 1);
        }
    }
}