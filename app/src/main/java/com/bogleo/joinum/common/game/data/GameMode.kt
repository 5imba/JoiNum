package com.bogleo.joinum.common.game.data

data class GameMode(
    val fieldCellsCount: Int,
    val inputCellsCount: Int,
    val colorsCount: Int,
    val session: Int
) {

    companion object {

        val default: GameMode get() = GameMode(
            fieldCellsCount = 9,
            inputCellsCount = 3,
            colorsCount = 3,
            session = SESSION_NEW
        )

        fun fromDifficulty(difficulty: Int) = GameMode(
            fieldCellsCount = 9,
            inputCellsCount = 3,
            colorsCount = difficulty,
            session = SESSION_NEW
        )

        const val SESSION_NEW = 0
        const val SESSION_RESUME = 1
    }
}