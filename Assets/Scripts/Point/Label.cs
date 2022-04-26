using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PointData
{
    public class Label : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        private Canvas canvas;


        private void Awake()
        {
            canvas = transform.parent.GetComponent<Canvas>();
        }

        public bool TextEnabled
        {
            get
            {
                return text.gameObject.activeSelf;
            }
            set
            {
                text.gameObject.SetActive(value);
            }
        }

        public bool ImageEnabled
        {
            get
            {
                return image.gameObject.activeSelf;
            }
            set
            {
                image.gameObject.SetActive(value);
            }
        }

        public string Text
        {
            get
            {
                return text.text;
            }
            set
            {
                text.text = value;
            }
        }

        public int OrderInLayer
        {
            get
            {
                return canvas.sortingOrder;
            }
            set
            {
                canvas.sortingOrder = value;
            }
        }
    }
}