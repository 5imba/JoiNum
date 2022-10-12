package com.bogleo.joinum.screens.stats

import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.viewModels
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.bogleo.joinum.R
import com.bogleo.joinum.common.adapters.recycler.ListItem
import com.bogleo.joinum.common.adapters.recycler.ListItemAdapter
import com.bogleo.joinum.common.adapters.recycler.listitems.StatsBlockListItem
import com.bogleo.joinum.common.adapters.recycler.listitems.StatsContentListItem
import com.bogleo.joinum.data.StatisticItem
import com.bogleo.joinum.databinding.FragmentStatsBinding
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class StatsFragment : Fragment() {

    private var _binding: FragmentStatsBinding? = null
    private val binding get() = _binding!!

    private val viewModel: StatsViewModel by viewModels()

    private val listItemAdapter = ListItemAdapter(
        listOf(
            StatsBlockListItem(
                listOf(StatsContentListItem())
            )
        )
    )

    companion object {
        const val TAG = "StatsFragment"
    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?,
    ): View? {
        // Inflate the layout for this fragment
        _binding = FragmentStatsBinding.inflate(inflater, container, false)
        return _binding?.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        with(binding) {

            with(recyclerStats) {
                adapter = listItemAdapter
                layoutManager = LinearLayoutManager(
                    requireContext(), RecyclerView.VERTICAL, false)
                listItemAdapter.submitList(viewModel.getStatistics(requireContext()))
            }

            btnBack.setOnClickListener {
                requireActivity().onBackPressed()
            }
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        _binding = null
    }

    private fun test(s: String): String {
        return  s
    }

}