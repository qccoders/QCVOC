package org.qccoders.qcvoc.shared;

import android.net.Uri;

import static org.qccoders.qcvoc.shared.Constants.devUrl;
import static org.qccoders.qcvoc.shared.Constants.prodUrl;

public class Utilities {
    public static boolean urlIsTrusted(String url) {
        String host = "http://" + Uri.parse(url).getHost();
        return host.equals(prodUrl) || host.equals(devUrl);
    }
}
