using System;
using System.Collections;

namespace UnityEngine.UI
{
    [AddComponentMenu("Layout/Content Size Fitter With Max", 141)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class ContentSizeFitterWithMax : ContentSizeFitter
    {
        public enum SizeType
        {
            Pixel,
            ScreenPercentage,
            CanvasPercentage
        }

        public enum UpdateMode
        {
            OnlyWhenChanged,
            OnResolutionChange,
            Continuous
        }

        [NonSerialized] private RectTransform m_Rect;
        private Canvas m_Canvas;
        private Vector2 m_LastScreenSize;
        private Vector2 m_LastCanvasSize;
        private Coroutine m_CanvasCheckCoroutine;
        private bool m_ResolutionChangeEventRegistered;

        [SerializeField] private UpdateMode m_UpdateMode = UpdateMode.OnResolutionChange;

        public UpdateMode updateMode
        {
            get => m_UpdateMode;
            set
            {
                if (m_UpdateMode != value)
                {
                    m_UpdateMode = value;
                    UpdateMonitoringStrategy();
                }
            }
        }

        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                {
                    m_Rect = GetComponent<RectTransform>();
                }

                return m_Rect;
            }
        }

        private Canvas parentCanvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = GetComponentInParent<Canvas>();
                    if (m_Canvas == null)
                    {
                        m_Canvas = FindFirstObjectByType<Canvas>();
                    }
                }

                return m_Canvas;
            }
        }

        [SerializeField] private SizeType m_WidthType = SizeType.Pixel;

        public SizeType widthType
        {
            get => m_WidthType;
            set
            {
                if (m_WidthType != value)
                {
                    m_WidthType = value;
                    UpdateMonitoringStrategy();
                    SetDirty();
                }
            }
        }

        [SerializeField] private float m_MaxWidth = -1;

        public float maxWidth
        {
            get => m_MaxWidth;
            set
            {
                if (m_MaxWidth != value)
                {
                    m_MaxWidth = value;
                    SetDirty();
                }
            }
        }

        [SerializeField] private SizeType m_HeightType = SizeType.Pixel;

        public SizeType heightType
        {
            get => m_HeightType;
            set
            {
                if (m_HeightType != value)
                {
                    m_HeightType = value;
                    UpdateMonitoringStrategy();
                    SetDirty();
                }
            }
        }

        [SerializeField] private float m_MaxHeight = -1;

        public float maxHeight
        {
            get => m_MaxHeight;
            set
            {
                if (m_MaxHeight != value)
                {
                    m_MaxHeight = value;
                    SetDirty();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_LastScreenSize = new Vector2(Screen.width, Screen.height);

            // 캔버스 사이즈 초기화
            if (parentCanvas != null)
            {
                Rect canvasRect = parentCanvas.GetComponent<RectTransform>().rect;
                m_LastCanvasSize = new Vector2(canvasRect.width, canvasRect.height);
            }

            // 모니터링 전략 설정
            UpdateMonitoringStrategy();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopCanvasCheck();
            UnregisterResolutionChangeEvent();
        }

        private void UpdateMonitoringStrategy()
        {
            // 에디터 모드에서는 항상 연속 업데이트
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                StartContinuousUpdates();
                return;
            }
#endif

            // 런타임에서는 설정에 따라 다르게 동작
            StopCanvasCheck();
            UnregisterResolutionChangeEvent();

            // UpdateMode에 따른 처리
            if (m_UpdateMode == UpdateMode.Continuous)
            {
                StartContinuousUpdates();
            }
            else if (m_UpdateMode == UpdateMode.OnResolutionChange)
            {
                RegisterResolutionChangeEvent();

                // 처음 한번은 초기화
                SetDirty();
            }
            // OnlyWhenChanged는 별도 모니터링이 필요 없음
        }

        private void StartContinuousUpdates()
        {
            // CanvasPercentage 타입인 경우에만 캔버스 지속적으로 체크
            if (m_WidthType == SizeType.CanvasPercentage || m_HeightType == SizeType.CanvasPercentage)
            {
                if (m_CanvasCheckCoroutine == null)
                {
                    m_CanvasCheckCoroutine = StartCoroutine(CheckCanvasSize());
                }
            }
        }

        private void RegisterResolutionChangeEvent()
        {
            if (!m_ResolutionChangeEventRegistered)
            {
                // Unity 2022.2 미만에서는 Update 메서드에서 해상도 변경을 감지합니다
                m_ResolutionChangeEventRegistered = true;
            }
        }

        private void UnregisterResolutionChangeEvent()
        {
            if (m_ResolutionChangeEventRegistered)
            {
                m_ResolutionChangeEventRegistered = false;
            }
        }

#if UNITY_2022_2_OR_NEWER
        private void OnResolutionChanged(int width, int height)
        {
            m_LastScreenSize = new Vector2(width, height);
            SetDirty();
        }
#endif

        void Update()
        {
            // Unity 2022.2 미만에서 처리하는 해상도 변경 감지 로직
            if (m_ResolutionChangeEventRegistered && m_UpdateMode == UpdateMode.OnResolutionChange)
            {
                Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
                if (m_LastScreenSize != currentScreenSize)
                {
                    m_LastScreenSize = currentScreenSize;
                    SetDirty();
                }
            }
        }

        private void StopCanvasCheck()
        {
            if (m_CanvasCheckCoroutine != null)
            {
                StopCoroutine(m_CanvasCheckCoroutine);
                m_CanvasCheckCoroutine = null;
            }
        }

        private IEnumerator CheckCanvasSize()
        {
            while (true)
            {
                if (parentCanvas != null)
                {
                    Rect canvasRect = parentCanvas.GetComponent<RectTransform>().rect;
                    Vector2 currentSize = new Vector2(canvasRect.width, canvasRect.height);

                    // 캔버스 크기가 변경되었으면 레이아웃 업데이트
                    if (m_LastCanvasSize != currentSize)
                    {
                        m_LastCanvasSize = currentSize;
                        SetDirty();
                    }
                }

                yield return null;
            }
        }

        private float GetActualMaxWidth()
        {
            if (m_MaxWidth <= 0)
                return -1;

            switch (m_WidthType)
            {
                case SizeType.Pixel:
                    return m_MaxWidth;
                case SizeType.ScreenPercentage:
                    return Screen.width * (m_MaxWidth / 100f);
                case SizeType.CanvasPercentage:
                    if (parentCanvas != null)
                    {
                        Rect canvasRect = parentCanvas.GetComponent<RectTransform>().rect;
                        return canvasRect.width * (m_MaxWidth / 100f);
                    }

                    Debug.LogWarning("Canvas not found for CanvasPercentage calculation");
                    return m_MaxWidth;
                default:
                    return m_MaxWidth;
            }
        }

        private float GetActualMaxHeight()
        {
            if (m_MaxHeight <= 0)
                return -1;

            switch (m_HeightType)
            {
                case SizeType.Pixel:
                    return m_MaxHeight;
                case SizeType.ScreenPercentage:
                    return Screen.height * (m_MaxHeight / 100f);
                case SizeType.CanvasPercentage:
                    if (parentCanvas != null)
                    {
                        Rect canvasRect = parentCanvas.GetComponent<RectTransform>().rect;
                        return canvasRect.height * (m_MaxHeight / 100f);
                    }

                    Debug.LogWarning("Canvas not found for CanvasPercentage calculation");
                    return m_MaxHeight;
                default:
                    return m_MaxHeight;
            }
        }

        public override void SetLayoutHorizontal()
        {
            base.SetLayoutHorizontal();

            float maxW = GetActualMaxWidth();
            if (maxW > 0)
            {
                if (horizontalFit == FitMode.MinSize)
                {
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                        Mathf.Min(LayoutUtility.GetMinSize(m_Rect, 0), maxW));
                }
                else if (horizontalFit == FitMode.PreferredSize)
                {
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                        Mathf.Min(LayoutUtility.GetPreferredSize(m_Rect, 0), maxW));
                }
            }
        }

        public override void SetLayoutVertical()
        {
            base.SetLayoutVertical();

            float maxH = GetActualMaxHeight();
            if (maxH > 0)
            {
                if (verticalFit == FitMode.MinSize)
                {
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                        Mathf.Min(LayoutUtility.GetMinSize(m_Rect, 1), maxH));
                }
                else if (verticalFit == FitMode.PreferredSize)
                {
                    rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                        Mathf.Min(LayoutUtility.GetPreferredSize(m_Rect, 1), maxH));
                }
            }
        }
    }
}