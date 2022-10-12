package com.bogleo.joinum.common.utils

import android.content.Context
import android.content.SharedPreferences
import com.bogleo.joinum.R
import com.bogleo.joinum.common.extensions.fromJson
import com.bogleo.joinum.common.extensions.toJson
import com.bogleo.joinum.common.game.data.GameData
import com.bogleo.joinum.common.game.data.GameMode
import dagger.hilt.android.qualifiers.ApplicationContext
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class SharedPrefs @Inject constructor(
    @ApplicationContext context: Context
)  {

    private val prefs: SharedPreferences = context.getSharedPreferences(
        context.getString(R.string.prefs_group),
        Context.MODE_PRIVATE
    )

    companion object {
        private const val GAME_DATA = "GAME_DATA"
        private const val GAME_MODE = "GAME_MODE"
        private const val THEME_MODE = "THEME_MODE"
        private const val ALLOW_SOUND = "ALLOW_SOUND"

        const val BEST_SCORE = "BEST_SCORE"
        const val MOST_MOVES = "MOST_MOVES"
        const val MAX_TIER = "MAX_TIER"
        const val FINISHED_GAMES = "FINISHED_GAMES"
    }

    var gameMode: GameMode
        get() {
            val json = prefs.getString(GAME_MODE, "")
            return json?.fromJson<GameMode>() ?: GameMode.default
        }
        set(value) = prefs.edit().putString(GAME_MODE, value.toJson()).apply()

    var gameData: GameData?
        get() {
            val json = prefs.getString(GAME_DATA, "")
            return json?.fromJson<GameData>()
        }
        set(value) {
            val json = value?.toJson()
            prefs.edit().putString(GAME_DATA, json).apply()
        }

    var themeMode: Int
        get() = prefs.getInt(THEME_MODE, 0)
        set(value) = prefs.edit().putInt(THEME_MODE, value).apply()

    var allowSound: Boolean
        get() = prefs.getBoolean(ALLOW_SOUND, true)
        set(value) = prefs.edit().putBoolean(ALLOW_SOUND, value).apply()

    //region Statistics

    fun getBestScore(difficult: Int = 0): Int {
        return prefs.getInt(BEST_SCORE.format(difficult), 0)
    }

    fun setBestScore(value: Int, difficult: Int = 0) {
        prefs.edit().putInt(BEST_SCORE.format(difficult), value).apply()
    }

    fun getFinishedGames(difficult: Int = 0): Int {
        return prefs.getInt(FINISHED_GAMES.format(difficult), 0)
    }

    fun setFinishedGames(value: Int, difficult: Int = 0) {
        prefs.edit().putInt(FINISHED_GAMES.format(difficult), value).apply()
    }

    fun getMaxTier(difficult: Int = 0): Int {
        return prefs.getInt(MAX_TIER.format(difficult), 0)
    }

    fun setMaxTier(value: Int, difficult: Int = 0) {
        prefs.edit().putInt(MAX_TIER.format(difficult), value).apply()
    }

    fun getMostMoves(difficult: Int = 0): Int {
        return prefs.getInt(MOST_MOVES.format(difficult), 0)
    }

    fun setMostMoves(value: Int, difficult: Int = 0) {
        prefs.edit().putInt(MOST_MOVES.format(difficult), value).apply()
    }

    private fun String.format(modifier: Int): String = "${this}_$modifier"

    //endregion

    fun clearPrefs() = prefs.edit().clear().apply()
}
