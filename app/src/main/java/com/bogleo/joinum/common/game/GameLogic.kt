package com.bogleo.joinum.common.game

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import com.bogleo.joinum.R
import com.bogleo.joinum.common.game.data.Index
import com.bogleo.joinum.common.game.core.Vector2
import com.bogleo.joinum.common.game.data.GameData
import com.bogleo.joinum.common.game.data.GameMode
import com.bogleo.joinum.common.game.models.PointCell
import com.bogleo.joinum.common.game.models.isHovered
import javax.inject.Inject
import javax.inject.Singleton
import kotlin.random.Random

@Singleton
class GameLogic @Inject constructor() {
    private lateinit var gameView: GameView
    private lateinit var gameData: GameData

    private val _score: MutableLiveData<String> = MutableLiveData<String>()
    val score: LiveData<String> = _score

    private var onGameOverCallback: ((score: Int, bestScore: Int) -> Unit)? = null

    private var currentInputCell: PointCell? = null
    private var currentHoveredPointCell: PointCell? = null
    private var currentFieldPointCell: PointCell? = null

    private val samePointCells: MutableList<PointCell> = mutableListOf()
    private val transitCells: MutableList<PointCell> = mutableListOf()
    private val transitGroupedCells: MutableList<MutableList<PointCell>> = mutableListOf()

    private var allowInput = true
    private var comboCounter = 0

    fun init(gameView: GameView, gameData: GameData) {
        // Set fields
        this.gameView = gameView
        this.gameData = gameData

        // Set touch listeners
        gameView.setOnTouchDownListener(::onTouchDown)
        gameView.setOnTouchMoveListener(::onTouchMove)
        gameView.setOnTouchUpListener(::onTouchUp)

        // Init game
        gameData.init()
        gameView.init(gameData)

        // Set Input PointCells if new session
        if(gameData.gameMode.session == GameMode.SESSION_NEW) {
            gameView.inputCells.forEach {
                updateInputPointCell(it)
            }
        }
        _score.postValue(scoreText)
    }

    fun setOnGameOverListener(onGameOverCallback: ((score: Int, bestScore: Int) -> Unit)) {
        this.onGameOverCallback = onGameOverCallback
    }

    private fun onTouchDown(x: Float, y: Float): Boolean {
        if(allowInput) {
            // Check if touch position on input cell
            currentInputCell = gameView.inputCells.find { pointCell ->
                pointCell.isHovered(x, y)
            }
            // Set selected cell on top of other
            currentInputCell?.let {
                gameView.setOnTop(it)
                return true
            }
        }
        return false
    }

    private fun onTouchMove(x: Float, y: Float): Boolean {
        currentInputCell?.let { pointCell ->
            // Follow pointer
            pointCell.transform.position = Vector2(x, y)

            // Scale up cell on hover
            var isHovered = false
            gameView.fieldCells.forEach {
                if(it.isHovered(x, y)) {
                    isHovered = true
                    if(it != currentHoveredPointCell) {
                        it.onHoverEnter { gameView.redraw() }
                        currentHoveredPointCell?.onHoverExit { gameView.redraw() }
                        currentHoveredPointCell = it
                    }
                }
            }
            // Scale down if hover exit
            if(!isHovered) {
                currentHoveredPointCell?.onHoverExit { gameView.redraw() }
                currentHoveredPointCell = null
            }
            gameView.redraw()
            return true
        }
        return false
    }

    private fun onTouchUp(x: Float, y: Float): Boolean {
        currentInputCell?.let { pointCell ->
            var isHovered = false
            // Find target Field PointCell
            gameView.fieldCells.forEach { fieldCell ->
                if(fieldCell.isHovered(x, y)) {
                    // Move to hovered cell if is empty
                    if(fieldCell.tier == 0) {
                        isHovered = true
                        // On PointCell set callback (main logic here)
                        pointCell.moveTo(fieldCell.transform.position, updateListener = ::drawUpdateListener) {
                            // Set PointCell data
                            fieldCell.data = pointCell.data
                            fieldCell.startScale(PointCell.SCALE_BIG, updateListener = ::drawUpdateListener)
                            // Create new Input PointCell
                            updateInputPointCell(pointCell)
                            // Start search recursion
                            checkForSame(fieldCell)
                            // Save game if empty cells left, else call GameOver
                            if(!checkIsGameOver()) {
                                saveGame()
                            }
                        }
                    }
                }
            }
            // If none cells hovered return to start position
            if(!isHovered) {
                val index = gameView.inputCells.indexOf(pointCell)
                pointCell.moveTo(gameView.inputCellsPos[index], updateListener = ::drawUpdateListener)
            }
            return true
        }
        return false
    }

    private fun checkIsGameOver(): Boolean {
        var emptyCellLeft = false
        // Find empty cells
        gameView.fieldCells.forEach {
            if(it.tier == 0) {
                emptyCellLeft = true
            }
        }
        // If none found call GameOver and clear session data
        val isGameOver = !emptyCellLeft
        if(isGameOver) {
            onGameOverCallback?.let { it(gameData.currentScore, gameData.bestScore) }
            gameData.clearData()
        }
        return isGameOver
    }

    private fun checkForSame(pointCell: PointCell) {
        allowInput = false
        // Clear fields
        samePointCells.clear()
        gameView.transitCells.clear()
        transitCells.clear()
        transitGroupedCells.clear()
        // Init fields
        currentFieldPointCell = pointCell
        samePointCells.add(pointCell)
        transitCells.add(pointCell)
        transitGroupedCells.add(transitCells)

        // Start search recursion
        findSamePointCells(pointCell)
        // If more then 3 cell found apply result
        if(samePointCells.size > 2) {
            comboCounter++
            samePointCells.forEachIndexed { i, pc ->
                if(i > 0) {
                    // Make cell for combine animation
                    gameView.transitCells.add(
                        PointCell().apply {
                            data = pc.data
                            transform.position = pc.transform.position
                            transform.scale = pc.transform.scale
                            transform.size = pc.transform.size
                            moveTo(
                                transitGroupedCells[i].map { it.transform.position },
                                updateListener = { gameView.redraw() },
                                doOnEnd = { removeTransitPointCell(this, pointCell) }
                            )
                        }
                    )
                    // Reset cell
                    pc.tier = 0
                    pc.color = gameView.emptyColor
                    pc.startScale(PointCell.SCALE_SMALL) { gameView.redraw() }
                }
            }
        } else {
            // Search is finish, reinit values
            allowInput = true
            comboCounter = 0
        }
    }

    private fun findSamePointCells(pointCell: PointCell) {
        val i = pointCell.index
        // Search directions
        val indexSet: List<Index> = listOf(
            Index(x = i.x - 1, y = i.y    ),
            Index(x = i.x,     y = i.y - 1),
            Index(x = i.x + 1, y = i.y    ),
            Index(x = i.x,     y = i.y + 1)
        )
        // Start search
        indexSet.forEach {
            if(validateIndex(it)) {
                val comparePoint = getFieldPointCell(it)
                if(!samePointCells.contains(comparePoint) && pointCell.isSame(comparePoint)) {
                    samePointCells.add(comparePoint)
                    // Save combine cells stack
                    transitCells.add(comparePoint)
                    transitGroupedCells.add(transitCells.toMutableList())

                    findSamePointCells(comparePoint)
                }
            }
        }
        transitCells.clear()
        currentFieldPointCell?.let { transitCells.add(it) }
    }

    private fun validateIndex(i: Index): Boolean {
        return i.y >= 0 && i.y < gameData.gameMode.fieldCellsCount
            && i.x >= 0 && i.x < gameData.gameMode.fieldCellsCount
    }

    private fun getFieldPointCell(i: Index): PointCell {
        return gameView.fieldCells[i.y * gameData.gameMode.fieldCellsCount + i.x]
    }

    private fun updateInputPointCell(pointCell: PointCell) {
        val randomIndex = Random.nextInt(0, gameData.gameMode.colorsCount)
        with(pointCell) {
            val index = gameView.inputCells.indexOf(this)
            if(index < gameView.inputCellsPos.size) {
                transform.position = gameView.inputCellsPos[index]
            }
            tier = Random.nextInt(1, gameData.maxTier[randomIndex])
            color = gameView.colors[randomIndex]
        }
    }

    private fun removeTransitPointCell(toRemove: PointCell, currentPointCell: PointCell) {
        gameView.transitCells.remove(toRemove)
        if(gameView.transitCells.isEmpty()) {
            currentPointCell.tier += 1
            val colorIndex = gameView.colors.indexOf(currentPointCell.color)
            if(currentPointCell.tier > gameData.maxTier[colorIndex]) {
                gameData.maxTier[colorIndex] = currentPointCell.tier
            }
            gameData.currentScore += samePointCells.size * comboCounter + transitGroupedCells.size
            _score.postValue(scoreText)
            saveGame()
            checkForSame(currentPointCell)
        }
    }

    private val scoreText get() = gameView.context.getString(R.string.score_label) +
                " ${gameData.currentScore}"

    private fun drawUpdateListener() {
        gameView.redraw()
    }

    private fun saveGame() {
        gameData.saveGameData(
            fieldCells = gameView.fieldCells.map { it.data },
            inputCells = gameView.inputCells.map { it.data }
        )
    }

    private fun PointCell.isSame(pointCell: PointCell): Boolean {
        return tier == pointCell.tier && color == pointCell.color
    }

}