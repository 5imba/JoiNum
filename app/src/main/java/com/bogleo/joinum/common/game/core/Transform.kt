package com.bogleo.joinum.common.game.core

import android.graphics.Rect

class Transform {

    var position: Vector2 = Vector2.zero

    var size: Vector2 = Vector2.zero
        set(value) {
            field = value
            halfSize = value * 0.5f
            scaledSize = value * scale
        }

    var halfSize: Vector2 = Vector2.zero
        private set

    var scale: Vector2 = Vector2.one
        set(value) {
            field = value
            scaledSize = value * size
        }

    var scaledSize: Vector2 = Vector2.zero
        private set(value) {
            field = value
            scaledHalfSize = value * 0.5f
        }

    var scaledHalfSize: Vector2 = Vector2.zero
        private set

    val collider: Rect = Rect()

    fun setCollider(size: Vector2) {
        val halfColliderSize = size * 0.5f
        collider.left   = (position.x - halfColliderSize.x).toInt()
        collider.top    = (position.y - halfColliderSize.y).toInt()
        collider.right  = (position.x + halfColliderSize.x).toInt()
        collider.bottom = (position.y + halfColliderSize.y).toInt()
    }
}