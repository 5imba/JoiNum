using UnityEngine;

namespace PointData
{
    public class ControlPointHolder : MonoBehaviour
    {
        [SerializeField] private Transform controlPointHolderUI;
        [SerializeField] private int index;
        [SerializeField] private float duration = 0.2f;

        private Point _point;
        private bool followPointer = false;
        private bool controlPointIsMoving = false;
        private Collider2D _collider;

        private float time;
        private Vector3 originPos, startPos, targetPos;

        bool init = false;
        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            Vector3 pos = controlPointHolderUI.position;
            pos.z = 0f;
            transform.position = pos;
            _collider = GetComponent<Collider2D>();
            originPos = transform.position;

            init = true;
        }

        private void Update()
        {
            if (controlPointIsMoving)
            {
                if (time <= duration)
                {
                    time += Time.deltaTime;
                    float t = time / duration;

                    _point.transform.position = Vector3.Lerp(startPos, targetPos, t);
                }
                else
                {
                    controlPointIsMoving = false;

                    if (!gameController.CurrentIndex.IsNegative())
                    {
                        gameController.SetPoint(index);
                    }
                }
            }
        }

        public void OnPointerUp()
        {
            // Moving control point
            controlPointIsMoving = true;
            startPos = _point.transform.position;
            targetPos = gameController.IsAnableToSet ? gameController.CurrentPointPos : originPos;

            time = 0f;
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public Point point
        {
            get
            {
                return _point;
            }
            set
            {
                if (_point != null)
                {
                    Destroy(_point.gameObject);
                }
                _point = value;
                _point.transform.position = transform.position;
                originPos = transform.position;
            }
        }

        public bool FollowPointer
        {
            set
            {
                followPointer = value;
            }
        }

        public Vector3 PointPos
        {
            get
            {
                return _point.transform.position;
            }
            set
            {
                if (followPointer)
                {
                    _point.transform.position = value;
                }
            }
        }

        public Bounds bounds
        {
            get
            {
                if (!init) Init();
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