package com.bogleo.joinum.common.utils

import android.util.Log
import android.view.View
import android.widget.AdapterView
import android.widget.ArrayAdapter
import android.widget.Spinner
import androidx.appcompat.app.AppCompatDelegate
import com.bogleo.joinum.R
import com.bogleo.joinum.common.utils.sound.SoundItem
import com.bogleo.joinum.common.utils.sound.SoundManager
import javax.inject.Inject

class SpinnerHelper @Inject constructor(
    private val sharedPrefs: SharedPrefs,
    private val soundManager: SoundManager
) {

    fun setup(spinner: Spinner) {
        ArrayAdapter.createFromResource(
            spinner.context,
            R.array.theme_modes_array,
            R.layout.item_spinner_txt
        ).also { adapter ->
            // Specify the layout to use when the list of choices appears
            adapter.setDropDownViewResource(R.layout.item_spinner_drop_txt)
            // Apply the adapter to the spinner
            spinner.adapter = adapter
            spinner.setSelection(sharedPrefs.themeMode, false)
        }
        spinner.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
            override fun onItemSelected(parent: AdapterView<*>?, view: View?, position: Int, id: Long) {
                soundManager.play(SoundItem.Click())
                val currentNightMode = AppCompatDelegate.getDefaultNightMode()
                if(position != currentNightMode) {
                    AppCompatDelegate.setDefaultNightMode(position)
                    sharedPrefs.themeMode = position
                }
            }
            override fun onNothingSelected(parent: AdapterView<*>?) { }
        }
    }
}