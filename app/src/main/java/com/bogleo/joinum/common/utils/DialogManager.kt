package com.bogleo.joinum.common.utils

import android.content.Context
import android.graphics.Point
import android.os.Build
import android.util.Log
import android.view.View
import android.view.WindowManager
import android.view.animation.AnticipateOvershootInterpolator
import android.view.animation.LinearInterpolator
import android.view.animation.OvershootInterpolator
import com.bogleo.joinum.common.extensions.setAllEnabled
import com.bogleo.joinum.common.utils.sound.SoundItem
import com.bogleo.joinum.common.utils.sound.SoundManager
import com.bogleo.joinum.screens.mainmenu.MainMenuFragment
import javax.inject.Inject

class DialogManager @Inject constructor(
    private val soundManager: SoundManager
) {

    private val dialogStack: MutableList<View> = mutableListOf()
    private val duration = 350L
    private val translationTension = 0.5f

    @Suppress("DEPRECATION")
    fun showDialog(
        dialog: View,
        mainLayer: View,
        fadeLayer: View? = null,
        hideable: Boolean = true,
        onMainLayerCallback: (() -> Unit)? = null
    ) {
        soundManager.play(SoundItem.Click())
        val prevDialog = dialogStack.lastOrNull()

        if(prevDialog != null) {
            prevDialog.setAllEnabled(enabled = false)
            prevDialog.animate()
                .scaleX(0f)
                .scaleY(0f)
                .alpha(0f)
                .setInterpolator(AnticipateOvershootInterpolator())
                .setDuration(duration)
                .start()
        } else {
            fadeLayer?.visibility = View.VISIBLE
            mainLayer.setAllEnabled(enabled = false)
            onMainLayerCallback?.let { it() }
        }

        val wm = dialog.context.getSystemService(Context.WINDOW_SERVICE) as WindowManager
        val screenHeight = if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
            wm.currentWindowMetrics.bounds.height()
        } else {
            val size = Point()
            wm.defaultDisplay.getSize(size)
            size.y
        }
        val startPosY = dialog.height + screenHeight

        dialog.translationY = startPosY.toFloat()
        dialog.visibility = View.VISIBLE
        dialog.animate()
            .translationY(0f)
            .setInterpolator(OvershootInterpolator(translationTension))
            .setDuration(duration)
            .start()
        if(hideable) {
            dialogStack.add(dialog)
        }
    }

    @Suppress("DEPRECATION")
    fun closeLastDialog(
        mainLayer: View,
        fadeLayer: View? = null,
        onMainLayerCallback: (() -> Unit)? = null
    ): Boolean {
        soundManager.play(SoundItem.Close())
        val currentDialog = dialogStack.lastOrNull()

        return if(currentDialog != null) {
            val wm = currentDialog.context.getSystemService(Context.WINDOW_SERVICE) as WindowManager
            val screenHeight = if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
                wm.currentWindowMetrics.bounds.height()
            } else {
                val size = Point()
                wm.defaultDisplay.getSize(size)
                size.y
            }
            val startPosY = currentDialog.height + screenHeight

            currentDialog.translationY = 0f
            currentDialog.animate()
                .translationY(startPosY.toFloat())
                .setInterpolator(LinearInterpolator())
                .setDuration(duration)
                .withEndAction {
                    currentDialog.visibility = View.GONE
                }
                .start()
            dialogStack.remove(currentDialog)

            val prevDialog = dialogStack.lastOrNull()
            if(prevDialog != null) {
                prevDialog.animate()
                    .scaleX(1f)
                    .scaleY(1f)
                    .alpha(1f)
                    .setInterpolator(OvershootInterpolator())
                    .setDuration(duration)
                    .withEndAction {
                        prevDialog.setAllEnabled(enabled = true)
                    }
                    .start()
            } else {
                fadeLayer?.visibility = View.GONE
                mainLayer.setAllEnabled(enabled = true)
                onMainLayerCallback?.let { it() }
            }
            true
        } else {
            false
        }
    }

    fun clearAllAndShowDialog(
        dialog: View,
        mainLayer: View,
        fadeLayer: View? = null,
        hideable: Boolean = true,
        onMainLayerCallback: (() -> Unit)? = null
    ) {
        clearDialogStack()
        showDialog(dialog, mainLayer, fadeLayer, hideable, onMainLayerCallback)
    }

    private fun clearDialogStack() {
        dialogStack.forEach { dialog ->
            dialog.visibility = View.GONE
        }
        dialogStack.clear()
    }
}