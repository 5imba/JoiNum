package com.bogleo.joinum.screens.game

import android.annotation.SuppressLint
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.activity.OnBackPressedCallback
import androidx.fragment.app.activityViewModels
import androidx.fragment.app.viewModels
import com.bogleo.joinum.R
import com.bogleo.joinum.common.game.GameLogic
import com.bogleo.joinum.common.game.data.GameData
import com.bogleo.joinum.common.game.data.GameMode
import com.bogleo.joinum.databinding.FragmentGameBinding
import com.bogleo.joinum.screens.MainViewModel
import dagger.hilt.android.AndroidEntryPoint
import javax.inject.Inject

@AndroidEntryPoint
class GameFragment : Fragment() {

    @Inject
    lateinit var gameLogic: GameLogic

    @Inject
    lateinit var gameData: GameData

    private var _binding: FragmentGameBinding? = null
    private val binding get() = _binding!!

    private val mainViewModel: MainViewModel by activityViewModels()
    private val viewModel: GameViewModel by viewModels()

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?,
    ): View? {
        // Inflate the layout for this fragment
        _binding = FragmentGameBinding.inflate(inflater, container, false)
        return binding.root
    }

    @SuppressLint("SetTextI18n")
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        with(binding) {
            // Initialize Game Logic (Main Game Enter Point)
            gameLogic.init(gameView = gameView, gameData = gameData)

            // Score in game
            gameLogic.score.observe(viewLifecycleOwner) { scoreText: String ->
                txtScore.text = scoreText
            }

            // Game Over
            val scorePrefix = requireContext().getString(R.string.game_over_score)
            val bestScorePrefix = requireContext().getString(R.string.game_over_best_score)
            gameLogic.setOnGameOverListener { score, bestScore ->
                txtScoreGameOver.text = "$scorePrefix $score"
                if(bestScore > 0) {
                    txtBestScoreGameOver.visibility = View.VISIBLE
                    txtBestScoreGameOver.text = "$bestScorePrefix $bestScore"
                }
                viewModel.dialogManager.clearAllAndShowDialog(
                    dialog = containerGameOver,
                    mainLayer = containerGame,
                    hideable = false
                ) {
                    binding.gameView.isPaused = true
                }
            }

            // Bind Pause dialog buttons
            btnPause.setOnClickListener { showDialog(containerPause) }
            btnClosePause.setOnClickListener { closeCurrentDialog() }
            btnResumeGame.setOnClickListener { closeCurrentDialog() }
            btnNewGameGame.setOnClickListener { showDialog(containerDifficulty) }
            btnMainMenuGame.setOnClickListener { mainViewModel.openMainMenu() }

            // Bind GameOver dialog buttons
            btnNewGameGameOver.setOnClickListener { showDialog(containerDifficulty) }
            btnMainMenuGameOver.setOnClickListener { mainViewModel.openMainMenu() }

            // Bind Difficulty dialog buttons
            btnCloseDifficulty.setOnClickListener { closeCurrentDialog() }
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

    private fun showDialog(dialog: View) {
        viewModel.dialogManager.showDialog(
            dialog = dialog,
            mainLayer = binding.containerGame
        ) {
            binding.gameView.isPaused = true
        }
    }

    private fun closeCurrentDialog() = viewModel.dialogManager
        .closeLastDialog(
            mainLayer = binding.containerGame
        ) {
            binding.gameView.isPaused = false
        }

    private val onBackPressedCallback = object : OnBackPressedCallback(true) {
        override fun handleOnBackPressed() {
            if(!closeCurrentDialog()) {
                isEnabled = false
                requireActivity().onBackPressed()
                isEnabled = true
            }
        }
    }

    companion object {
        const val TAG = "GameFragment"
    }

}