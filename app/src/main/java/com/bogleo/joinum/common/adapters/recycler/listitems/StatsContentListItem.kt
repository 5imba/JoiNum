package com.bogleo.joinum.common.adapters.recycler.listitems

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.DiffUtil
import com.bogleo.joinum.R
import com.bogleo.joinum.common.adapters.recycler.BaseViewHolder
import com.bogleo.joinum.common.adapters.recycler.ListItem
import com.bogleo.joinum.data.Item
import com.bogleo.joinum.data.StatisticContentItem
import com.bogleo.joinum.databinding.ItemStatsContentBinding

class StatsContentListItem : ListItem<ItemStatsContentBinding, StatisticContentItem> {

    override fun isRelativeItem(item: Item) = item is StatisticContentItem

    override fun getLayoutId(): Int = R.layout.item_stats_content

    override fun getViewHolder(
        layoutInflater: LayoutInflater,
        parent: ViewGroup,
    ): BaseViewHolder<ItemStatsContentBinding, StatisticContentItem> {
        val binding = ItemStatsContentBinding.inflate(layoutInflater, parent, false)
        return StatsContentViewHolder(binding = binding)
    }

    override fun getDiffUtil() = diffUtil

    private val diffUtil = object : DiffUtil.ItemCallback<StatisticContentItem>() {

        override fun areItemsTheSame(oldItem: StatisticContentItem, newItem: StatisticContentItem) = oldItem.title == newItem.title

        override fun areContentsTheSame(oldItem: StatisticContentItem, newItem: StatisticContentItem) = oldItem == newItem
    }
}

class StatsContentViewHolder(
    binding: ItemStatsContentBinding
) : BaseViewHolder<ItemStatsContentBinding, StatisticContentItem>(binding = binding) {

    override fun onBind(item: StatisticContentItem) {
        super.onBind(item)
        with(binding) {
            txtTitle.text = item.title
            txtValue.text = item.value
        }
    }
}