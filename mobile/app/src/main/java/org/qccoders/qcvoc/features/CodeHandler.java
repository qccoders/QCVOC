package org.qccoders.qcvoc.features;

import static android.view.KeyEvent.KEYCODE_VOLUME_DOWN;
import static android.view.KeyEvent.KEYCODE_VOLUME_UP;
import static org.qccoders.qcvoc.shared.Constants.devUrl;
import static org.qccoders.qcvoc.shared.Constants.prodUrl;

public class CodeHandler {
    private static boolean developmentEnvironment = false;

    private static final int[] environmentCode = {KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP,
            KEYCODE_VOLUME_DOWN, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_DOWN,
            KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP,
            KEYCODE_VOLUME_DOWN};
    private static int environmentCodeIndex = 0;

    public static String inputKey(int keycode){
        if (keycode == environmentCode[environmentCodeIndex]) {
            environmentCodeIndex += 1;
        } else {
            environmentCodeIndex = 0;
        }

        if (environmentCodeIndex == environmentCode.length) {
            environmentCodeIndex = 0;
            developmentEnvironment = !developmentEnvironment;

            return developmentEnvironment ? devUrl : prodUrl;
        }
        return null;
    }

}
