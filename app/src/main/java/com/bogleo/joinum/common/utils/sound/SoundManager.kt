package com.bogleo.joinum.common.utils.sound

import android.content.Context
import android.media.SoundPool
import android.util.Log
import androidx.annotation.RawRes
import com.bogleo.joinum.R
import com.bogleo.joinum.common.utils.SharedPrefs
import dagger.hilt.android.qualifiers.ApplicationContext
import javax.inject.Inject
import javax.inject.Singleton
import kotlin.reflect.full.createInstance

sealed class SoundItem(@RawRes val resId: Int, val volume: Float, val repetitions: Int) {
    class Combine(volume: Float = 1f, repetitions: Int = 0) : SoundItem(resId = R.raw.combine, volume = volume, repetitions = repetitions)
    class SetPoint(volume: Float = 1f, repetitions: Int = 0) : SoundItem(resId = R.raw.set, volume = volume, repetitions = repetitions)
    class Click(volume: Float = 1f, repetitions: Int = 0) : SoundItem(resId = R.raw.click, volume = volume, repetitions = repetitions)
    class Close(volume: Float = 1f, repetitions: Int = 0) : SoundItem(resId = R.raw.close, volume = volume, repetitions = repetitions)
}

@Singleton
class SoundManager @Inject constructor(
    @ApplicationContext private val context: Context,
    private val sharedPrefs: SharedPrefs,
    private val soundPool: SoundPool,
)  {

    private val mSounds: MutableMap<Int, Int> = mutableMapOf()

    init {
        // Get all sounds
        val sounds: List<SoundItem> = SoundItem::class.sealedSubclasses
            .map { clazz -> clazz.createInstance() }

        // Load sounds in pool and save Ids to Map
        sounds.forEach { soundItem: SoundItem ->
            mSounds[soundItem.resId] = soundPool.load(
                context,
                soundItem.resId,
                1
            )
        }
    }

    fun play(soundItem: SoundItem) {
        if(sharedPrefs.allowSound) {
            soundPool.play(
                mSounds[soundItem.resId] ?: 0,
                soundItem.volume,
                soundItem.volume,
                1,
                soundItem.repetitions,
                1f
            )
        }
    }

    fun release() {
        soundPool.release()
    }
}