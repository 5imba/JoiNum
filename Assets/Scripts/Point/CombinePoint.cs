using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PointData
{
    public class CombinePoint : MonoBehaviour
    {
        private bool moving = false;
        private float duration, time;
        private Vector3 startPos;
        private Vector3[] targetPos;
        private Point targetPoint;
        private ColorType color;
        private int tier;
        private bool isSuper;

        private int index = 0;
        private int counter = 1;
        private float q;

        void Update()
        {
            if (moving)
            {
                if (time <= duration)
                {
                    time += Time.deltaTime;
                    float t = time / duration;

                    if (targetPos.Length > 1)
                    {
                        if (t >= q * counter)
                        {
                            startPos = targetPos[index];
                            counter += 1;
                            index -= 1;
                        }
                        else
                        {
                            float qt = Utils.Remap(t % q, 0f, q, 0f, 1f);
                            transform.position = Vector3.Lerp(startPos, targetPos[index], qt);
                        }
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(startPos, targetPos[0], t);
                    }
                }
                else
                {
                    moving = false;

                    if (tier > 0 || isSuper)
                    {
                        targetPoint.Tier = tier;
                        targetPoint.color = color;
                        targetPoint.IsEmpty = false;
                        targetPoint.IsBomb = isSuper;
                        gameController.SaveGameState();
                    }

                    Destroy(gameObject);
                }
            }
        }
        public void MoveTo(Point[] targetPoint, float duration, ColorType color, bool isSuper, int tier = -1)
        {
            index = targetPoint.Length - 1;
            q = 1f / targetPoint.Length;

            startPos = transform.position;

            targetPos = new Vector3[targetPoint.Length];
            for (int i = 0; i < targetPoint.Length; i++)
            {
                Vector3 pos = targetPoint[i].transform.position;
                pos.z = transform.position.z;
                targetPos[i] = pos;
            }

            this.duration = duration;
            time = 0f;
            moving = true;

            this.targetPoint = targetPoint[0];
            this.color = color;
            this.tier = tier;
            this.isSuper = isSuper;
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
