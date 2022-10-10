package com.bogleo.joinum.screens.mainmenu

import androidx.lifecycle.ViewModel
import com.bogleo.joinum.common.utils.DialogManager
import com.bogleo.joinum.common.utils.sound.SoundManager
import com.bogleo.joinum.common.utils.SpinnerHelper
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class MainMenuViewModel @Inject constructor() : ViewModel() {

    @Inject
    lateinit var dialogManager: DialogManager

    @Inject
    lateinit var spinnerHelper: SpinnerHelper
}