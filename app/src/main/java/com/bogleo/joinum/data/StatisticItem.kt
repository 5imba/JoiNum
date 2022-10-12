package com.bogleo.joinum.data

data class StatisticItem(
    val title: String,
    val content: List<StatisticContentItem>
) : Item