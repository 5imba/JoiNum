package com.bogleo.joinum.screens.game

import androidx.lifecycle.ViewModel
import com.bogleo.joinum.common.utils.DialogManager
import dagger.hilt.android.lifecycle.HiltViewModel
import javax.inject.Inject

@HiltViewModel
class GameViewModel @Inject constructor() : ViewModel() {

    @Inject
    lateinit var dialogManager: DialogManager

}