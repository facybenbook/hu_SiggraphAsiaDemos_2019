using System;
using UnityEngine;
using Devdog.General.UI;

namespace Devdog.General
{
    public partial class Trigger : TriggerBase
    {
        public bool handleWindowDirectly
        {
            get
            {
                if (windowContainer != null && windowContainer.Equals(null) == false)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Only required if handling the window directly
        /// </summary>
        [Header("The window")]
        [SerializeField]
        private UIWindowField _window;
        public UIWindowField window
        {
            get { return _window; }
            set { _window = value; }
        }

        /// <summary>
        /// The window this trigger will use;
        /// If a ITriggerWindowContainer is present it will grab it's window, if not the UIWindowField (this.window) will be used.
        /// </summary>
        public UIWindow windowToUse
        {
            get
            {
                if (windowContainer != null)
                    return windowContainer.window;

                return window.window;
            }
        }

        [Header("Animations & Audio")]
        public MotionInfo useAnimation;
        public MotionInfo unUseAnimation;

        public AudioClipInfo useAudioClip;
        public AudioClipInfo unUseAudioClip;

        protected Animator animator;
        protected ITriggerWindowContainer windowContainer;

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponent<Animator>();
            windowContainer = GetComponent<ITriggerWindowContainer>();
        }

        protected virtual void WindowOnHide()
        {
            UnUse();
        }

        protected virtual void WindowOnShow()
        {

        }

        public override void DoVisuals()
        {
            if (useAnimation.motion != null && animator != null)
            {
                animator.Play(useAnimation);
            }

            AudioManager.AudioPlayOneShot(useAudioClip);
        }

        public override void UndoVisuals()
        {
            if (unUseAnimation.motion != null && animator != null)
            {
                animator.Play(unUseAnimation);
            }

            AudioManager.AudioPlayOneShot(unUseAudioClip);
        }

        public override bool Use(Player player)
        {
            if (CanUse(player) == false)
            {
                return false;
            }

            if (isInUse)
            {
                return true;
            }

            if (TriggerManager.currentActiveTrigger != null)
            {
                TriggerManager.currentActiveTrigger.UnUse(player);
            }

            if (windowToUse != null)
            {
                windowToUse.OnShow += WindowOnShow;
                windowToUse.OnHide += WindowOnHide;

                if (handleWindowDirectly)
                {
                    windowToUse.Show();
                }
            }

            DoVisuals();

            TriggerManager.currentActiveTrigger = this;
            NotifyTriggerUsed(player);

            return true;
        }
        
        public override bool UnUse(Player player)
        {
            if (CanUnUse(player) == false)
            {
                return false;
            }

            if (windowToUse != null)
            {
                windowToUse.OnShow -= WindowOnShow;
                windowToUse.OnHide -= WindowOnHide;

                if (handleWindowDirectly)
                {
                    windowToUse.Hide();
                }
            }

            UndoVisuals();

            TriggerManager.currentActiveTrigger = null;
            NotifyTriggerUnUsed(player);

            return true;
        }
    }
}