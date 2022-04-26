using System.Collections;
using UnityEngine;

namespace PointData
{
    public class Point : MonoBehaviour
    {
        [SerializeField] private float minScaleSize = 0.15f;
        [SerializeField] private float maxScaleSize = 0.6f;
        [SerializeField] private float changeScaleDuration = 0.5f;
        [SerializeField] private float changeSPScaleDuration = 0.5f;
        [SerializeField] private bool isEmpty, isControl, isCombine, isBomb;

        private Icon icon;
        private Label label;
        [SerializeField] private Index i;
        [SerializeField] private int tier;
        private ColorType pColor;

        private Collider2D _collider;
        private bool scaling = false;
        private float duration, scaleTime;
        private Vector3 startScale, targetScale;

        public bool onPointerEnter = false;
        public bool onPointerExit = false;

        void Awake()
        {
            icon = GetComponentInChildren<Icon>();
            label = GetComponentInChildren<Label>();
            _collider = GetComponent<Collider2D>();
        }

        void Update()
        {
            if (scaling)
            {
                if (scaleTime <= duration)
                {
                    scaleTime += Time.deltaTime;
                    float t = scaleTime / duration;

                    icon.Scale = Vector3.Lerp(startScale, targetScale, t);
                }
                else
                {
                    scaling = false;
                }
            }
        }

        public void ChangeScale(Vector3 startScale, Vector3 targetScale, float duration)
        {
            this.startScale = startScale;// icon.Scale;
            this.targetScale = targetScale;
            this.duration = duration;
            scaleTime = 0f;
            scaling = true;
        }

        public void OnPointerEnter()
        {
            if (!onPointerEnter)
            {
                gameController.CurrentIndex = i;

                if (isEmpty && gameController.AllowPointSetting && !isControl)
                {
                    ChangeScale(icon.Scale, Utils.ToVector3(maxScaleSize), changeScaleDuration);
                }

                onPointerEnter = true;
                onPointerExit = false;
            }
        }

        public void OnPointerExit()
        {
            if (!onPointerExit)
            {
                if (isEmpty && gameController.AllowPointSetting && !isControl)
                {
                    ChangeScale(icon.Scale, Utils.ToVector3(minScaleSize), changeScaleDuration);
                }

                onPointerExit = true;
                onPointerEnter = false;
            }
        }

        public void SetEmptyWithAnimation()
        {
            StartCoroutine(EmptyAnimation());
        }

        private IEnumerator EmptyAnimation()
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, pos.y, -5f);
            float halfDuration = changeSPScaleDuration * 0.5f;

            startScale = icon.Scale;
            targetScale = Utils.ToVector3(maxScaleSize) * 1.5f;
            duration = halfDuration;
            scaleTime = 0f;

            while (scaleTime <= duration)
            {
                scaleTime += Time.deltaTime;
                float t = scaleTime / duration;

                icon.Scale = Vector3.Lerp(startScale, targetScale, t);

                yield return null;
            }

            startScale = icon.Scale;
            targetScale = Utils.ToVector3(minScaleSize);
            duration = halfDuration;
            scaleTime = 0f;

            while (scaleTime <= duration)
            {
                scaleTime += Time.deltaTime;
                float t = scaleTime / duration;

                icon.Scale = Vector3.Lerp(startScale, targetScale, t);

                yield return null;
            }

            isEmpty = true;
            label.TextEnabled = false;
            label.ImageEnabled = false;
            IsBomb = false;
            tier = 0;
            color = ColorType.Empty;
            transform.position = pos;
        }

        public ColorType color
        {
            get
            {
                return pColor;
            }
            set
            {
                pColor = value;
                iconColor = Props.GetPointColor(pColor);
            }
        }

        public Color iconColor
        {
            get
            {
                return icon.spriteRenderer.color;
            }
            set
            {
                icon.spriteRenderer.color = value;
            }
        }

        public string text
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
            }
        }

        public int OrderInLayer
        {
            get
            {
                return label.OrderInLayer;
            }
            set
            {
                label.OrderInLayer = value;
            }
        }

        public int Tier
        {
            get
            {
                return tier;
            }
            set
            {
                tier = value;
                if (tier >= 0)
                {
                    label.Text = tier.ToString();
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                return isEmpty;
            }
            set
            {
                isEmpty = value;


                if (isEmpty)
                {
                    label.TextEnabled = false;
                    label.ImageEnabled = false;
                    tier = 0;
                    color = ColorType.Empty;
                    ChangeScale(icon.Scale, Utils.ToVector3(minScaleSize), changeScaleDuration);
                }
                else
                {
                    if (isBomb)
                    {
                        label.ImageEnabled = true;
                    }
                    else
                    {
                        label.TextEnabled = true;
                    }
                    ChangeScale(icon.Scale, Utils.ToVector3(maxScaleSize), changeScaleDuration);
                }
            }
        }

        public bool IsControl
        {
            get
            {
                return isControl;
            }
            set
            {
                isControl = value;

                if (isControl)
                {
                    ChangeScale(icon.Scale, Utils.ToVector3(maxScaleSize), changeScaleDuration * 0.5f);
                }
            }
        }

        public bool IsCombine
        {
            get
            {
                return isCombine;
            }
            set
            {
                isControl = value;

                if (isControl)
                {
                    icon.transform.localScale = new Vector3(maxScaleSize, maxScaleSize, maxScaleSize);
                }
            }
        }

        public bool IsBomb
        {
            get
            {
                return isBomb;
            }
            set
            {
                isBomb = value;

                if (isBomb)
                {
                    label.TextEnabled = false;
                    label.ImageEnabled = true;
                    tier = 0;
                    color = ColorType.Super;
                }
                else
                {
                    if (isEmpty) return;
                    label.TextEnabled = true;
                    label.ImageEnabled = false;
                }
            }
        }

        public Index index
        {
            get
            {
                return i;
            }
            set
            {
                i = value;
            }
        }

        public Bounds bounds
        {
            get
            {
                return _collider.bounds;
            }
        }

        private Controller _gameController;
        private Controller gameController
        {
            get
            {
                if (_gameController != null)
                {
                    return _gameController;
                }

                _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
                return _gameController;
            }
        }
    }
}
