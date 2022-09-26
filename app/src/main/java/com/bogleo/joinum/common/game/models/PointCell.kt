package com.bogleo.joinum.common.game.models

import android.animation.ValueAnimator
import android.graphics.*
import androidx.core.animation.doOnEnd
import com.bogleo.joinum.common.game.core.GameObject
import com.bogleo.joinum.common.game.data.Index
import com.bogleo.joinum.common.game.core.Vector2
import java.lang.Float.min

class PointCell : GameObject() {

    private val scalingDuration = 100L
    private val movingDuration = 300L

    var tier: Int = 0
        set(value) {
            field = value
            isHovered = false
        }
    var color: Int = 0
        set(value) {
            field = value
            circlePaint.color = value
        }

    var data: PointCellData
        get() = PointCellData(
            tier = tier,
            color = color
        )
        set(value) {
            tier = value.tier
            color = value.color
        }

    var index: Index = Index(-1, -1)

    private val circlePaint: Paint = Paint(Paint.ANTI_ALIAS_FLAG).apply {
        style = Paint.Style.FILL
    }

    private val tierPaint: Paint = Paint(Paint.ANTI_ALIAS_FLAG).apply {
        textAlign = Paint.Align.CENTER
        color = Color.BLACK
    }

    private val tierBounds: Rect = Rect()

    override fun draw(canvas: Canvas) {
        // Draw circle
        canvas.drawCircle(
            transform.position.x,
            transform.position.y,
            transform.scaledHalfSize.x,
            circlePaint
        )

        if(tier > 0) {
            val text = data.tier.toString()
            val padding = transform.scaledSize.x * 0.2f
            val textSize = transform.scaledSize.x - padding * 2
            val testTextSize = 90f

            // Calculate text bounds
            tierPaint.textSize = testTextSize
            tierPaint.getTextBounds(text, 0, text.length, tierBounds)

            // Calculate desired size as proportion of testTextSize
            val textScale = testTextSize * textSize
            val desiredTextSize = min(
                textScale / tierBounds.width(),
                textScale / tierBounds.height()
            )

            // Calculate Y offset
            tierPaint.textSize = desiredTextSize
            tierPaint.getTextBounds(text, 0, text.length, tierBounds)
            val offsetY = tierBounds.height() * 0.5f

            // Draw tier text
            canvas.drawText(
                text,
                transform.position.x,
                transform.position.y + offsetY,
                tierPaint
            )
        }
    }

    private val animator = ValueAnimator()
    private var isHovered = false

    fun onHoverEnter(updateListener: (() -> Unit)? = null) {
        if(tier == 0 && !isHovered) {
            isHovered = true
            startScale(
                SCALE_BIG,
                updateListener
            )
        }
    }

    fun onHoverExit(updateListener: (() -> Unit)? = null) {
        if(tier == 0 && isHovered) {
            isHovered = false
            startScale(
                SCALE_SMALL,
                updateListener
            )
        }
    }

    fun startScale(scaleSize: Float, updateListener: (() -> Unit)? = null) {
        startFloatAnimation(
            transform.scale.x,
            scaleSize,
            updateListener
        )
    }

    fun moveTo(target: Vector2, updateListener: (() -> Unit)? = null, doOnEnd: (() -> Unit)? = null) {
        moveTo(listOf(target), updateListener, doOnEnd)
    }

    fun moveTo(targets: List<Vector2>, updateListener: (() -> Unit)? = null, doOnEnd: (() -> Unit)? = null) {
        startVectorAnimation(transform.position, targets, updateListener, doOnEnd)
    }

    fun setupTierPaint(color: Int, typeface: Typeface) {
        tierPaint.apply {
            this.color = color
            this.typeface = typeface
        }
    }

    private fun startFloatAnimation(start: Float, stop: Float, updateListener: (() -> Unit)? = null) {
        with(animator) {
            removeAllListeners()
            duration = scalingDuration
            setFloatValues(start, stop)
            addUpdateListener {
                val value = it.animatedValue as Float
                transform.scale = Vector2(value, value)
                updateListener?.let { onUpdate -> onUpdate() }
            }
            start()
        }
    }

    private fun startVectorAnimation(
        start: Vector2,
        targets: List<Vector2>,
        updateListener: (() -> Unit)? = null,
        doOnEnd: (() -> Unit)? = null,
    ) {
        var index = targets.size - 1
        var counter = 1
        val q = 1f / targets.size
        var startPos = start

        with(animator) {
            removeAllListeners()
            duration = movingDuration
            setFloatValues(0f, 1f)
            addUpdateListener {
                val value = it.animatedValue as Float
                if (targets.size > 1) {
                    if (value >= q * counter) {
                        startPos = targets[index]
                        counter += 1
                        index -= 1
                    } else {
                        val qt: Float = remap(value % q, 0f, q, 0f, 1f)
                        transform.position = Vector2.lerp(startPos, targets[index], qt)
                    }
                } else {
                    transform.position = Vector2.lerp(startPos, targets[0], value)
                }
                updateListener?.let { onUpdate -> onUpdate() }
            }
            doOnEnd { doOnEnd?.let { onEnd -> onEnd() } }
            start()
        }
    }

    private fun remap(value: Float, from1: Float, to1: Float, from2: Float, to2: Float): Float {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2
    }

    companion object {
        const val SCALE_SMALL = 0.15f
        const val SCALE_BIG = 0.7f
    }

}

fun PointCell.isHovered(x: Float, y: Float): Boolean = isHovered(x.toInt(), y.toInt())
fun PointCell.isHovered(x: Int, y: Int): Boolean = transform.collider.contains(x, y)