package com.bogleo.joinum.screens.stats

import android.content.Context
import androidx.lifecycle.ViewModel
import com.bogleo.joinum.R
import com.bogleo.joinum.common.utils.SharedPrefs
import com.bogleo.joinum.data.StatisticContentItem
import com.bogleo.joinum.data.StatisticItem
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class StatsViewModel @Inject constructor() : ViewModel() {

    @Inject
    lateinit var sharedPrefs: SharedPrefs

    fun getStatistics(context: Context): List<StatisticItem> {
        val difficultArray = context.resources.getStringArray(R.array.stats_blocks_array)
        val stats: MutableList<StatisticItem> = mutableListOf()

        difficultArray.forEachIndexed { index, str ->
            // TODO fix difficult mapping
            val difficult = if(index == 0) 0 else index + 2
            stats.add(
                StatisticItem(
                    str,
                    listOf(
                        StatisticContentItem(
                            context.getString(R.string.best_score),
                            sharedPrefs.getBestScore(difficult).toString()
                        ),
                        StatisticContentItem(
                            context.getString(R.string.max_tier),
                            sharedPrefs.getMaxTier(difficult).toString()
                        ),
                        StatisticContentItem(
                            context.getString(R.string.finished_games),
                            sharedPrefs.getFinishedGames(difficult).toString()
                        ),
                        StatisticContentItem(
                            context.getString(R.string.most_moves),
                            sharedPrefs.getMostMoves(difficult).toString()
                        )
                    )
                )
            )
        }
        return stats
    }

}