package com.bogleo.joinum.common.game

import android.annotation.SuppressLint
import android.content.Context
import android.graphics.*
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View
import androidx.core.content.ContextCompat
import com.bogleo.joinum.R
import com.bogleo.joinum.common.game.data.Index
import com.bogleo.joinum.common.game.core.Vector2
import com.bogleo.joinum.common.game.data.GameData
import com.bogleo.joinum.common.game.data.GameMode
import com.bogleo.joinum.common.game.models.PointCell
import kotlinx.coroutines.*


class GameView @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet?,
    defStyleAttr: Int = 0,
) : View(context, attrs, defStyleAttr) {

    var fieldCells: List<PointCell> = listOf()
        private set
    var inputCells: MutableList<PointCell> = mutableListOf()
        private set
    var inputCellsPos: MutableList<Vector2> = mutableListOf()
        private set
    val transitCells: MutableList<PointCell> = mutableListOf()

    val emptyColor: Int = context.getColor(R.color.gray_400)
    val colors: List<Int> = listOf(
        context.getColor(R.color.green),
        context.getColor(R.color.blue),
        context.getColor(R.color.yellow),
        context.getColor(R.color.orange),
        context.getColor(R.color.red),
    )

    var isPaused: Boolean = false
        set(value) {
            field = value
            redraw()
        }

    private var onTouchDown: ((x: Float, y: Float) -> Boolean)? = null
    private var onTouchMove: ((x: Float, y: Float) -> Boolean)? = null
    private var onTouchUp: ((x: Float, y: Float) -> Boolean)? = null

    fun setOnTouchDownListener(onTouchDownListener: (x: Float, y: Float) -> Boolean) {
        onTouchDown = onTouchDownListener
    }
    fun setOnTouchMoveListener(OnTouchMoveListener: (x: Float, y: Float) -> Boolean) {
        onTouchMove = OnTouchMoveListener
    }
    fun setOnTouchUpListener(OnTouchUpListener: (x: Float, y: Float) -> Boolean) {
        onTouchUp = OnTouchUpListener
    }

    private val fieldCellPaddingPercentage = 0.02f
    private val inputCellsDividerPercentage = 0.05f
    private var backgroundRoundnessPercentage: Float = 0.03f
    private val smallScale: Vector2 = Vector2(
        PointCell.SCALE_SMALL,
        PointCell.SCALE_SMALL
    )
    private val bigScale: Vector2 = Vector2(
        PointCell.SCALE_BIG,
        PointCell.SCALE_BIG
    )

    private var inputCellsCount = 3
    private var fieldCellsCount = 0
    private var desiredSizeProportion = (1080f + 1080f * inputCellsCount) / (1080f * inputCellsCount)
    private var cellSize: Float = 0f
    private var backgroundRoundness: Float = 0f

    private var backgrounds: List<RectF> = List(inputCellsCount + 1) { RectF() }
    private val backgroundPaint: Paint = Paint(Paint.ANTI_ALIAS_FLAG).apply {
        style = Paint.Style.FILL
    }

    private val coroutineScope: CoroutineScope = CoroutineScope(Job() + Dispatchers.Main)
    private var needRedraw = true

    init {
        context.theme.obtainStyledAttributes(
            attrs,
            R.styleable.GameView,
            0, 0
        ).apply {
            try {
                backgroundPaint.color = getColor(
                    R.styleable.GameView_fieldsTint,
                    ContextCompat.getColor(context, R.color.white)
                )
            } finally {
                recycle()
            }
        }

        coroutineScope.launch {
            gameLoop()
        }
    }

    fun init(gameData: GameData) {
        inputCellsCount = gameData.gameMode.inputCellsCount
        fieldCellsCount = gameData.gameMode.fieldCellsCount
        desiredSizeProportion = (1080f + 1080f * inputCellsCount) / (1080f * inputCellsCount)

        val cellColor = if (gameData.gameMode.session == GameMode.SESSION_NEW) emptyColor
                        else null

        val tierColor = context.getColor(R.color.gray_900)
        val tierTypeface = context.resources.getFont(R.font.montserrat_medium)

        fieldCells = List(gameData.fieldCells.size) {
            PointCell().apply {
                transform.scale = smallScale
                data = gameData.fieldCells[it]
                cellColor?.let { if(color == 0) color = it }
                index = Index(it % fieldCellsCount, it / fieldCellsCount)
                if(tier > 0) startScale(PointCell.SCALE_BIG) { redraw() }
                setupTierPaint(tierColor, tierTypeface)
            }
        }

        gameData.inputCells.forEachIndexed { i, pointCellData ->
            inputCells.add(PointCell().apply {
                transform.scale = bigScale
                data = pointCellData
                index = Index(i, -1)
                setupTierPaint(tierColor, tierTypeface)
            })
        }

        backgrounds = List(inputCellsCount + 1) { RectF() }
    }

    fun redraw() { if(!needRedraw) needRedraw = true }

    fun setOnTop(pointCell: PointCell)  {
        val currentIndex = inputCells.indexOf(pointCell)
        if (currentIndex < 0) return
        val pointCellPos = inputCellsPos[currentIndex]
        inputCells.removeAt(currentIndex)
        inputCells.add(pointCell)
        inputCellsPos.removeAt(currentIndex)
        inputCellsPos.add(pointCellPos)
    }

    override fun onSizeChanged(w: Int, h: Int, oldw: Int, oldh: Int) {
        super.onSizeChanged(w, h, oldw, oldh)

        val paddedWidth = w - (paddingLeft + paddingRight)
        val paddedHeight = h - (paddingTop + paddingBottom)

        // Calculate padding if less than desired proportion
        val proportion = paddedHeight.toFloat() / paddedWidth.toFloat()
        var padding = 0f
        if(proportion < desiredSizeProportion) {
            val desiredWidth = paddedHeight / desiredSizeProportion
            padding = paddedWidth - desiredWidth
        }

        // Get cell size
        val fieldWidth = paddedWidth - padding
        val halfPadding = padding * 0.5f

        // Get space between field and input cells
        val innerSpace = paddedHeight - (fieldWidth + fieldWidth / inputCellsCount)
        // Get delta to center fields on tall screens
        val deltaSpace = innerSpace / 4

        val fieldPadding = fieldWidth * fieldCellPaddingPercentage
        val fieldPaddedWidth = fieldWidth - fieldPadding * 2
        cellSize = fieldPaddedWidth / fieldCellsCount
        // Calculate position offset
        val halfCellSize = cellSize * 0.5f
        val fieldCellOffsetX = halfCellSize + halfPadding + fieldPadding + paddingLeft
        val fieldCellOffsetY = halfCellSize + fieldPadding + paddingTop + deltaSpace

        // Set field cells transform
        fieldCells.forEachIndexed { index, pointCell ->
            with(pointCell.transform) {
                position = Vector2(
                    (index % fieldCellsCount) * cellSize + fieldCellOffsetX,
                    (index / fieldCellsCount) * cellSize + fieldCellOffsetY
                )
                size = Vector2(cellSize, cellSize)
                setCollider(size)
            }
        }

        // Set transit cells size
        transitCells.forEach { pointCell ->
            pointCell.transform.size = Vector2(cellSize, cellSize)
        }

        // Get input cells divider length
        val inputCellsDividerLength = fieldWidth * inputCellsDividerPercentage
        // Calculate input cell size
        val inputCellSize = (fieldWidth - ((inputCellsCount - 1) * inputCellsDividerLength)) / inputCellsCount
        val halfInputCellSize = inputCellSize * 0.5f
        // Calculate position offset
        val inputCellOffsetX = halfInputCellSize + halfPadding + paddingLeft
        val inputDeltaOffsetY = deltaSpace * 1.5f
        val inputCellPosY = h - halfInputCellSize - paddingBottom - inputDeltaOffsetY

        // Set input cells transform
        inputCellsPos.clear()
        inputCells.forEachIndexed { index, pointCell ->
            with(pointCell.transform) {
                position = Vector2(
                    index * (inputCellSize + inputCellsDividerLength) + inputCellOffsetX,
                    inputCellPosY
                )
                inputCellsPos.add(position)
                size = Vector2(cellSize, cellSize)
                setCollider(Vector2(inputCellSize, inputCellSize))
            }
        }

        // Calculate backgrounds
        val fieldBackgroundOffsetY = paddingTop + deltaSpace
        backgrounds[0].apply {
            left = halfPadding + paddingLeft
            top = fieldBackgroundOffsetY
            right = w - halfPadding - paddingRight
            bottom = fieldWidth + fieldBackgroundOffsetY
        }

        val bottomPos = h - paddingBottom - inputDeltaOffsetY
        val topPos = bottomPos - inputCellSize
        for(i in 1 until backgrounds.size) {
            val leftPos = (i - 1) * (inputCellSize + inputCellsDividerLength) + halfPadding + paddingLeft
            backgrounds[i].apply {
                left = leftPos
                top = topPos
                right = leftPos + inputCellSize
                bottom = bottomPos
            }
        }
        // Calculate background roundness
        backgroundRoundness = w * backgroundRoundnessPercentage
    }

    private suspend fun gameLoop() {
        if(needRedraw) {
            needRedraw = false
            invalidate()
        }
        delay(16)
        gameLoop()
    }

    override fun onDraw(canvas: Canvas) {

        backgrounds.forEach { it.drawRound(canvas) }
        if(!isPaused) {
            fieldCells.forEach { it.draw(canvas) }
            transitCells.forEach { it.draw(canvas) }
            inputCells.forEach { it.draw(canvas) }
        }

        super.onDraw(canvas)
    }

    @SuppressLint("ClickableViewAccessibility")
    override fun onTouchEvent(event: MotionEvent?): Boolean {
        event ?: return false

        return if(isEnabled) {
            when(event.action) {
                MotionEvent.ACTION_DOWN -> onTouchDown?.let { it(event.x, event.y) } ?: false
                MotionEvent.ACTION_MOVE -> onTouchMove?.let { it(event.x, event.y) } ?: false
                MotionEvent.ACTION_UP   -> onTouchUp?.let { it(event.x, event.y) } ?: false
                else -> false
            }
        } else {
            false
        }
    }

    private fun RectF.drawRound(canvas: Canvas) {
        canvas.drawRoundRect(
            this,
            backgroundRoundness,
            backgroundRoundness,
            backgroundPaint
        )
    }
}