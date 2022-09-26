package com.bogleo.joinum.screens

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import androidx.activity.viewModels
import androidx.core.view.WindowCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.WindowInsetsControllerCompat
import com.bogleo.joinum.R
import com.bogleo.joinum.databinding.ActivityMainBinding
import com.bogleo.joinum.screens.game.GameFragment
import com.bogleo.joinum.screens.mainmenu.MainMenuFragment
import dagger.hilt.android.AndroidEntryPoint

@AndroidEntryPoint
class MainActivity : AppCompatActivity() {

    lateinit var binding: ActivityMainBinding

    private val viewModel: MainViewModel by viewModels()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)
        setTransparentStatusBar()

        // Screens navigation logic
        viewModel.navigateToFragment.observe(this) {
            it?.getContentIfNotHandled()?.let { fragmentRequest ->
                val transaction = supportFragmentManager.beginTransaction()
                transaction.replace(
                    R.id.fragment_container, fragmentRequest.fragment, fragmentRequest.tag
                )
                if (fragmentRequest.backStack) transaction.addToBackStack(null)
                transaction.commit()
            }
        }
        savedInstanceState
            // Restore fragment from bundle
            ?.let { bundle ->
                when(bundle.getString(CURRENT_FRAGMENT)) {
                    MainMenuFragment.TAG -> viewModel.openMainMenu()
                    GameFragment.TAG -> viewModel.openResumeGame()
                }
            }
            // Open MainMenu if no saved
            ?: viewModel.openMainMenu()
    }

    override fun onSaveInstanceState(outState: Bundle) {
        // Save current fragment to bundle
        viewModel.currentFragmentTag?.let { tag ->
            outState.putString(CURRENT_FRAGMENT, tag)
        }
        super.onSaveInstanceState(outState)
    }

    private fun setTransparentStatusBar() {
        WindowCompat.setDecorFitsSystemWindows(window, false)
        WindowInsetsControllerCompat(window, binding.root).let { controller ->
            controller.hide(WindowInsetsCompat.Type.systemBars())
            controller.systemBarsBehavior = WindowInsetsControllerCompat
                .BEHAVIOR_SHOW_TRANSIENT_BARS_BY_SWIPE
        }
    }
}

private const val CURRENT_FRAGMENT = "CURRENT_FRAGMENT"