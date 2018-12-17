package org.qccoders.qcvoc;

import android.net.Uri;

import static org.qccoders.qcvoc.Constants.devUrl;
import static org.qccoders.qcvoc.Constants.prodUrl;

class Utilities {
    public static boolean urlIsTrusted(String url) {
        String host = "http://" + Uri.parse(url).getHost();
        return host.equals(prodUrl) || host.equals(devUrl);
    }
}
