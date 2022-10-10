package com.bogleo.joinum.screens

import android.se.omapi.Session
import android.util.Log
import androidx.appcompat.app.AppCompatDelegate
import androidx.fragment.app.Fragment
import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.bogleo.joinum.common.game.data.GameMode
import com.bogleo.joinum.common.utils.Event
import com.bogleo.joinum.common.utils.SharedPrefs
import com.bogleo.joinum.common.utils.sound.SoundItem
import com.bogleo.joinum.common.utils.sound.SoundManager
import com.bogleo.joinum.screens.game.GameFragment
import com.bogleo.joinum.screens.mainmenu.MainMenuFragment
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class MainViewModel @Inject constructor() : ViewModel() {

    @Inject
    lateinit var sharedPrefs: SharedPrefs

    @Inject
    lateinit var soundManager: SoundManager

    private val _navigateToFragment = MutableLiveData<Event<FragmentNavigationRequest>>()
    val navigateToFragment: LiveData<Event<FragmentNavigationRequest>> get() = _navigateToFragment

    val isResumeSessionAvailable: Boolean get() = sharedPrefs.gameMode.session == GameMode.SESSION_RESUME

    var currentFragmentTag: String? = null
        private set

    private fun showFragment(fragment: Fragment, backStack: Boolean = true, tag: String? = null) {
        _navigateToFragment.value = Event(FragmentNavigationRequest(fragment, backStack, tag))
        currentFragmentTag = fragment.tag
    }

    fun openMainMenu() {
        soundManager.play(SoundItem.Click())
        showFragment(fragment = MainMenuFragment(), backStack = false, MainMenuFragment.TAG)
    }

    fun openNewGame(gameMode: GameMode, addToBackStack: Boolean = false) {
        soundManager.play(SoundItem.Click())
        sharedPrefs.gameMode = gameMode.copy(session = GameMode.SESSION_NEW)
        showFragment(fragment = GameFragment(), backStack = addToBackStack, GameFragment.TAG)
    }

    fun openResumeGame(addToBackStack: Boolean = false) {
        soundManager.play(SoundItem.Click())
        sharedPrefs.gameMode = GameMode(
            fieldCellsCount = 0,
            inputCellsCount = 0,
            colorsCount = 0,
            session = GameMode.SESSION_RESUME
        )
        showFragment(fragment = GameFragment(), backStack = addToBackStack, GameFragment.TAG)
    }

    fun setThemeMode() {
        val currentNightMode = AppCompatDelegate.getDefaultNightMode()
        val savedNightMode = sharedPrefs.themeMode
        if(savedNightMode != currentNightMode) {
            AppCompatDelegate.setDefaultNightMode(savedNightMode)
        }
    }

    fun toggleSounds(allowSounds: Boolean) {
        sharedPrefs.allowSound = allowSounds
        soundManager.play(SoundItem.Click())
    }

    override fun onCleared() {
        super.onCleared()
        soundManager.release()
    }
}

data class FragmentNavigationRequest(
    val fragment: Fragment,
    val backStack: Boolean = false,
    val tag: String? = null
)