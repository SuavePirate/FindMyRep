package com.example.findmyrep.models.voicify

import java.util.*

data class VoicifyDevice(val id: String,
                         val name: String,
                         val supportsVideo: Boolean,
                         val supportsForegroundImage: Boolean,
                         val supportsBackgroundImage: Boolean,
                         val supportsAudio: Boolean,
                         val supportsSsml: Boolean,
                         val supportsDisplayText: Boolean,
                         val supportsVoiceInput: Boolean,
                         val supportsTextInput: Boolean);


data class VoicifyUser(val id: String, val name: String)

data class VoicifyRequestContext(val sessionId: String,
                                val noTracking: Boolean,
                                val requestType: String,
                                var requestName: String?,
                                val originalInput: String,
                                val channel: String,
                                var requiresLanguageUnderstanding: Boolean,
                                val locale: String,
                                var slots: Map<String, String>?)

data class VoicifyCustomAssistantRequest(val requestId: String,
                                        val context: VoicifyRequestContext,
                                        val device: VoicifyDevice,
                                        val user: VoicifyUser)


data class VoicifyCustomAssistantResponse(val ssml: String,
                                        val outputSpeech: String,
                                        val displayText: String,
                                        val responseTemplate: String,
                                        val endSession: Boolean
                                        )