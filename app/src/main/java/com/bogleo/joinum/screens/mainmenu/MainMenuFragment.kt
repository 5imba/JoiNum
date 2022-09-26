package com.bogleo.joinum.screens.mainmenu

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.activity.OnBackPressedCallback
import androidx.fragment.app.Fragment
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import com.bogleo.joinum.common.game.data.GameMode
import com.bogleo.joinum.databinding.FragmentMainMenuBinding
import com.bogleo.joinum.screens.MainViewModel
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class MainMenuFragment : Fragment() {

    private var _binding: FragmentMainMenuBinding? = null
    private val binding get() = _binding!!

    private val mainViewModel: MainViewModel by activityViewModels()
    private val viewModel: MainMenuViewModel by viewModels()

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?,
    ): View? {
        // Inflate the layout for this fragment
        _binding = FragmentMainMenuBinding.inflate(inflater, container, false)
        return binding.root
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        with(binding) {
            // Show resume button if [isResumeSessionAvailable]
            if(mainViewModel.isResumeSessionAvailable) {
                btnResume.visibility = View.VISIBLE
                btnResume.setOnClickListener {
                    mainViewModel.openResumeGame()
                }
            }
            // Show difficulty popup
            btnNewGame.setOnClickListener {
                viewModel.dialogManager.showDialog(
                    dialog = containerDifficulty,
                    mainLayer = containerMain
                )
            }
            // Close difficulty popup
            btnCloseDifficulty.setOnClickListener {
                viewModel.dialogManager.closeLastDialog(
                    mainLayer = containerMain
                )
            }
            // Start New Game with selected difficulty
            btnEasy.setOnClickListener {
                mainViewModel.openNewGame(GameMode.fromDifficulty(difficulty = 3))
            }
            btnNormal.setOnClickListener {
                mainViewModel.openNewGame(GameMode.fromDifficulty(difficulty = 4))
            }
            btnHard.setOnClickListener {
                mainViewModel.openNewGame(GameMode.fromDifficulty(difficulty = 5))
            }
        }
        // Add onBackPressed callback
        requireActivity().onBackPressedDispatcher.addCallback(
            viewLifecycleOwner,
            onBackPressedCallback
        )
    }

    override fun onDestroy() {
        super.onDestroy()
        _binding = null
    }

    private val onBackPressedCallback= object : OnBackPressedCallback(true) {
        override fun handleOnBackPressed() {
            if(!viewModel.dialogManager.closeLastDialog(binding.containerMain)) {
                isEnabled = false
                requireActivity().onBackPressed()
                isEnabled = true
            }
        }
    }

    companion object {
        const val TAG = "MainMenuFragment"
    }

}