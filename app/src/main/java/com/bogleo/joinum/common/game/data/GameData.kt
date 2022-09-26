package com.bogleo.joinum.common.game.data

import com.bogleo.joinum.common.game.models.PointCellData
import com.bogleo.joinum.common.utils.SharedPrefs
import javax.inject.Inject
import javax.inject.Singleton
import kotlin.math.max

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
            bestScore = max(bestScore, value)
        }

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

    var bestScore: Int
        get() = sharedPrefs.bestScore
        set(value) {
            sharedPrefs.bestScore = value
        }
}

