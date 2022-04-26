using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using PointData;

public class TutorialController : Controller
{
    [SerializeField] private GameObject[] tutorialPages;

    [Header("")]
    [SerializeField] private bool handControl = false;
    [SerializeField] private GameObject handPointer;
    [SerializeField] private Tutorial.HintsButton hintsButton;
    [SerializeField] private Button undo;
    [SerializeField] private Button refresh;

    //[SerializeField] private AudioController audioCtrl;
    [SerializeField] private Transform pointPrefab;
    [SerializeField] private Transform controlPointPrefab;
    [SerializeField] private Transform combinePointPrefab;
    [SerializeField] private Transform pointsHolder;
    [SerializeField] private ControlPointHolder[] controlPointHolders;

    [Header("UI")]
    [SerializeField] private Button undoButton;
    [SerializeField] private ScoreLabel score;

    [Header("Grid Settings")]
    [SerializeField] private int gridSize = 9;
    [SerializeField] private float pointScale = 0.5f;

    [Header("Point Settings")]
    //[SerializeField] private bool random = false;
    //[SerializeField] private int[] numColors;
    //[SerializeField] private int[] numTiers;
    [SerializeField] private int achiveLimitTier = 8;
    //[SerializeField] private int minControlPointTier = 1;
    [SerializeField] private int maxStartControlPointTier = 3;

    [Header("Animations")]
    [SerializeField] private float combineDuration = 0.3f;
    [SerializeField] private float multipleCombinesDelay = 0.25f;
    [SerializeField] private float scoreChangingDuration = 0.35f;
    [SerializeField] private float handMovingSpeed = 1f;

    [Header("Configs")]
    [SerializeField] private int historyCapacity = 10;

    private Point[,] pointGrid;
    private Index i = Index.negative;
    private Index bombPointIndex = Index.negative;
    private bool hasSuperPoint = false;

    private bool allowInput = true;
    private bool allowPointSetting = false;
    private ControlPointHolder currentCPHolder;

    private int scoreNum = 0;
    private int[] maxTier;
    private int reachedTier = 0;

    private List<History> history = new List<History>();
    private List<Point> samePoints = new List<Point>();
    private List<Point> combinePoints = new List<Point>();
    private List<List<Point>> combinePointsGrouped = new List<List<Point>>();
    private Point currentPoint;

    Coroutine tutorial;
    private int tutorialIndex = 0;
    private List<List<TutorialTask>> tutorialTasks;
    private int[] pointsOrderIndexes;

    private int task = 0;
    private int subTaskIndex = 0;
    private int pointTaskIndex = 0;

    bool isMoving = false;
    int control = 2;
    Index target;
    Vector3 startPos, targetPos;
    float time = 0f;
    bool taskDone = false;

    public void NextTutorial()
    {
        //Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);

        tutorialIndex += 1;
        if (tutorialIndex >= tutorialPages.Length) tutorialIndex = tutorialPages.Length - 1;

        for (int i = 0; i < tutorialPages.Length; i++)
        {
            tutorialPages[i].SetActive(i == tutorialIndex);
        }

        if (tutorialIndex != 1)
        {
            task += 1;
            subTaskIndex = 0;
            pointTaskIndex = 0;

            StopCoroutine(tutorial);
            isMoving = false;

            switch (task)
            {
                case 0:
                    tutorial = StartCoroutine(MovePoints(0.3f));
                    break;
                case 1:
                    tutorial = StartCoroutine(OpenHints());
                    break;
                case 2:
                    tutorial = StartCoroutine(UndoClick());
                    break;
                case 3:
                    tutorial = StartCoroutine(RefreshClick());
                    break;
                case 4:
                    tutorial = StartCoroutine(MovePoints(0.3f, true));
                    break;
                case 5:
                    tutorial = StartCoroutine(MovePoints(0.3f));
                    break;
            }
        }
    }

    public void PreviousTutorial()
    {
        //Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);

        tutorialIndex -= 1;
        if (tutorialIndex < 0) tutorialIndex = 0;

        for (int i = 0; i < tutorialPages.Length; i++)
        {
            tutorialPages[i].SetActive(i == tutorialIndex);
        }

        if (tutorialIndex != 0)
        {
            task -= 1;
            subTaskIndex = 0;
            pointTaskIndex = 0;

            StopCoroutine(tutorial);
            isMoving = false;

            switch (task)
            {
                case 5:
                    tutorial = StartCoroutine(MovePoints(0.3f));
                    break;
                case 4:
                    tutorial = StartCoroutine(MovePoints(0.3f, true));
                    break;
                case 3:
                    tutorial = StartCoroutine(RefreshClick());
                    break;
                case 2:
                    tutorial = StartCoroutine(UndoClick());
                    break;
                case 1:
                    tutorial = StartCoroutine(OpenHints());
                    break;
                case 0:
                    tutorial = StartCoroutine(MovePoints(0.3f));
                    break;
            }
        }
    }

    public void FinishTutorial()
    {
        //Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
        PlayerPrefs.SetInt("firstPlay", 1);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    void Start()
    {
        Init();

        // Set values
        maxTier = Enumerable.Repeat(maxStartControlPointTier, Props.PointColorsLength).ToArray();

        // Create points
        CreateGrid();
        InitControlPoints();

        reachedTier = maxStartControlPointTier;
        tutorial = StartCoroutine(MovePoints(0.5f));
    }

    private void Update()
    {
        // Input Management
        if (!handControl)
        {
            if (isMoving)
            {
                if (currentCPHolder.PointPos != targetPos)
                {
                    time += Time.deltaTime * handMovingSpeed;

                    Vector3 newPos = Vector3.MoveTowards(currentCPHolder.PointPos, targetPos, Time.deltaTime * handMovingSpeed);
                    currentCPHolder.PointPos = newPos;
                    handPointer.transform.position = newPos;

                    bool hoverAnyPoint = false;
                    for (int y = 0; y < pointGrid.GetLength(0); y++)
                    {
                        for (int x = 0; x < pointGrid.GetLength(1); x++)
                        {
                            Point p = GetPoint(y, x);
                            Vector2 pos = newPos;
                            if (p.bounds.Contains(pos))
                            {
                                p.OnPointerEnter();
                                hoverAnyPoint = true;
                            }
                            else
                            {
                                p.OnPointerExit();
                            }
                        }
                    }

                    // Reset Current Index
                    if (!hoverAnyPoint) i = Index.negative;
                }
                else
                {
                    handPointer.SetActive(false);

                    if (!i.IsNegative()) CurrentPoint.OnPointerExit();

                    allowPointSetting = false;
                    currentCPHolder.OnPointerUp();

                    isMoving = false;
                    taskDone = true;
                }
            }
        }
        else
        {
            Vector2 resultPos;
            if (Pointer.position(out resultPos))
            {
                Vector3 pointerPos = Camera.main.ScreenToWorldPoint(resultPos);
                pointerPos.z = 0f;

                // Check point entering
                if (allowPointSetting)
                {
                    bool hoverAnyPoint = false;
                    for (int y = 0; y < pointGrid.GetLength(0); y++)
                    {
                        for (int x = 0; x < pointGrid.GetLength(1); x++)
                        {
                            Point p = GetPoint(y, x);
                            if (p.bounds.Contains(pointerPos))
                            {
                                p.OnPointerEnter();
                                hoverAnyPoint = true;
                            }
                            else
                            {
                                p.OnPointerExit();
                            }
                        }
                    }

                    // Reset Current Index
                    if (!hoverAnyPoint) i = Index.negative;
                }

                // Check pointer entering to control point 
                else if (allowInput)
                {
                    for (int i = 0; i < controlPointHolders.Length; i++)
                    {
                        ControlPointHolder cph = controlPointHolders[i];
                        if (cph.bounds.Contains(pointerPos))
                        {
                            if (i == 0 ? GameTempData.Hints > 0 : true)
                            {
                                allowPointSetting = true;
                                allowInput = false;
                                currentCPHolder = cph;
                                currentCPHolder.FollowPointer = true;
                            }
                        }
                    }
                }

                // control point following pointer
                if (currentCPHolder != null)
                {
                    currentCPHolder.PointPos = new Vector3(pointerPos.x, pointerPos.y, -1f);
                }
            }
            else
            {
                // On pointer up
                if (currentCPHolder != null)
                {
                    if (!i.IsNegative()) CurrentPoint.OnPointerExit();

                    allowPointSetting = false;
                    currentCPHolder.OnPointerUp();
                    currentCPHolder = null;
                }
            }
        }
    }

    private void Init()
    {
        tutorialTasks = new List<List<TutorialTask>>()
        {
            // Combine points
            new List<TutorialTask>()
            {
                new TutorialTask(
                    new List<PointObj>[4]
                    {
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Super, Index.negative, false, true, false, true)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false)
                        }
                    },
                    new List<TutorialPoint>()
                    {
                        new TutorialPoint(2, new Index(6, 4)),
                        new TutorialPoint(2, new Index(5, 4)),
                        new TutorialPoint(2, new Index(4, 4))
                    },
                    new PointObj[]
                    {
                        new PointObj(1, ColorType.Yellow, new Index(4, 5), false, false, false, false),
                        new PointObj(1, ColorType.Yellow, new Index(4, 6), false, false, false, false),
                        new PointObj(2, ColorType.Orange, new Index(2, 4), false, false, false, false),
                        new PointObj(2, ColorType.Orange, new Index(3, 4), false, false, false, false)
                    }
                ),
                new TutorialTask(
                    new List<PointObj>[4]
                    {
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Super, Index.negative, false, true, false, true)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false)
                        }
                    },
                    new List<TutorialPoint>()
                    {
                        new TutorialPoint(2, new Index(6, 4)),
                        new TutorialPoint(2, new Index(5, 4)),
                        new TutorialPoint(2, new Index(5, 5))
                    },
                    new PointObj[]
                    {
                        new PointObj(1, ColorType.Yellow, new Index(4, 5), false, false, false, false),
                        new PointObj(1, ColorType.Yellow, new Index(4, 6), false, false, false, false),
                        new PointObj(2, ColorType.Orange, new Index(2, 4), false, false, false, false),
                        new PointObj(2, ColorType.Orange, new Index(3, 4), false, false, false, false)
                    }
                ),
                new TutorialTask(
                    new List<PointObj>[4]
                    {
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Super, Index.negative, false, true, false, true)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false)
                        }
                    },
                    new List<TutorialPoint>()
                    {
                        new TutorialPoint(2, new Index(6, 4)),
                        new TutorialPoint(2, new Index(5, 4)),
                        new TutorialPoint(2, new Index(4, 4))
                    },
                    new PointObj[]
                    {
                        new PointObj(2, ColorType.Green, new Index(4, 2), false, false, false, false),
                        new PointObj(2, ColorType.Green, new Index(4, 3), false, false, false, false),
                        new PointObj(3, ColorType.Green, new Index(2, 4), false, false, false, false),
                        new PointObj(3, ColorType.Green, new Index(3, 4), false, false, false, false),
                        new PointObj(4, ColorType.Green, new Index(4, 5), false, false, false, false),
                        new PointObj(4, ColorType.Green, new Index(4, 6), false, false, false, false)
                    }
                )
            },

            // Open hints
            new List<TutorialTask>()
            {
                new TutorialTask(
                    new List<PointObj>[4]
                    {
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Super, Index.negative, false, true, false, true)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                        },
                        new List<PointObj>()
                        {
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false)
                        }
                    },
                    new List<TutorialPoint>()
                    {
                        new TutorialPoint(0, new Index(4, 4))
                    },
                    new PointObj[]
                    {
                        new PointObj(1, ColorType.Green, new Index(5, 4), false, false, false, false),
                        new PointObj(1, ColorType.Green, new Index(6, 4), false, false, false, false),
                        new PointObj(4, ColorType.Red, new Index(4, 5), false, false, false, false),
                        new PointObj(4, ColorType.Red, new Index(4, 6), false, false, false, false),
                        new PointObj(2, ColorType.Yellow, new Index(4, 2), false, false, false, false),
                        new PointObj(2, ColorType.Yellow, new Index(4, 3), false, false, false, false),
                        new PointObj(3, ColorType.Orange, new Index(2, 4), false, false, false, false),
                        new PointObj(3, ColorType.Orange, new Index(3, 4), false, false, false, false),
                    }
                )
            },

            // Undo hint
            new List<TutorialTask>()
            {
                new TutorialTask(
                    new List<PointObj>[4]
                    {
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Super, Index.negative, false, true, false, true)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                        },
                        new List<PointObj>()
                        {
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false)
                        }
                    },
                    new List<TutorialPoint>()
                    {
                        new TutorialPoint(0, new Index(4, 4))
                    },
                    new PointObj[]
                    {
                        new PointObj(2, ColorType.Green, new Index(4, 4), false, false, false, false),
                        new PointObj(2, ColorType.Yellow, new Index(4, 2), false, false, false, false),
                        new PointObj(2, ColorType.Yellow, new Index(4, 3), false, false, false, false),
                        new PointObj(3, ColorType.Orange, new Index(2, 4), false, false, false, false),
                        new PointObj(3, ColorType.Orange, new Index(3, 4), false, false, false, false),
                        new PointObj(4, ColorType.Red, new Index(4, 5), false, false, false, false),
                        new PointObj(4, ColorType.Red, new Index(4, 6), false, false, false, false)
                    }
                )
            },

            // Refresh hint
            new List<TutorialTask>()
            {
                new TutorialTask(
                    new List<PointObj>[4]
                    {
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Super, Index.negative, false, true, false, true)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                        },
                        new List<PointObj>()
                        {
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false)
                        }
                    },
                    new List<TutorialPoint>()
                    {
                        new TutorialPoint(0, new Index(4, 4))
                    },
                    new PointObj[]
                    {
                        new PointObj(1, ColorType.Green, new Index(5, 4), false, false, false, false),
                        new PointObj(1, ColorType.Green, new Index(6, 4), false, false, false, false),
                        new PointObj(2, ColorType.Yellow, new Index(4, 2), false, false, false, false),
                        new PointObj(2, ColorType.Yellow, new Index(4, 3), false, false, false, false),
                        new PointObj(3, ColorType.Orange, new Index(2, 4), false, false, false, false),
                        new PointObj(3, ColorType.Orange, new Index(3, 4), false, false, false, false),
                        new PointObj(4, ColorType.Red, new Index(4, 5), false, false, false, false),
                        new PointObj(4, ColorType.Red, new Index(4, 6), false, false, false, false)
                    }
                )
            },

            // Bomb hint
            new List<TutorialTask>()
            {
                new TutorialTask(
                    new List<PointObj>[4]
                    {
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Super, Index.negative, false, true, false, true),
                            new PointObj(1, ColorType.Super, Index.negative, false, true, false, true)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                        },
                        new List<PointObj>()
                        {
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false)
                        }
                    },
                    new List<TutorialPoint>()
                    {
                        new TutorialPoint(0, new Index(4, 4))
                    },
                    new PointObj[]
                    {
                        new PointObj(1, ColorType.Green, new Index(5, 4), false, false, false, false),
                        new PointObj(1, ColorType.Green, new Index(6, 4), false, false, false, false),
                        new PointObj(2, ColorType.Yellow, new Index(4, 2), false, false, false, false),
                        new PointObj(2, ColorType.Yellow, new Index(4, 3), false, false, false, false),
                        new PointObj(3, ColorType.Orange, new Index(2, 4), false, false, false, false),
                        new PointObj(3, ColorType.Orange, new Index(3, 4), false, false, false, false),
                        new PointObj(4, ColorType.Red, new Index(4, 5), false, false, false, false),
                        new PointObj(4, ColorType.Red, new Index(4, 6), false, false, false, false)
                    }
                )
            },

            // Gameplay
            new List<TutorialTask>()
            {
                new TutorialTask(
                    new List<PointObj>[4]
                    {
                        new List<PointObj>()
                        {
                            new PointObj(1, ColorType.Super, Index.negative, false, true, false, true)
                        },
                        new List<PointObj>()
                        {
                            new PointObj(3, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Orange, Index.negative, false, true, false, false),
                        },
                        new List<PointObj>()
                        {
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Yellow, Index.negative, false, true, false, false),
                        },
                        new List<PointObj>()
                        {
                            new PointObj(3, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(1, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Orange, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Green, Index.negative, false, true, false, false),
                            new PointObj(3, ColorType.Yellow, Index.negative, false, true, false, false),
                            new PointObj(2, ColorType.Orange, Index.negative, false, true, false, false),
                        }
                    },
                    new List<TutorialPoint>()
                    {
                        new TutorialPoint(1, new Index(8, 0)),//3g
                        new TutorialPoint(3, new Index(7, 0)),//3g
                        new TutorialPoint(1, new Index(8, 8)),//2y
                        new TutorialPoint(2, new Index(7, 8)),//2y
                        new TutorialPoint(2, new Index(5, 3)),//1o
                        new TutorialPoint(1, new Index(4, 3)),//1o
                        new TutorialPoint(2, new Index(8, 1)),//3g
                        new TutorialPoint(1, new Index(7, 6)),//3y
                        new TutorialPoint(2, new Index(8, 6)),//3y                        
                        new TutorialPoint(2, new Index(8, 7)),//2y
                        new TutorialPoint(3, new Index(5, 5)),//3o
                        new TutorialPoint(3, new Index(8, 0)),//3g
                        new TutorialPoint(3, new Index(4, 4)),//1o
                        new TutorialPoint(2, new Index(7, 8)),//2y
                        new TutorialPoint(3, new Index(8, 8)),//2y
                        new TutorialPoint(3, new Index(7, 0)),//3g
                        new TutorialPoint(1, new Index(6, 0)),//2g
                        new TutorialPoint(2, new Index(6, 1)),//2g
                        new TutorialPoint(1, new Index(7, 1)),//2g
                        new TutorialPoint(1, new Index(6, 8)),//2y
                        new TutorialPoint(1, new Index(6, 0)),//3g
                        new TutorialPoint(1, new Index(5, 0)),//2g
                        new TutorialPoint(1, new Index(4, 3)),//2o
                        new TutorialPoint(3, new Index(5, 4)),//2o
                        new TutorialPoint(3, new Index(4, 4)),//3o                        
                        new TutorialPoint(3, new Index(3, 5)),//3o
                        new TutorialPoint(3, new Index(3, 6)),//3o
                        new TutorialPoint(3, new Index(6, 2)),//3g
                        new TutorialPoint(2, new Index(8, 6)),//1y
                        new TutorialPoint(2, new Index(6, 7)),//3y
                        new TutorialPoint(2, new Index(5, 5)),//1o
                        new TutorialPoint(1, new Index(5, 6)),//1o
                        new TutorialPoint(2, new Index(7, 7)),//3y                        
                        new TutorialPoint(2, new Index(5, 1)),//2g
                        new TutorialPoint(2, new Index(4, 6)),//1o
                        new TutorialPoint(2, new Index(5, 6)),//2o
                        new TutorialPoint(2, new Index(6, 1)),//2g
                        new TutorialPoint(2, new Index(4, 5)),//2o
                        new TutorialPoint(2, new Index(6, 4)),//3o
                        new TutorialPoint(1, new Index(8, 5)),//1y
                        new TutorialPoint(1, new Index(7, 5)),//1y
                        new TutorialPoint(1, new Index(5, 6)),//2o
                        new TutorialPoint(1, new Index(6, 6)),//2o
                        new TutorialPoint(2, new Index(6, 5)),//2o
                        new TutorialPoint(1, new Index(8, 5)),//2y
                        new TutorialPoint(1, new Index(8, 6)),//2y
                        
                        new TutorialPoint(1, new Index(8, 0)),//2g
                        new TutorialPoint(1, new Index(8, 5)),//2y
                        new TutorialPoint(1, new Index(8, 2)),//1g
                        new TutorialPoint(3, new Index(8, 3)),//1g
                        new TutorialPoint(3, new Index(7, 5)),//2y
                        new TutorialPoint(3, new Index(8, 1)),//1g
                        new TutorialPoint(3, new Index(5, 4)),//3o
                        new TutorialPoint(3, new Index(3, 6)),//3o
                        new TutorialPoint(3, new Index(6, 6)),//3y
                        new TutorialPoint(3, new Index(8, 3)),//3g
                        new TutorialPoint(3, new Index(8, 2)),//2g
                        new TutorialPoint(3, new Index(7, 6)),//3y
                    },
                    new PointObj[]
                    {

                    }
                )
            },
        };

        pointsOrderIndexes = new int[4] { 0, 0, 0, 0 };
    }

    private PointObj GetPointsOrderIndex(int cpIndex)
    {
        int result;
        if (cpIndex == 0)
        {
            result = 0;
        }
        else
        {
            result = pointsOrderIndexes[cpIndex];
            pointsOrderIndexes[cpIndex] += 1;
        }

        //Debug.Log($"t:{task} st:{subTaskIndex} i:{cpIndex} r:{result}");

        return tutorialTasks[task][subTaskIndex].controlPointsPool[cpIndex][result];
    }

    private IEnumerator MovePoints(float delay = 0f, bool hintsMenu = false)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        else
            yield return new WaitForEndOfFrame();

        ClearGrid();
        InitControlPoints();

        if (hintsMenu)
            hintsButton.HardOpenHintsMenu();
        else
            hintsButton.HardCloseHintsMenu();

        while (true)
        {
            if (pointTaskIndex >= tutorialTasks[task][subTaskIndex].pointsOrder.Count)
            {
                yield return new WaitForSeconds(1.5f);

                subTaskIndex += 1;
                if (subTaskIndex >= tutorialTasks[task].Count)
                {
                    subTaskIndex = 0;
                }

                pointTaskIndex = 0;
                pointsOrderIndexes = Enumerable.Repeat(0, 4).ToArray();

                

                ClearGrid();
                InitControlPoints();
                GameTempData.Hints = 10;
            }
            if (hintsMenu)
            {
                if (!hintsButton.isOpened)
                {
                    hintsButton.OpenHintsMenu();
                }
            }
            else
            {
                if (hintsButton.isOpened)
                {
                    hintsButton.CloseHintsMenu();
                }
            }

            var t = tutorialTasks[task][subTaskIndex].pointsOrder[pointTaskIndex];
            control = t.control;
            target = t.target;

            currentCPHolder = controlPointHolders[control];
            currentCPHolder.FollowPointer = true;
            allowPointSetting = true;

            startPos = currentCPHolder.point.transform.position;
            startPos.z = -1f;

            targetPos = GetPoint(target).transform.position;
            targetPos.z = -1f;

            handPointer.transform.position = startPos;
            handPointer.SetActive(true);

            time = 0;
            isMoving = true;
            taskDone = false;
            allowInput = false;
            pointTaskIndex += 1;

            while (!taskDone || !allowInput)
            {
                yield return null;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator OpenHints()
    {
        yield return new WaitForSeconds(0.3f);
        ClearGrid();
        hintsButton.HardCloseHintsMenu();

        while (true)
        {
            handPointer.transform.position = hintsButton.transform.position;
            handPointer.SetActive(true);

            StartCoroutine(ClickHand());
            yield return new WaitForSeconds(0.1f);

            hintsButton.OpenHintsMenu();
            yield return new WaitForSeconds(1.5f);

            handPointer.SetActive(false);

            hintsButton.CloseHintsMenu();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator UndoClick()
    {
        History lastHistory = new History(GetPoint(4, 4), new HistoryPoint(new Index(0, -2), 1, ColorType.Green, Vector3.zero, false), 2);
        lastHistory.AddCombinePoints(new List<HistoryPoint>()
        {
            new HistoryPoint(new Index(5, 4), 1, ColorType.Green, GetPoint(5, 4).transform.position, false),
            new HistoryPoint(new Index(6, 4), 1, ColorType.Green, GetPoint(6, 4).transform.position, false)
        });
        lastHistory.AddTargetPoints(new List<List<Point>>()
        {
            new List<Point>()
            {
                GetPoint(4, 4),
                GetPoint(5, 4)
            },
            new List<Point>()
            {
                GetPoint(4, 4),
                GetPoint(5, 4),
                GetPoint(6, 4)
            }
        });


        Image undoImage = undo.GetComponent<Image>();
        Color undoColor = undoImage.color;
        Color pressedColor = undoColor * new Color(0.7843137f, 0.7843137f, 0.7843137f, 1f);

        hintsButton.HardOpenHintsMenu();
        while (true)
        {
            handPointer.transform.position = undo.transform.position;
            handPointer.SetActive(true);

            ClearGrid();
            GameTempData.Hints = 10;
            yield return new WaitForSeconds(1f);

            undoImage.color = pressedColor;
            StartCoroutine(ClickHand());
            yield return new WaitForSeconds(0.1f);

            history = new List<History>() { lastHistory };
            undo.onClick.Invoke();
            undoImage.color = undoColor;

            yield return new WaitForSeconds(3f);
        }
    }

    private IEnumerator RefreshClick()
    {
        Image refreshImage = refresh.GetComponent<Image>();
        Color refreshColor = refreshImage.color;
        Color pressedColor = refreshColor * new Color(0.7843137f, 0.7843137f, 0.7843137f, 1f);

        hintsButton.HardOpenHintsMenu();
        while (true)
        {
            handPointer.transform.position = refresh.transform.position;
            handPointer.SetActive(true);

            ClearGrid();
            GameTempData.Hints = 10;
            yield return new WaitForSeconds(1f);

            refreshImage.color = pressedColor;
            StartCoroutine(ClickHand());
            yield return new WaitForSeconds(0.1f);

            refresh.onClick.Invoke();
            refreshImage.color = refreshColor;

            yield return new WaitForSeconds(1.5f);
        }
    }

    private IEnumerator ClickHand()
    {
        Transform hand = handPointer.transform.Find("Hand");
        Transform rings = handPointer.transform.Find("Circles");
        Image ring1 = handPointer.transform.Find("Circles/Circle1").GetComponent<Image>();
        Image ring2 = handPointer.transform.Find("Circles/Circle2").GetComponent<Image>();

        Vector3 handStart = Vector3.one;
        Vector3 handTarget = new Vector3(0.85f, 0.85f, 0.85f);

        Vector3 ringsStart = Vector3.zero;
        Vector3 ringsTarget = new Vector3(3.5f, 3.5f, 3.5f);

        Color startColor = Color.white;
        Color targetColor = startColor;
        targetColor.a = 0f;

        float time = 0f;
        float duration = 0.1f;
        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            hand.localScale = Vector3.Lerp(handStart, handTarget, t);
            yield return null;
        }

        time = 0f;
        duration = 0.7f;
        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            hand.localScale = Vector3.Lerp(handTarget, handStart, t);
            rings.localScale = Vector3.Lerp(ringsStart, ringsTarget, t);

            ring1.color = Color.Lerp(startColor, targetColor, t);
            ring2.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }
    }

    private void CreateGrid()
    {
        pointGrid = new Point[gridSize, gridSize];
        float borderPosition = (gridSize - 1) * pointScale * 0.5f;

        // Start form top left corner
        float yPos = borderPosition;
        for (int y = 0; y < pointGrid.GetLength(0); y++)
        {
            float xPos = -borderPosition;
            for (int x = 0; x < pointGrid.GetLength(1); x++)
            {
                Point p;
                var preloadPoints = tutorialTasks[task][subTaskIndex].preloadPoints;
                PointObj newPoint = null;

                if (preloadPoints != null)
                {
                    for (int i = 0; i < preloadPoints.Length; i++)
                    {
                        if (preloadPoints[i].index.IsEqual(new Index(y, x)))
                        {
                            newPoint = preloadPoints[i];
                            break;
                        }
                    }
                }

                if (newPoint != null)
                {
                    p = CreatePoint(pointPrefab, pointsHolder, new Vector2(xPos, yPos), newPoint);

                }
                else
                {
                    p = CreatePoint(pointPrefab, pointsHolder, new Vector2(xPos, yPos),
                        new PointObj(0, ColorType.Empty, new Index(y, x), true, false, false, false));
                }

                pointGrid[y, x] = p;
                xPos += pointScale;
            }
            yPos -= pointScale;
        }
    }

    private void ClearGrid()
    {
        for (int y = 0; y < pointGrid.GetLength(0); y++)
        {
            for (int x = 0; x < pointGrid.GetLength(1); x++)
            {
                var preloadPoints = tutorialTasks[task][subTaskIndex].preloadPoints;
                PointObj newPoint = null;

                for (int i = 0; i < preloadPoints.Length; i++)
                {
                    if (preloadPoints[i].index.IsEqual(new Index(y, x)))
                    {
                        newPoint = preloadPoints[i];
                        Point p = GetPoint(y, x);
                        p.Tier = newPoint.tier;
                        p.color = newPoint.color;
                        p.IsEmpty = newPoint.isEmpty;
                        p.IsControl = newPoint.isControl;
                        p.IsCombine = newPoint.isCombine;
                        p.IsBomb = newPoint.isBomb;
                        break;
                    }
                }

                if (newPoint == null) GetPoint(y, x).IsEmpty = true;
            }
        }

        score.SetScore(0);
        scoreNum = 0;
    }

    private void InitControlPoints()
    {
        pointsOrderIndexes = new int[4] { 0, 0, 0, 0 };
        for (int i = 0; i < controlPointHolders.Length; i++)
        {
            CreateControlPoint(i, GetPointsOrderIndex(i));
        }
    }

    private void CreateRandomControlPoint(int i)
    {    
        Point p = CreatePoint(controlPointPrefab, controlPointHolders[i].transform, Vector3.zero,
            new PointObj(Random.Range(1, 4), (ColorType)Random.Range(0, 5),
            new Index(0, -i), false, true, false, i == 0));
    
        controlPointHolders[i].point = p;
    }

    private void CreateControlPoint(int i, PointObj point)
    {
        Point p = CreatePoint(controlPointPrefab, controlPointHolders[i].transform, Vector3.zero, point);
        controlPointHolders[i].point = p;
    }

    private void CreateCombinePoint(Point pointFrom, float offsetZ, Point[] pointTo, bool setTarget)
    {
        Transform t = Instantiate(combinePointPrefab);

        Point tP = t.GetComponent<Point>();
        CombinePoint tCP = t.GetComponent<CombinePoint>();

        t.position = pointFrom.transform.position + new Vector3(0f, 0f, offsetZ);

        tP.color = pointFrom.color;
        tP.Tier = pointFrom.Tier;
        tP.IsCombine = true;
        tP.IsBomb = pointFrom.IsBomb;
        if (setTarget)
        {
            tCP.MoveTo(pointTo, combineDuration, pointFrom.color, pointFrom.IsBomb, pointFrom.Tier);
        }
        else
        {
            tCP.MoveTo(pointTo, combineDuration, pointFrom.color, false, -1);
        }
    }

    private void CreateCombinePoint(Vector3 posFrom, HistoryPoint pointFrom, Point[] pointTo, bool setTarget)
    {
        Transform t = Instantiate(combinePointPrefab);

        Point tP = t.GetComponent<Point>();
        CombinePoint tCP = t.GetComponent<CombinePoint>();

        t.position = posFrom;
        tP.color = pointFrom.color;
        tP.Tier = pointFrom.tier;
        tP.IsCombine = true;
        tP.IsBomb = pointFrom.isSuper;
        if (setTarget)
        {
            tCP.MoveTo(pointTo, combineDuration, pointFrom.color, pointFrom.isSuper, pointFrom.tier);
        }
        else
        {
            tCP.MoveTo(pointTo, combineDuration, pointFrom.color, pointFrom.isSuper, -1);
        }
    }

    private Point CreatePoint(Transform prefab, Transform parent, Vector3 position, PointObj pointObj)
    {
        Transform pointTransform = Instantiate(prefab);
        pointTransform.SetParent(parent, false);

        Point point = pointTransform.GetComponent<Point>();
        point.transform.localPosition = position;
        point.transform.localScale = new Vector3(pointScale, pointScale, pointScale);
        point.color = pointObj.color;
        point.Tier = pointObj.tier;
        point.index = pointObj.index;
        point.IsEmpty = pointObj.isEmpty;

        if (pointObj.isControl) point.IsControl = pointObj.isControl;
        if (pointObj.isCombine) point.IsCombine = pointObj.isCombine;
        if (pointObj.isBomb) point.IsBomb = pointObj.isBomb;

        return point;
    }

    public override void SetPoint(int index)
    {
        if (CurrentPoint.IsEmpty)
        {
            //audioCtrl.PlaySound(Sound.Post);
            bool bombPoint = index == 0;

            Point p = controlPointHolders[index].point;
            CurrentPoint.Tier = p.Tier;
            CurrentPoint.color = p.color;
            CurrentPoint.IsEmpty = false;
            CurrentPoint.IsBomb = bombPoint;

            // History capacity limitation
            if (history.Count >= historyCapacity) history.RemoveAt(0);
            history.Add(new History(CurrentPoint, new HistoryPoint(p), index));

            CreateControlPoint(index, GetPointsOrderIndex(index));
            CheckForSame(CurrentIndex, true);

            undoButton.interactable = true;
            if (bombPoint) GameTempData.Hints -= 1;
        }
    }

    public void CheckForSame(Index i, bool writeToHistory)
    {
        samePoints.Clear();
        combinePoints.Clear();
        combinePointsGrouped.Clear();
        combinePointsGrouped.Add(null);

        List<HistoryPoint> historyPoints = new List<HistoryPoint>();
        currentPoint = GetPoint(i);
        bool hasSame = false;
        int scoreToAdd = 0;

        // Check for same points
        samePoints.Add(currentPoint); // skip current point
        combinePoints.Add(currentPoint);

        FindSamePoints(currentPoint);

        if (!hasSuperPoint)
        {
            for (int k = 1; k < samePoints.Count; k++)
            {
                // Check for Bomb Point
                if (samePoints[k].IsBomb)
                {
                    hasSuperPoint = true;
                    bombPointIndex = samePoints[k].index;
                }
            }
        }

        // If bomb point, reinit with bomb at center
        if (!currentPoint.IsBomb && hasSuperPoint)
        {
            // Add to history
            for (int k = 1; k < samePoints.Count; k++)
            {
                historyPoints.Add(new HistoryPoint(samePoints[k].index, samePoints[k].Tier,
                    samePoints[k].color, samePoints[k].transform.position, samePoints[k].IsBomb));
            }
            history.Last().AddCombinePoints(historyPoints);
            history.Last().AddTargetPoints(combinePointsGrouped.Skip(1).ToList());


            StartCoroutine(CheckRecursion(bombPointIndex, false));
            return;
        }
        // Create combine points
        else if (samePoints.Count > 2)
        {
            for (int k = 1; k < samePoints.Count; k++)
            {
                // Create combine points
                Point[] targetPoints = combinePointsGrouped[k].ToArray();
                CreateCombinePoint(samePoints[k], -0.1f * k, targetPoints, false);

                // Add to history
                historyPoints.Add(new HistoryPoint(samePoints[k].index, samePoints[k].Tier,
                    samePoints[k].color, samePoints[k].transform.position, samePoints[k].IsBomb));

                samePoints[k].IsEmpty = true;
            }

            hasSame = true;
        }



        // Apply the result
        if (hasSame)
        {
            if (writeToHistory)
            {
                history.Last().AddCombinePoints(historyPoints);
                history.Last().AddTargetPoints(combinePointsGrouped.Skip(1).ToList());
            }

            if (currentPoint.IsBomb)
            {
                //audioCtrl.PlaySound(Sound.Super);
                currentPoint.SetEmptyWithAnimation();
                scoreToAdd += samePoints.Count;
            }
            else
            {
                //audioCtrl.PlaySound(Sound.Combine);
                currentPoint.Tier += 1;

                if (currentPoint.Tier > Mathf.Max(achiveLimitTier, reachedTier))
                {
                    reachedTier = currentPoint.Tier;
                    scoreToAdd += currentPoint.Tier * samePoints.Count * 2;
                }
                else
                {
                    scoreToAdd += currentPoint.Tier * samePoints.Count;
                }

                maxTier[(int)currentPoint.color] = Mathf.Max(currentPoint.Tier - 2, maxTier[(int)currentPoint.color]);
            }
            StartCoroutine(CheckRecursion(i, true));

            // Apply score
            history.Last().ScoreToAdd += scoreToAdd;
            StartCoroutine(StepChangingScore(scoreToAdd, scoreChangingDuration));
        }
        else
        {
            // Reset values
            this.i = Index.negative;
            bombPointIndex = Index.negative;
            hasSuperPoint = false;
            allowInput = true;
        }
    }

    private void FindSamePoints(Point point)
    {
        Index i = point.index;
        Index[] compareIndexes = new Index[]
        {
            new Index(i.y, i.x - 1),
            new Index(i.y - 1, i.x),
            new Index(i.y, i.x + 1),
            new Index(i.y + 1, i.x),
        };

        for (int k = 0; k < compareIndexes.Length; k++)
        {
            if (IsValidIndex(compareIndexes[k]))
            {
                Point comparePoint = GetPoint(compareIndexes[k]);

                bool isSame = hasSuperPoint ? ComparePointsSuper(comparePoint, point) : ComparePoints(comparePoint, point);
                if (!samePoints.Contains(comparePoint) && isSame)
                {
                    samePoints.Add(comparePoint);
                    combinePoints.Add(comparePoint);
                    combinePointsGrouped.Add(new List<Point>(combinePoints));

                    FindSamePoints(comparePoint);
                }
            }
        }

        combinePoints = new List<Point>();
        combinePoints.Add(currentPoint);
    }

    private void FindSuperPoint(Point point)
    {
        Index i = point.index;
        Index[] compareIndexes = new Index[]
        {
            new Index(i.y, i.x - 1),
            new Index(i.y - 1, i.x),
            new Index(i.y, i.x + 1),
            new Index(i.y + 1, i.x),
        };

        for (int k = 0; k < compareIndexes.Length; k++)
        {
            if (IsValidIndex(compareIndexes[k]))
            {
                Point comparePoint = GetPoint(compareIndexes[k]);
                if (!samePoints.Contains(comparePoint))
                {
                    if (comparePoint.IsBomb)
                    {
                        bombPointIndex = compareIndexes[k];
                        break;
                    }
                    else if (ComparePointsSuper(comparePoint, point))
                    {
                        samePoints.Add(comparePoint);
                        FindSuperPoint(comparePoint);
                    }
                }
            }
        }
    }

    private bool IsValidIndex(Index i)
    {
        if (i.y >= 0 && i.y < gridSize && i.x >= 0 && i.x < gridSize)
        {
            return true;
        }
        return false;
    }

    private Point GetPoint(Index i)
    {
        return pointGrid[i.y, i.x];
    }

    private Point GetPoint(int y, int x)
    {
        return pointGrid[y, x];
    }

    private bool ComparePoints(Point p1, Point p2)
    {
        if (!p1.IsEmpty && !p2.IsEmpty)
        {
            if (p1.color == p2.color && p1.Tier == p2.Tier || p1.IsBomb || p2.IsBomb)
            {
                return true;
            }
        }

        return false;
    }

    private bool ComparePointsSuper(Point p1, Point p2)
    {
        if (!p1.IsEmpty && !p2.IsEmpty)
        {
            if (p1.color == p2.color && p1.Tier == p2.Tier || p1.IsBomb || p2.IsBomb)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator CheckRecursion(Index index, bool writeToHistory)
    {
        yield return new WaitForSeconds(multipleCombinesDelay);

        CheckForSame(index, writeToHistory);
    }

    #region Public accesors

    public override Index CurrentIndex
    {
        get
        {
            return i;
        }
        set
        {
            if (allowPointSetting)
            {
                i = value;
            }
        }
    }

    public Point CurrentPoint
    {
        get
        {
            return GetPoint(i);
        }
    }

    public override Vector3 CurrentPointPos
    {
        get
        {
            return GetPoint(i).transform.position;
        }
    }

    public override bool AllowPointSetting
    {
        get
        {
            return allowPointSetting;
        }
        set
        {
            allowPointSetting = value;
        }
    }

    public bool IsCurrentControlPointSuper
    {
        get
        {
            return currentCPHolder.Index == 0;
        }
    }

    public bool PointerDown { get; set; }

    public override bool IsAnableToSet
    {
        get
        {
            bool b = !CurrentIndex.IsNegative() && CurrentPoint.IsEmpty;
            if (!b) allowInput = true;
            return b;
        }
    }

    #endregion

    /// <summary>
    /// Game History
    /// </summary>
    class History
    {
        Point point;
        HistoryPoint controlPoint;
        int controlPointHolderIndex, scoreToAdd = 0;
        List<List<HistoryPoint>> combinePoints = new List<List<HistoryPoint>>();
        List<List<List<Point>>> targetPoints = new List<List<List<Point>>>();

        public History(Point point, HistoryPoint controlPoint, int controlPointHolderIndex)
        {
            this.point = point;
            this.controlPoint = controlPoint;
            this.controlPointHolderIndex = controlPointHolderIndex;
        }

        public void AddCombinePoints(List<HistoryPoint> historyPoints)
        {
            //for(int i = 0; i < historyPoints.Count; i++)
            //{
            //    Debug.Log(
            //        $"hp i: x:{historyPoints[i].i.x} y:{historyPoints[i].i.y} " +
            //        $"t:{historyPoints[i].tier} " +
            //        $"c:{historyPoints[i].color} " +
            //        $"p:{historyPoints[i].pos} " +
            //        $"b:{historyPoints[i].isSuper}");
            //}

            combinePoints.Add(historyPoints);
        }

        public void AddTargetPoints(List<List<Point>> targetPoints)
        {
            //for (int i = 0; i < targetPoints.Count; i++)
            //{
            //    for (int k = 0; k < targetPoints[i].Count; k++)
            //    {
            //        Debug.Log(
            //           $"p i: x:{targetPoints[i][k].index.x} y:{targetPoints[i][k].index.y} " +
            //           $"t:{targetPoints[i][k].Tier} " +
            //           $"c:{targetPoints[i][k].color} " +
            //           $"e:{targetPoints[i][k].IsEmpty} " +
            //           $"ctr:{targetPoints[i][k].IsControl} " +
            //           $"com:{targetPoints[i][k].IsCombine} " +
            //           $"b:{targetPoints[i][k].IsBomb} "
            //
            //           );
            //    }                
            //}

            this.targetPoints.Add(targetPoints);
        }

        public Point Point
        {
            get
            {
                return point;
            }
        }

        public HistoryPoint ControlPoint
        {
            get
            {
                return controlPoint;
            }
        }

        public int GetLength()
        {
            return combinePoints.Count;
        }

        public List<HistoryPoint> GetCombinePoints(int i)
        {
            return combinePoints[i];
        }

        public List<List<Point>> GetTargetPoints(int i)
        {
            return targetPoints[i];
        }

        public int ControlPointHolderIndex
        {
            get
            {
                return controlPointHolderIndex;
            }
        }

        public int ScoreToAdd
        {
            get
            {
                return scoreToAdd;
            }
            set
            {
                scoreToAdd = value;
            }
        }
    }

    public struct HistoryPoint
    {
        public int tier;
        public Index i;
        public ColorType color;
        public Vector3 pos;
        public bool isSuper;

        public HistoryPoint(Index i, int tier, ColorType color, Vector3 pos, bool isSuper)
        {
            this.i = i;
            this.tier = tier;
            this.color = color;
            this.pos = pos;
            this.isSuper = isSuper;
        }

        public HistoryPoint(Point point)
        {
            i = point.index;
            tier = point.Tier;
            color = point.color;
            pos = point.transform.position;
            isSuper = point.IsBomb;
        }
    }

    /// <summary>
    /// UI Management
    /// </summary>

    public void ReloadControlPoints()
    {
        if (GameTempData.Hints > 0)
        {
            //audioCtrl.PlaySound(Sound.Click);
            for (int i = 1; i < controlPointHolders.Length; i++)
            {
                CreateRandomControlPoint(i);
            }
            GameTempData.Hints -= 1;
            SaveGameState();
        }
    }

    public void Undo()
    {
        if (GameTempData.Hints > 0 && history.Count > 0)
        {
            //audioCtrl.PlaySound(Sound.Click);
            History lastHistory = history.Last();

            for (int i = lastHistory.GetLength() - 1; i >= 0; i--)
            {
                List<HistoryPoint> combinePoints = lastHistory.GetCombinePoints(i);
                List<List<Point>> targetPoints = lastHistory.GetTargetPoints(i);
                for (int cp = 0; cp < combinePoints.Count; cp++)
                {
                    targetPoints[cp].Reverse();
                    CreateCombinePoint(lastHistory.Point.transform.position, combinePoints[cp], targetPoints[cp].ToArray(), true);
                    targetPoints[cp].Reverse();
                }
            }

            StartCoroutine(StepChangingScore(-lastHistory.ScoreToAdd, scoreChangingDuration));
            CreateControlPoint(lastHistory.ControlPointHolderIndex, new PointObj(lastHistory.ControlPoint.tier,
                lastHistory.ControlPoint.color, new Index(0, -lastHistory.ControlPointHolderIndex), false, true, false,
                lastHistory.ControlPointHolderIndex == 0));

            lastHistory.Point.IsEmpty = true;
            GameTempData.Hints -= 1;
        }
    }

    public IEnumerator StepChangingScore(int scoreToAdd, float duration)
    {
        float time = 0f;
        int newScore = scoreNum + scoreToAdd;

        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            scoreNum = (int)Mathf.Lerp(scoreNum, newScore, t);
            score.SetScore(scoreNum);

            yield return null;
        }
        scoreNum = newScore;
        score.SetScore(scoreNum);
    }
}


