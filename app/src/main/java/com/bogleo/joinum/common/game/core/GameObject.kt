package com.bogleo.joinum.common.game.core

import android.graphics.Canvas

abstract class GameObject {

    val transform: Transform = Transform()

    abstract fun draw(canvas: Canvas)
}