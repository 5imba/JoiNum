package com.bogleo.joinum.common.game.core

data class Vector2(
    val x: Float,
    val y: Float
) {

    constructor(x: Int, y: Int) : this(
        x = x.toFloat(),
        y = y.toFloat()
    )

    operator fun minus(v: Vector2) = Vector2(x - v.x, y - v.y)
    operator fun times(v: Vector2) = Vector2(x * v.x, y * v.y)
    operator fun times(f: Float) = Vector2(x * f, y * f)



    companion object {
        val zero: Vector2 get() = Vector2(0f, 0f)
        val one: Vector2 get() = Vector2(1f, 1f)

        fun lerp(start: Vector2, stop: Vector2, amount: Float): Vector2 {
            return Vector2(
                com.google.android.material.math.MathUtils.lerp(start.x, stop.x, amount),
                com.google.android.material.math.MathUtils.lerp(start.y, stop.y, amount)
            )
        }
    }

}