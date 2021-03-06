/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

package org.qccoders.qcvoc.features;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

import com.google.android.gms.samples.vision.barcodereader.BarcodeCaptureActivity;
import com.google.android.gms.vision.barcode.Barcode;

import static org.qccoders.qcvoc.shared.Constants.BARCODE_REQUEST;

public class BarcodeHandler {
    private Context mContext;
    private String callback;

    public BarcodeHandler(Context mContext) {
        this.mContext = mContext;
    }

    public void scanBarcode(String callback) {
        this.callback = callback;

        Intent intent = new Intent(mContext, BarcodeCaptureActivity.class);
        intent.putExtra(BarcodeCaptureActivity.AutoFocus, true);

        ((Activity) mContext).startActivityForResult(intent, BARCODE_REQUEST);
    }

    public String returnBarcode(Intent data) {
        Barcode barcode = data.getParcelableExtra(BarcodeCaptureActivity.BarcodeObject);
        Log.d("BarcodeHandler", barcode.displayValue);
        return callback + "('" + barcode.displayValue + "')";
    }
}
