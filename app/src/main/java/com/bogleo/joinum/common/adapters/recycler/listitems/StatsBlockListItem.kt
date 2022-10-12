package com.bogleo.joinum.common.adapters.recycler.listitems

import android.view.LayoutInflater
import android.view.ViewGroup
import androidx.recyclerview.widget.DiffUtil
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.ListAdapter
import androidx.recyclerview.widget.RecyclerView
import com.bogleo.joinum.R
import com.bogleo.joinum.common.adapters.recycler.BaseViewHolder
import com.bogleo.joinum.common.adapters.recycler.ListItem
import com.bogleo.joinum.common.adapters.recycler.ListItemAdapter
import com.bogleo.joinum.data.Item
import com.bogleo.joinum.data.StatisticItem
import com.bogleo.joinum.databinding.ItemStatsBlockBinding

class StatsBlockListItem(
    private val listItems: List<ListItem<*, *>>
) : ListItem<ItemStatsBlockBinding, StatisticItem> {

    override fun isRelativeItem(item: Item) = item is StatisticItem

    override fun getLayoutId(): Int = R.layout.item_stats_block

    override fun getViewHolder(
        layoutInflater: LayoutInflater,
        parent: ViewGroup,
    ): BaseViewHolder<ItemStatsBlockBinding, StatisticItem> {
        val binding = ItemStatsBlockBinding.inflate(layoutInflater, parent, false)
        return StatsBlockViewHolder(binding = binding, listItems = listItems)
    }

    override fun getDiffUtil() = diffUtil

    private val diffUtil = object : DiffUtil.ItemCallback<StatisticItem>() {

        override fun areItemsTheSame(oldItem: StatisticItem, newItem: StatisticItem) = oldItem.title == newItem.title

        override fun areContentsTheSame(oldItem: StatisticItem, newItem: StatisticItem) = oldItem == newItem
    }
}

class StatsBlockViewHolder(
    binding: ItemStatsBlockBinding,
    listItems: List<ListItem<*, *>>
) : BaseViewHolder<ItemStatsBlockBinding, StatisticItem>(binding = binding) {

    private val listItemAdapter = ListItemAdapter(listItems)

    init {
        with(binding.recyclerStatsContent) {
            adapter = listItemAdapter
            layoutManager = LinearLayoutManager(context, RecyclerView.VERTICAL, false)
        }
    }

    override fun onBind(item: StatisticItem) {
        super.onBind(item)
        with(binding) {
            txtTitle.text = item.title
            listItemAdapter.submitList(item.content)
        }
    }
}