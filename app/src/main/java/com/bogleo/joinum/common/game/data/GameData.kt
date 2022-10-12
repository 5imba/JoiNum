package com.bogleo.joinum.common.game.data

import com.bogleo.joinum.common.game.models.PointCellData
import com.bogleo.joinum.common.utils.SharedPrefs
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class GameData @Inject constructor(
    @Transient private val sharedPrefs: SharedPrefs
) {
    lateinit var gameMode: GameMode
    lateinit var fieldCells: List<PointCellData>
        private set
    lateinit var inputCells: List<PointCellData>
        private set
    val maxTier: MutableList<Int> = mutableListOf()
    var currentScore: Int = 0
        set(value) {
            field = value
            with(sharedPrefs) {
                // Set total BestScore
                if(value > totalBestScore) {
                    setBestScore(
                        value = value
                    )
                }
                // Set current BestScore
                if(value > currentBestScore) {
                    setBestScore(
                        value = value,
                        difficult = currentDifficult
                    )
                }
            }
        }

    val totalBestScore: Int get() = sharedPrefs.getBestScore()
    val currentBestScore: Int get() = sharedPrefs.getBestScore(difficult = currentDifficult)

    private val currentDifficult: Int get() = gameMode.colorsCount

    fun init() {
        val savedGameData = sharedPrefs.gameData
        val savedGameMode = sharedPrefs.gameMode
        if(savedGameMode.session == GameMode.SESSION_RESUME
            && savedGameData != null
        ) {
            savedGameData.also { gameData ->
                gameMode = gameData.gameMode.copy(session = GameMode.SESSION_RESUME)
                fieldCells = gameData.fieldCells
                inputCells = gameData.inputCells
                currentScore = gameData.currentScore
                maxTier.clear()
                maxTier.addAll(gameData.maxTier)
            }
        } else {
            gameMode = savedGameMode
            fieldCells = List(gameMode.fieldCellsCount * gameMode.fieldCellsCount) {
                PointCellData(tier = 0, color = 0)
            }
            inputCells = List(gameMode.inputCellsCount) {
                PointCellData(tier = 0, color = 0)
            }
            maxTier.clear()
            for(i in 0..gameMode.colorsCount) {
                maxTier.add(3)
            }
        }
    }

    fun saveGameData(
        fieldCells: List<PointCellData>,
        inputCells: List<PointCellData>,
        changeSession: Boolean = true
    ) {
        if(gameMode.session == GameMode.SESSION_NEW && changeSession) {
            gameMode = gameMode.copy(session = GameMode.SESSION_RESUME)
            sharedPrefs.gameMode = gameMode
        }
        this.fieldCells = fieldCells
        this.inputCells = inputCells
        sharedPrefs.gameData = this
    }

    fun clearData() {
        sharedPrefs.gameMode = gameMode.copy(session = GameMode.SESSION_NEW)
        saveGameData(listOf(), listOf(), false)
    }

    fun incrementMostMoves() {
        with(sharedPrefs) {
            // Total moves
            setMostMoves(
                value = getMostMoves() + 1
            )
            // Current difficulty moves
            setMostMoves(
                value = getMostMoves(difficult = currentDifficult) + 1,
                difficult = currentDifficult
            )
        }
    }

    fun incrementFinishedGames() {
        with(sharedPrefs) {
            // Total games
            setFinishedGames(
                value = getFinishedGames() + 1
            )
            // Current difficulty games
            setFinishedGames(
                value = getFinishedGames(difficult = currentDifficult) + 1,
                difficult = currentDifficult
            )
        }
    }

    fun setMaxTier(tier: Int) {
        with(sharedPrefs) {
            // Total max tier
            if(tier > getMaxTier()) {
                setMaxTier(
                    value = tier
                )
            }
            // Current difficulty max tier
            if(tier > getMaxTier(difficult = currentDifficult)) {
                setMaxTier(
                    value = tier,
                    difficult = currentDifficult
                )
            }
        }
    }
}

