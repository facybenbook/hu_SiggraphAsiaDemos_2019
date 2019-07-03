using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ToucanSystems
{
    public enum InteractionType
    {
        onHover,
        onClick
    }

    /// <summary>
    /// Class representing single marker for the chart.
    /// </summary>
    public class ChartMarkerController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField]
        private float pulseDuration = 0.25f;
        [HideInInspector]
        public GameObject markerLabel;
        [HideInInspector]
        public GameObject background;
        [HideInInspector]
        public InteractionType interactionType;

        private RectTransform markerRT;
        private CanvasGroup backgroundCG;

        private void Start()
        {
            markerRT = GetComponent<RectTransform>();
            backgroundCG = background.GetComponent<CanvasGroup>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (interactionType == InteractionType.onHover)
            {
                StartCoroutine(ClickAnimationCoroutine(true));
                background.transform.parent.transform.SetAsLastSibling();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (interactionType == InteractionType.onHover)
            {
                StartCoroutine(ClickAnimationCoroutine(false));
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (interactionType == InteractionType.onClick)
            {
                if (!background.activeInHierarchy)
                {
                    DisableOthers();
                    StartCoroutine(ClickAnimationCoroutine(true));
                    background.transform.parent.transform.SetAsLastSibling();
                }
                else
                {
                    StartCoroutine(ClickAnimationCoroutine(false));
                }
            }
        }

        private IEnumerator ClickAnimationCoroutine(bool open)
        {
            float duration = pulseDuration / 2;
            float fromAlpha = 0;
            float toAlpha = 1;

            if (open)
            {
                background.SetActive(true);
            }

            if (!open)
            {
                fromAlpha = 1;
                toAlpha = 0;
            }

            for (float i = 0; i < duration; i += Time.deltaTime)
            {
                markerRT.localScale = new Vector3(1, 1, 1) * Mathf.SmoothStep(1, 1.3f, i / duration);
                backgroundCG.alpha = Mathf.SmoothStep(fromAlpha, toAlpha, i / duration);
                yield return null;
            }

            backgroundCG.alpha = toAlpha;

            for (float i = 0; i < duration; i += Time.deltaTime)
            {
                markerRT.localScale = new Vector3(1, 1, 1) * Mathf.SmoothStep(1.3f, 1, i / duration);
                yield return null;
            }

            markerRT.localScale = new Vector3(1, 1, 1);

            if (!open)
            {
                background.SetActive(false);
            }
        }

        private void DisableOthers()
        {
            ChartMarkerController[] markersControllers = transform.parent.GetComponentsInChildren<ChartMarkerController>();
            foreach (ChartMarkerController marker in markersControllers)
            {
                marker.background.SetActive(false);
            }
        }
    }
}
