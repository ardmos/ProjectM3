package com.unity3d.player

import android.widget.Toast

class NativeToast {
    fun ShowToast(message: String) {
        UnityPlayer.currentActivity.runOnUiThread {
            val toast = Toast.makeText(UnityPlayer.currentActivity, message, Toast.LENGTH_SHORT)
            toast.show()
        }
    }
}
