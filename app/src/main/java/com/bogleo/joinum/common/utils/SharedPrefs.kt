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
        private const val BEST_SCORE = "BEST_SCORE"
        private const val THEME_MODE = "THEME_MODE"
        private const val ALLOW_SOUND = "ALLOW_SOUND"
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

    var bestScore: Int
        get() = prefs.getInt(BEST_SCORE, 0)
        set(value) = prefs.edit().putInt(BEST_SCORE, value).apply()

    var themeMode: Int
        get() = prefs.getInt(THEME_MODE, 0)
        set(value) = prefs.edit().putInt(THEME_MODE, value).apply()

    var allowSound: Boolean
        get() = prefs.getBoolean(ALLOW_SOUND, true)
        set(value) = prefs.edit().putBoolean(ALLOW_SOUND, value).apply()

    fun clearPrefs() = prefs.edit().clear().apply()
}
