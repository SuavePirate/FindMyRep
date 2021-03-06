package com.example.findmyrep

import android.Manifest
import android.content.Context
import android.content.pm.PackageManager
import android.os.Bundle
import android.util.Log
import androidx.appcompat.app.AppCompatActivity
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import com.example.findmyrep.models.voicify.*
import com.github.kittinunf.fuel.Fuel
import com.github.kittinunf.fuel.gson.jsonBody
import com.github.kittinunf.fuel.gson.responseObject
import com.google.android.material.snackbar.Snackbar
import com.squareup.picasso.OkHttp3Downloader
import com.squareup.picasso.Picasso
import io.spokestack.spokestack.OnSpeechEventListener
import io.spokestack.spokestack.SpeechContext
import io.spokestack.spokestack.SpeechPipeline
import io.spokestack.spokestack.nlu.NLUResult
import io.spokestack.spokestack.nlu.TraceListener
import io.spokestack.spokestack.nlu.tensorflow.TensorflowNLU
import io.spokestack.spokestack.tts.SynthesisRequest
import io.spokestack.spokestack.tts.TTSManager
import io.spokestack.spokestack.util.EventTracer
import kotlinx.android.synthetic.main.activity_main.*
import kotlinx.android.synthetic.main.content_main.*
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.OkHttpClient
import okhttp3.Protocol
import java.io.File
import java.io.FileOutputStream
import java.io.IOException
import java.io.InputStream
import java.util.*


class MainActivity : AppCompatActivity(), OnSpeechEventListener, TraceListener {
    private var pipeline: SpeechPipeline? = null
    private var tts: TTSManager? = null
    private var voicifyUser: VoicifyUser? = null
    private var sessionId: String? = null
    private var client: OkHttpClient? = null
    private var picasso: Picasso? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        setSupportActionBar(toolbar)
        sessionId = UUID.randomUUID().toString()
        checkForModels()
        voicifyUser = VoicifyUser(UUID.randomUUID().toString(), UUID.randomUUID().toString())

        client = OkHttpClient.Builder()
            .protocols(Collections.singletonList(Protocol.HTTP_1_1))
            .build()
        picasso = Picasso.Builder(this)
            .downloader(OkHttp3Downloader(client))
            .build()

        Picasso.setSingletonInstance(picasso!!)


        pipeline = SpeechPipeline.Builder()
            .useProfile("io.spokestack.spokestack.profile.VADTriggerAndroidASR")
            .setAndroidContext(applicationContext)
            .addOnSpeechEventListener(this)
            .build()
        tts = TTSManager.Builder()
            .setTTSServiceClass("io.spokestack.spokestack.tts.SpokestackTTSService")
            .setOutputClass("io.spokestack.spokestack.tts.SpokestackTTSOutput")
            .setProperty("spokestack-id", "3b40def8-f77e-42f8-8d5c-47a8fd7e4797")
            .setProperty(
                "spokestack-secret",
                "9DAC2C5AA917752AB1854B47DE1990560203394AF5716E157EDD57C7DAD99949"
            )
            .setAndroidContext(applicationContext)
            .setLifecycle(lifecycle)
            .build()

        fab.setOnClickListener { view ->
            tts?.stopPlayback()
            when {
                ContextCompat.checkSelfPermission(
                    this,
                    Manifest.permission.RECORD_AUDIO
                ) == PackageManager.PERMISSION_GRANTED -> {
                    // You can use the API that requires the permission.
                    if (pipeline?.isRunning == true) {
                        pipeline?.stop()
                        Snackbar.make(view, "No longer listening", Snackbar.LENGTH_LONG)
                            .setAction("Action", null).show()
                    } else {

                        pipeline?.start()
                        Snackbar.make(view, "Now listening", Snackbar.LENGTH_LONG)
                            .setAction("Action", null).show()
                    }
                }
                else -> {
                    // You can directly ask for the permission
                    ActivityCompat
                        .requestPermissions(
                            this,
                            Array<String>(1) { _ -> Manifest.permission.RECORD_AUDIO },
                            1
                        );
                }
            }

        }
    }

    override fun onRequestPermissionsResult(
        requestCode: Int,
        permissions: Array<out String>,
        grantResults: IntArray
    ) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults)
    }

    override fun onEvent(event: SpeechContext.Event?, context: SpeechContext?) {
        when (event) {
            SpeechContext.Event.ACTIVATE -> println("ACTIVATED")
            SpeechContext.Event.DEACTIVATE -> println("DEACTIVATED")
            SpeechContext.Event.RECOGNIZE -> context?.let { handleSpeech(it.transcript) }
            SpeechContext.Event.TIMEOUT -> println("TIMEOUT")
            SpeechContext.Event.ERROR -> context?.let { println("ERROR: ${it.error}") }
            else -> {
                // do nothing
            }
        }
    }

    private fun handleSpeech(transcript: String) {
        pipeline?.stop()
        val nlu = TensorflowNLU.Builder()
            .setProperty("nlu-model-path", "$cacheDir/nlu.tflite")
            .setProperty("nlu-metadata-path", "$cacheDir/metadata.json")
            .setProperty("wordpiece-vocab-path", "$cacheDir/vocab.txt")
            .addTraceListener(this)
            .build()

        GlobalScope.launch(Dispatchers.Default) {
            nlu?.let {
                val result = it.classify(transcript).get()
                withContext(Dispatchers.Main) {
                    Snackbar.make(fab, result.intent, Snackbar.LENGTH_LONG)
                        .setAction("Action", null).show()

                    val requestModel = getVoicifyRequest(transcript, result)

                    Fuel.post("https://assistant.voicify.com/api/customAssistant/handlerequest?applicationId=9bf40a95-46df-41f5-8eb7-ebeae97f61d5&applicationSecret=NjU1Zjg1N2MtNjljZi00ZmU5LTllMDctMzI4NmI5ZjQ3NDE4\n")
                        .jsonBody(requestModel!!)
                        .responseObject<VoicifyCustomAssistantResponse> { _, _, voicifyResult ->
                            // handle result similarly as it is shown by the core module
                            Log.i("VOICIFY_RESPONSE", voicifyResult.component1()?.ssml)

                            displayText.text = voicifyResult.component1()?.displayText;

                            if (voicifyResult.component1()?.foregroundImage != null) {
                                Picasso.get().load(voicifyResult.component1()?.foregroundImage?.url)
                                    .into(displayImage);
                            }

                            val ttsRequest =
                                SynthesisRequest.Builder(voicifyResult.component1()?.ssml)
                                    .withMode(SynthesisRequest.Mode.SSML).build()
//                            val ttsRequest = SynthesisRequest.Builder("Here's what I found. Elizabeth Warren, and you can call them at a phone number.   Would you like to learn about your governor?").build()
                            tts?.synthesize(ttsRequest)

                        }
                }
            }
        }
    }

    private fun getVoicifyRequest(
        transcript: String,
        result: NLUResult
    ): VoicifyCustomAssistantRequest? {
        var requestModel: VoicifyCustomAssistantRequest? = null;
        val requestContext: VoicifyRequestContext = VoicifyRequestContext(
            sessionId!!,
            false,
            "IntentRequest",
            null,
            transcript,
            "Android App",
            true,
            "en-US",
            null
        )

        if (result.intent == "SenatorZipCodeIntent" || result.intent == "MayorZipCodeIntent" || result.intent == "GovZipCodeIntent" || result.intent == "AllZipCodeIntent") {
            requestContext.requestName = result.intent
            requestContext.slots = result.slots.mapValues { it.value.rawValue.toString() }
            requestContext.requiresLanguageUnderstanding = false
        }

        requestModel = VoicifyCustomAssistantRequest(
            UUID.randomUUID().toString(),
            requestContext,
            VoicifyDevice(
                UUID.randomUUID().toString(),
                "Android Phone",
                true,
                true,
                true,
                true,
                true,
                true,
                true,
                true
            ),
            voicifyUser!!
        );

        return requestModel
    }

    private fun checkForModels() {
        // PREF_NAME and VERSION_KEY are static Strings set at the top of the file;
        // we want PREF_NAME to uniquely refer to our app, and VERSION_KEY to be
        // unique within the app itself
        if (!modelsCached()) {
            decompressModels()
        } else {
            val currentVersionCode = BuildConfig.VERSION_CODE
            val prefs = getSharedPreferences("com.suavepirate.findmyrep", Context.MODE_PRIVATE)
            val savedVersionCode = prefs.getInt("VERSION_KEY", -1)
            if (currentVersionCode != savedVersionCode) {
                decompressModels()

                // Update the shared preferences with the current version code
                prefs.edit().putInt("VERSION_KEY", currentVersionCode).apply()
            }
        }
    }

    private fun modelsCached(): Boolean {
        val nluName = "nlu.tflite"
        val nluFile = File("$cacheDir/$nluName")
        return nluFile.exists()
    }

    private fun decompressModels() {
        try {
            cacheAsset("nlu.tflite")
            cacheAsset("metadata.json")
            cacheAsset("vocab.txt")
            cacheAsset("minecraft-recipe.json")
        } catch (e: IOException) {
            Log.e("FINDMYREP", "Unable to cache NLU data", e)
        }
    }

    @Throws(IOException::class)
    private fun cacheAsset(assetName: String) {
        val assetFile = File("$cacheDir/$assetName")
        val inputStream: InputStream = assets.open(assetName)
        val size: Int = inputStream.available()
        val buffer = ByteArray(size)
        inputStream.read(buffer)
        inputStream.close()
        val fos = FileOutputStream(assetFile)
        fos.write(buffer)
        fos.close()
    }

    override fun onTrace(level: EventTracer.Level?, message: String?) {
        Log.w("FINDMYREP", message);
    }
}
