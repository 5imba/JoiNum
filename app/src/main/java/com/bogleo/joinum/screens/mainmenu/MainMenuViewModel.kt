package com.bogleo.joinum.screens.mainmenu

import androidx.lifecycle.ViewModel
import com.bogleo.joinum.common.utils.DialogManager
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class MainMenuViewModel @Inject constructor() : ViewModel() {

    @Inject
    lateinit var dialogManager: DialogManager


}