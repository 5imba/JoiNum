package com.bogleo.joinum.common.utils.sound

import android.media.SoundPool
import androidx.annotation.RawRes


sealed interface SoundTaskAction {
    object Play : SoundTaskAction
    object Load : SoundTaskAction
    object Unload : SoundTaskAction
    object Cancel : SoundTaskAction
}

data class SoundTask(
    @RawRes val resId: Int,
    val repetitions: Int,
    val volume: Float,
    val action: SoundTaskAction
) {

    val isPlay get() = action is SoundTaskAction.Play
    val isLoad get() = action is SoundTaskAction.Load
    val isUnload get() = action is SoundTaskAction.Unload
    val isCancel get() = action is SoundTaskAction.Cancel

    companion object {
        fun load(@RawRes resId: Int) = SoundTask(
            resId = resId,
            repetitions = 0,
            volume = 0f,
            action = SoundTaskAction.Load
        )

        fun play(@RawRes resId: Int, repetitions: Int, volume: Float) = SoundTask(
            resId = resId,
            repetitions = repetitions,
            volume = volume,
            action = SoundTaskAction.Play
        )

        fun unload(@RawRes resId: Int) = SoundTask(
            resId = resId,
            repetitions = 0,
            volume = 0f,
            action = SoundTaskAction.Unload
        )

        fun cancel() = SoundTask(
            resId = 0,
            repetitions = 0,
            volume = 0f,
            action = SoundTaskAction.Cancel
        )
    }
}