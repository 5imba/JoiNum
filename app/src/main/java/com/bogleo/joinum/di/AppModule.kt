package com.bogleo.joinum.di

import android.media.AudioAttributes
import android.media.SoundPool
import dagger.Module
import dagger.Provides
import dagger.hilt.InstallIn
import dagger.hilt.components.SingletonComponent

@Module
@InstallIn(SingletonComponent::class)
class AppModule {

    @Provides
    fun provideAudioAttributes(): AudioAttributes = AudioAttributes.Builder()
        .setUsage(AudioAttributes.USAGE_GAME)
        .setContentType(AudioAttributes.CONTENT_TYPE_SONIFICATION)
        //.setFlags(AudioAttributes.FLAG_AUDIBILITY_ENFORCED)
        .build()

    @Provides
    fun provideSoundPool(audioAttributes: AudioAttributes): SoundPool = SoundPool.Builder()
        .setMaxStreams(10)
        .setAudioAttributes(audioAttributes)
        .build()
}